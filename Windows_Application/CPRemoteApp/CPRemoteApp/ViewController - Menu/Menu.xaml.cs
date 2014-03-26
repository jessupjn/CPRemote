using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CPRemoteApp.Bluetooth_Connections;
using Windows.UI;
using CPRemoteApp.Utility_Classes;
using Windows.Storage;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CPRemoteApp
{
    /// <summary>
    /// The main menu and opening page for CPRemote.
    /// </summary>
    /// 
    public sealed partial class Menu : Page
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public Menu()
        {
            this.InitializeComponent();

            Canvas.SetLeft(titleText, (Window.Current.Bounds.Width - titleText.Width) / 2);

            // customization of _goToRemote
            _goToRemote_frame.Width = 0.7 * Window.Current.Bounds.Height;
            _goToRemote_frame.Height = _goToRemote.Width = _goToRemote.Height = _goToRemote_indicator.Width = _goToRemote_indicator.Height = _goToRemote_frame.Width;
            _goToRemote_indicator.RadiusX = _goToRemote_indicator.RadiusY = _goToRemote_indicator.Width / 2;
            Canvas.SetLeft(_goToRemote_frame, (Window.Current.Bounds.Width - _goToRemote_frame.Width) / 2);
            Canvas.SetTop(_goToRemote_frame, 0.15 * Window.Current.Bounds.Height);

        
            // customization of _goToSettings
            Canvas.SetLeft(_goToSettings_frame, 25);
            Canvas.SetTop(_goToSettings_frame, 25 );


            // customization of _bluetooth_status_frame
            Canvas.SetLeft(_bluetooth_status_frame, Window.Current.Bounds.Width - 100);
            Canvas.SetTop(_bluetooth_status_frame, 25);
            if (((App)(CPRemoteApp.App.Current)).deviceController.isInitialized() == false)
            {
                loadDeviceManager();
            }
            // check for bluetooth status every 0.2 seconds.
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += checkBluetoothStatus;
            //timer.Start();
            

        }

        private async void loadDeviceManager()
        {
            DeviceManager device_manager = ((App)(CPRemoteApp.App.Current)).deviceController;
            StorageFolder local_folder = App.appData.LocalFolder;
            StorageFolder devices_folder = await local_folder.CreateFolderAsync("devices_folder", CreationCollisionOption.OpenIfExists);

            //TODO: Should check the return value of device_manager to ensure devices were loaded properly
            await device_manager.initialize(devices_folder);
        }




        private void checkBluetoothStatus(object sender, object e)
        {
          bool connected = App.bm.connectionManager_isConnected(sender);
          System.Diagnostics.Debug.WriteLine(connected);


          SolidColorBrush fill;

          if(connected)
          {
            Color color = Colors.GreenYellow;
            color.A = 160;
            fill = new SolidColorBrush(color);
          }
          else
          {
            Color color = Colors.DarkRed;
            color.A = 160;
            fill = new SolidColorBrush(color);
          }

          _bluetooth_status_indicator.Fill = fill;
        }

        private void remoteClick(object sender, RoutedEventArgs e)
        {
          // TODO: ADD DEVICE-CHECK
          if( /* DEVICES ARE SET */ true )
             this.Frame.Navigate(typeof(ViewController___Remote.RemoteMenu));
          else
          {
            MessageDialog msgDialog = new MessageDialog("Devices are not set!", "Please set up your devices in the settings menu!");
            UICommand okBtn = new UICommand("OK");
            okBtn.Invoked += delegate { };
            msgDialog.Commands.Add(okBtn);
            msgDialog.ShowAsync();
          }
        }

        private void settingsClick(object sender, RoutedEventArgs e)
        {
          if (_goToSettings_indicator.Visibility == Visibility.Visible) this.Frame.Navigate(typeof(ViewController___Settings.SettingsMenu));
        }


        private void enterHighlight(object sender, PointerRoutedEventArgs e)
        {
            if (sender.Equals(_goToRemote)) { _goToRemote_indicator.Visibility = Visibility.Visible; }
            else if (sender.Equals(_goToSettings)) { _goToSettings_indicator.Visibility = Visibility.Visible; }
        }

        private void exitHighlight(object sender, PointerRoutedEventArgs e)
        {
            if (sender.Equals(_goToRemote)) { _goToRemote_indicator.Visibility = Visibility.Collapsed; }
            else if (sender.Equals(_goToSettings)) { _goToSettings_indicator.Visibility = Visibility.Collapsed; }
        }

    }
}
