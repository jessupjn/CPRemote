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
        private Device new_device = new VolumeDevice();
        // volume = true, channel = false
        private bool channel_or_volume = true;
        
        public AddDevicePopup()
        {
            this.InitializeComponent();
        }

        public void validateName(object sender, RoutedEventArgs e)
        {

        }

        public void setDevice(Device dev, bool chan_or_vol)
        {
            channel_or_volume = chan_or_vol;
            new_device = dev;
            if(channel_or_volume)
            {
                heading_text.Text = "Create New Volume Device";
            }
            else
            {
                heading_text.Text = "Create New Channel Device";
            }
        }
    }
}
