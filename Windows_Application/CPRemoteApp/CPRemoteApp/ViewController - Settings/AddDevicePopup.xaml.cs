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
// CPRemote Using Statements
using CPRemoteApp.Utility_Classes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CPRemoteApp.ViewController___Settings
{
    public sealed partial class AddDevicePopup : UserControl
    {
        private VolumeDevice vol_device = new VolumeDevice();
        private ChannelDevice chan_device = new ChannelDevice();
        // volume = true, channel = false
        private bool channel_or_volume = true;
        
        public AddDevicePopup()
        {
            this.InitializeComponent();
        }

        public async void validateName(object sender, RoutedEventArgs e)
        {
            // Check to make sure the name isn't blank or already used then change to first button to train screen
            if(channel_or_volume)
            {
                trainVolumeUp();
            }
            else
            {
                trainDigits();
            }
        }

        public void setDeviceType(bool chan_or_vol)
        {
            channel_or_volume = chan_or_vol;
            if(channel_or_volume)
            {
                heading_text.Text = "Create New Volume Device";
            }
            else
            {
                heading_text.Text = "Create New Channel Device";
            }
        }

        private void trainVolumeUp()
        {

        }

        private void trainVolumeDown()
        {

        }

        private void trainVolumeMute()
        {

        }

        private void trainDigits()
        {
            for(int digit = 0; digit < 10; digit++)
            {
                //Do Bluetooth call and wait for response.
            }
        }

        // Parameters TBD, will set the content to notify the user of which button to train
        private void setContent()
        {

        }

        private void displaySuccessMessage()
        {

        }

        private void displayErrorMessage()
        {

        }

    }// End of AddDevicePopup Class
}
