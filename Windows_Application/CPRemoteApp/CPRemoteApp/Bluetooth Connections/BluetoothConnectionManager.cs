using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Popups;

using Windows.Storage;
using CPRemoteApp;


namespace TCD.Arduino.Bluetooth
{
    public class BluetoothConnectionManager
    {
        #region Events
        //OnStateChanged
        public delegate void AddOnStateChangedDelegate(object sender, BluetoothConnectionState state);
        public event AddOnStateChangedDelegate StateChanged;
        private void OnStateChangedEvent(object sender, BluetoothConnectionState state)
        {
            if (StateChanged != null)
                StateChanged(sender, state);
        }
        //OnExceptionOccured
        public delegate void AddOnExceptionOccuredDelegate(object sender, Exception ex);
        public event AddOnExceptionOccuredDelegate ExceptionOccured;
        private void OnExceptionOccuredEvent(object sender, Exception ex)
        {
            if (ExceptionOccured != null)
                ExceptionOccured(sender, ex);
        }
        //OnMessageReceived
        public delegate void AddOnMessageReceivedDelegate(object sender, string message);
        public event AddOnMessageReceivedDelegate MessageReceived;
        private void OnMessageReceivedEvent(object sender, string message)
        {
            if (MessageReceived != null)
                MessageReceived(sender, message);
        }
        #endregion

        #region Variables
        private IAsyncOperation<RfcommDeviceService> connectService;
        private IAsyncAction connectAction;
        private RfcommDeviceService rfcommService;
        private StreamSocket socket;
        private DataReader reader;
        private DataWriter writer;

