using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using TCD.Controls;
using Windows.UI.Xaml.Controls;
using TCD.Arduino.Bluetooth;
using Windows.Storage;


namespace CPRemoteApp.Bluetooth_Connections
{
    public sealed partial class BluetoothController
    {
        /// <summary>
        /// IMPORTANT:
        /// in Package.appxmanifest make sure that you declare the following capabilities:
        /*
        ...
        <Capabilities>
        <m2:DeviceCapability Name="bluetooth.rfcomm">
            <m2:Device Id="any">
            <m2:Function Type="name:serialPort" />
            </m2:Device>
        </m2:DeviceCapability>
        </Capabilities>
        ...
        </Package>
            */
        /// </summary>
        /// 

        public string rcvd_code { get; set; }
        private BluetoothConnectionManager connectionManager = new BluetoothConnectionManager();//mange the connection to another device
        DateTime last_alive_time = new DateTime();
        
        public BluetoothController()
        {
            rcvd_code = "";
            connectionManager.ExceptionOccured += delegate(object sender, Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message + "\n"); };
            connectionManager.MessageReceived += connectionManager_MessageReceived;
            connectionManager.StateChanged += connectionManager_StateChanged;
            connectionManager.State = BluetoothConnectionState.Disconnected;//to trigger UI update

        }

        #region Lifecycle
        //control
        public async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            //ask the user to connect
          await connectionManager.EnumerateDevicesAsync((sender as Canvas).GetElementRect());
          if (connectionManager.isConnected())
          {
              last_alive_time = DateTime.Now;
          }

        }
        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            connectionManager.AbortConnection();//cancel current connection attempts
        }
        public void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            connectionManager.Disconnect();//disconnect
        }

        public async void connectToDefault(object sender, RoutedEventArgs e)
        {
            connectionManager.ConnectToServiceAsync(null);
            if (connectionManager.isConnected())
            {
                last_alive_time = DateTime.Now;
            }
        }
        //react
        public void connectionManager_StateChanged(object sender, BluetoothConnectionState state)
        {
            //enable/disable according to the connection state
            //enumerateButton.IsEnabled = (state == BluetoothConnectionState.Disconnected);
            //cancelButton.IsEnabled = (state == BluetoothConnectionState.Connecting);
            // disconnectButton.IsEnabled = (state == BluetoothConnectionState.Connected);
        }

        public string connectedDeviceName()
        {
            return connectionManager.connectedDeviceName(); 
        }

        //react
        public bool connectionManager_isConnected(object sender)
        {
           TimeSpan five_second = new TimeSpan(0, 0, 0, 20, 0);

           if (connectionManager.isConnected())
           {

               TimeSpan sub = DateTime.Now.Subtract(last_alive_time);
               System.Diagnostics.Debug.WriteLine(sub);
               if (sub.CompareTo(five_second) == -1)
               {
                   return true;
               }
           }
            
            return false; 

         }
        #endregion

        #region Send & Receive
        public async void OperateTVButton_Click(string message)
        {
         
            var res = await connectionManager.SendMessageAsync(message);
            if (res == 1)//log if successful
                System.Diagnostics.Debug.WriteLine("Sent: " + message);
        }
        public void connectionManager_MessageReceived(object sender, string message)
        {

            switch (message)//interpret other messages
            {
               
            }

            if (message.StartsWith("-L."))
            {
                System.Diagnostics.Debug.WriteLine("Learn code received");
                rcvd_code = message; 

            }

            //for every received message, reset the timer
            last_alive_time = DateTime.Now;


            //log incoming transmission
            System.Diagnostics.Debug.WriteLine("Received: " + message);
        }
        #endregion



    }
}
