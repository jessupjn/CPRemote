//======================================================authorship
//CC-BY Michael Osthege (2013)
//======================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TCD.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TCD.Arduino.Bluetooth;

namespace BluetoothCommunicationSampleController
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



    public sealed partial class MainPage : Page
    {
        private BluetoothConnectionManager connectionManager = new BluetoothConnectionManager();//mange the connection to another device
        private Dictionary<string, bool> leds = new Dictionary<string, bool>();                 //keep track of LED states
        
        public MainPage()
        {
            this.InitializeComponent();
            connectionManager.ExceptionOccured += delegate(object sender, Exception ex) { console.Text += ex.Message + "\n"; };
            connectionManager.MessageReceived += connectionManager_MessageReceived;
            connectionManager.StateChanged += connectionManager_StateChanged;
            connectionManager.State = BluetoothConnectionState.Disconnected;//to trigger UI update
        }
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            connectionManager.Disconnect();//clean up the mess
            base.OnNavigatedFrom(e);
        }
        private void WriteLine(string line)//write a new line to the "Console"
        {
            console.Text += line + "\n";
            scrollViewer.ChangeView(0, scrollViewer.ScrollableHeight, 1, false);
        }

        #region Lifecycle
        //control
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            leds.Clear();//reset LED state cache
            leds.Add("RED", false);
            leds.Add("GREEN", false);
            //ask the user to connect
            await connectionManager.EnumerateDevicesAsync((sender as Button).GetElementRect());
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            connectionManager.AbortConnection();//cancel current connection attempts
        }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            connectionManager.Disconnect();//disconnect
        }
        //react
        private void connectionManager_StateChanged(object sender, BluetoothConnectionState state)
        {
            //enable/disable according to the connection state
            redButton.IsEnabled = (state == BluetoothConnectionState.Connected);
            greenButton.IsEnabled = (state == BluetoothConnectionState.Connected);
            progBar.IsIndeterminate = (state == BluetoothConnectionState.Connecting);
            enumerateButton.IsEnabled = (state == BluetoothConnectionState.Disconnected);
            cancelButton.IsEnabled = (state == BluetoothConnectionState.Connecting);
            disconnectButton.IsEnabled = (state == BluetoothConnectionState.Connected);
        }
        #endregion

        #region Send & Receive
        private async void LEDButton_Click(object sender, RoutedEventArgs e)
        {
            //the Button controls are tagged with "GREEN" or "RED" in their XAML definition
            string ledcol = (sender as Button).Tag as string;
            //send ON or OFF commands according to the LEDs last known state
            string cmd = string.Format(leds[ledcol] ? "TURN_OFF_{0}" : "TURN_ON_{0}", ledcol);
            //try to send this message
            var res = await connectionManager.SendMessageAsync(cmd);
            if (res == 1)//log if successful
                WriteLine("> " + cmd);
        }
        private async void connectionManager_MessageReceived(object sender, string message)
        {
            int equIndex = message.IndexOf('=');
            if (equIndex > 0)//is this a report? (eg: "A0=245" alternative: confirmation like "RED_ON")
            {
                switch(message.Substring(0, equIndex))//we can receive more than one report...
                {
                    case "A0": A0bar.Value = Convert.ToInt32(message.Substring(equIndex + 1)); break;
                }
                if (A0bar.Value > 512)
                    await connectionManager.SendMessageAsync("TURN_ON_RED");
                else
                    await connectionManager.SendMessageAsync("TURN_OFF_RED");
            }
            else
            {
                switch (message)//interpret other messages
                {
                    case "RED_OFF": leds["RED"] = false; break;//remember the LED state
                    case "RED_ON": leds["RED"] = true; break;
                    case "GREEN_OFF": leds["GREEN"] = false; break;
                    case "GREEN_ON": leds["GREEN"] = true; break;
                }
            }
            //log incoming transmission
            WriteLine("< " + message);
        }        
        #endregion

        private void A0bar_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

        }
    }
}