        private BluetoothConnectionState _State;
        /// <summary>
        /// The BluetoothConnectionState of this instance.
        /// </summary>
        public BluetoothConnectionState State
        {
            get { return _State; }
            set { _State = value; OnStateChangedEvent(this, value); }
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Displays a PopupMenu for selection of the other Bluetooth device.
        /// Continues by establishing a connection to the selected device.
        /// </summary>
        /// <param name="invokerRect">for example: connectButton.GetElementRect();</param>
        public async Task EnumerateDevicesAsync(Rect invokerRect)
        {
            this.State = BluetoothConnectionState.Enumerating;
            var serviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            PopupMenu menu = new PopupMenu();
            foreach (var serviceInfo in serviceInfoCollection)
                menu.Commands.Add(new UICommand(serviceInfo.Name, new UICommandInvokedHandler(ConnectToServiceAsync), serviceInfo));
            if (serviceInfoCollection.Count == 0)
            {
                menu.Commands.Add(new UICommand("No device found..."));
                await menu.ShowForSelectionAsync(invokerRect);
                this.State = BluetoothConnectionState.Disconnected;
                return; 
            }

            var result = await menu.ShowForSelectionAsync(invokerRect);


        }
        
        public async void ConnectToServiceAsync(IUICommand command)
        {
           
            StorageFolder local_folder = App.appData.LocalFolder;
            StorageFolder devices_folder = await local_folder.CreateFolderAsync("devices_folder", CreationCollisionOption.OpenIfExists);
            StorageFile bluetooth_file = (StorageFile)await devices_folder.TryGetItemAsync("bluetooth_file.txt");
           
            string bluetooth_file_line=null; 
            //if file doesn't exist, return and wanting to connect at initialization, return 
            if(bluetooth_file == null && command == null)
            {
                return; 
            }
            //read from bluetooth file 
            if (bluetooth_file != null) { 
                bluetooth_file_line = await FileIO.ReadTextAsync(bluetooth_file);
                System.Diagnostics.Debug.WriteLine(bluetooth_file_line);
            }

            string serviceIDString;
            string serviceNameString;  

            if (command == null)
            {
                string[] parse_bluetooth_file_line = bluetooth_file_line.Split(',');
                serviceNameString = parse_bluetooth_file_line[0];
                serviceIDString = parse_bluetooth_file_line[1]; 
               
            }
            else
            {
                DeviceInformation serviceInfo = (DeviceInformation)command.Id;
                serviceIDString = serviceInfo.Id;
                serviceNameString = serviceInfo.Name;
            }
            //this.State = BluetoothConnectionState.Connecting;
            this.Disconnect(); 

            try
            {
                // Initialize the target Bluetooth RFCOMM device service
                connectService = RfcommDeviceService.FromIdAsync(serviceIDString);
                rfcommService = await connectService;
                if (rfcommService != null)
                {
                    // Create a socket and connect to the target 
                    socket = new StreamSocket();
                    connectAction = socket.ConnectAsync(rfcommService.ConnectionHostName, rfcommService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                    await connectAction;//to make it cancellable
                    writer = new DataWriter(socket.OutputStream);
                    reader = new DataReader(socket.InputStream);
                    Task listen = ListenForMessagesAsync();
                    this.State = BluetoothConnectionState.Connected;


                    //write device information
                    if (command != null)
                    {
                        StorageFile device_info = await devices_folder.CreateFileAsync("bluetooth_file.txt", CreationCollisionOption.ReplaceExisting);

                        await FileIO.WriteTextAsync(device_info, serviceNameString+ ','+ serviceIDString);
                    }

                }
                else
                {
                    OnExceptionOccuredEvent(this, new Exception("Unable to create service.\nMake sure that the 'bluetooth.rfcomm' capability is declared with a function of type 'name:serialPort' in Package.appxmanifest."));

                }
            }
            catch (TaskCanceledException)
            {
                this.State = BluetoothConnectionState.Disconnected;
            }
            catch (Exception ex)
            {
                this.State = BluetoothConnectionState.Disconnected;
                OnExceptionOccuredEvent(this, ex);
            }
        }

        /// <summary>
        /// Abort the connection attempt.
        /// </summary>
        public void AbortConnection()
        {
            if (connectService != null && connectService.Status == AsyncStatus.Started)
                connectService.Cancel();
            if (connectAction != null && connectAction.Status == AsyncStatus.Started)
                connectAction.Cancel();
        }
        /// <summary>
        /// Terminate an connection.
        /// </summary>
        public void Disconnect()
        {
            if (reader != null) {
                reader = null;
            }

            if (writer != null)
            {
                writer.DetachStream();
                writer = null;
            }
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            if (rfcommService != null)
            {
                rfcommService = null;
            }
            this.State = BluetoothConnectionState.Disconnected;
        }


        public bool isConnected ()
        {

            if (this.State == BluetoothConnectionState.Connected)
                return true;
            else
                return false; 

        }
        #endregion

        #region Send & Receive
        /// <summary>
        /// Send a string message.
        /// </summary>
        /// <param name="message">The string to send.</param>
        /// <returns></returns>
        public async Task<uint> SendMessageAsync(string message)
        {
            uint sentMessageSize = 0;
            try
            {
                if (writer != null)
                {
                    uint messageSize = writer.MeasureString(message);
                    writer.WriteByte((byte)messageSize);
                    sentMessageSize = writer.WriteString(message);
                    await writer.StoreAsync();

                    if (sentMessageSize <= 0)
                    {
                        Disconnect(); 
                    }
                }
            }
            catch (Exception ex)
            {
                this.State = BluetoothConnectionState.Disconnected;
            }
            return sentMessageSize;
        }
        private async Task ListenForMessagesAsync()
        {
            while (reader != null)
            {
                try
                {
                    // Read first byte (length of the subsequent message, 255 or less). 
                    uint sizeFieldCount = await reader.LoadAsync(1);
                    if (sizeFieldCount != 1)
                    {
                        // The underlying socket was closed before we were able to read the whole data. 
                        return;
                    }
                    
                    // Read the message. 
                    uint messageLength = reader.ReadByte();
                    uint actualMessageLength = await reader.LoadAsync(messageLength);
                    if (messageLength != actualMessageLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data. 
                        return;
                    }
                    // Read the message and process it.
                    string message = reader.ReadString(actualMessageLength);
                    OnMessageReceivedEvent(this, message);
                }
                catch (Exception ex)
                {
                    if (reader != null)
                    {
                        this.State = BluetoothConnectionState.Disconnected;
                        OnExceptionOccuredEvent(this, ex);
                    }
                }
            }
        }
        #endregion
    }
    public enum BluetoothConnectionState
    {
        Disconnected,
        Connected,
        Enumerating,
        Connecting
    }
}
