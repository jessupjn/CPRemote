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
using Windows.Storage;
using System.Threading.Tasks;

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
            string name = device_name_text.Text;
            bool name_exists = await ((App)CPRemoteApp.App.Current).deviceController.device_input_file_exists(name, channel_or_volume);
            if(name_exists)
            {
                // TODO: Display warning/Confirmation Dialog
                return;
            }
            
            device_name_text.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            device_name_block.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            
            if(channel_or_volume)
            {
                await trainVolumeDevice(name);
            }
            else
            {
                await trainChannelDevice(name);
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

        private async Task<string> trainVolumeDevice(string name)
        {
            string result = "";
            // [0] protocol, [1] # IR Bits, [2] Vol Up IR Code, [3] Vol Down IR Code, [4] Mute IR Code
            List<string> IR_info = new List<string>();
            // TODO: Set the UI
            // Wait for the IR Info
            // If Successful display success
                // Wait 1 Sec
                // return result
            // Else
                // Display Error Message

            return result;

        }


        private async Task<string> trainChannelDevice(string name)
        {
            for(int digit = 0; digit < 10; digit++)
            {
                //Do Bluetooth call and wait for response.
            }
            string result = "";
            return result;
        }

        // Parameters TBD, will set the content to notify the user of which button to train
        private void setContent()
        {

        }

        /*private async Task<string> getIRInfo()
        {
            string learn_ir_command = "learn";
            App.bm.OperateTVButton_Click(learn_ir_command);
        }*/

        private void displaySuccessMessage()
        {

        }

        private void displayErrorMessage()
        {

        }

    }// End of AddDevicePopup Class
}
