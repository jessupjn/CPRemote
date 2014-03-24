// CPRemote Using Statements
using CPRemoteApp.Utility_Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CPRemoteApp.ViewController___Settings
{
    public sealed partial class AddDevicePopup : UserControl
    {
        private VolumeDevice vol_device = new VolumeDevice();
        private ChannelDevice chan_device = new ChannelDevice();
        // volume = true, channel = false
        private bool channel_or_volume = true;
        private bool time_left = true;
        
        public AddDevicePopup()
        {
            this.InitializeComponent();
        }

        public async void validateName(object sender, RoutedEventArgs e)
        {
            // Check to make sure the name isn't blank or already used then change to first button to train screen
            string name = device_name_text.Text;
            //bool name_exists = await ((App)CPRemoteApp.App.Current).deviceController.device_input_file_exists(name, channel_or_volume);
            //if(name_exists)
            //{
                // TODO: Display warning/Confirmation Dialog
            //    return;
            //}
            
            device_name_text.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            device_name_block.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            next_button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if(channel_or_volume)
            {
                trainVolumeDevice(name);
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

        private async void trainVolumeDevice(string name)
        {
            // [0] protocol, [1] # IR Bits, [2] Vol Up IR Code, [3] Vol Down IR Code, [4] Mute IR Code
            List<string> IR_info = new List<string>();
            // TODO: Set the UI
            string vol_up_info = await getIRInfo();
            // get rid of -L on front of message
            getNextData(ref vol_up_info);
            string protocol = getNextData(ref vol_up_info);
            string vol_up_ir_code = getNextData(ref vol_up_info);
            string num_bits = getNextData(ref vol_up_info);
            IR_info.Add(protocol);
            IR_info.Add(num_bits);
            IR_info.Add(vol_up_ir_code);
            System.Diagnostics.Debug.WriteLine("protocol: " + protocol);
            System.Diagnostics.Debug.WriteLine("Num Bits: " + num_bits);
            System.Diagnostics.Debug.WriteLine("Vol up IR Code: " + vol_up_ir_code);
            string vol_down_info = await getIRInfo();
            getNextData(ref vol_down_info);
            string protocol_2 = getNextData(ref vol_down_info);
            if(protocol_2 != protocol)
            {
                // TODO: Handle Mismatching Protocols
            }
            string vol_down_ir_code = getNextData(ref vol_down_info);
            IR_info.Add(vol_down_ir_code);
            System.Diagnostics.Debug.WriteLine(vol_down_ir_code);
            string mute_info = await getIRInfo();
            getNextData(ref mute_info);
            string mute_protocol = getNextData(ref mute_info);
            if(mute_protocol != protocol)
            {
                // TODO: Handle Mismatching Protocols. Probably try again.
            }
            string mute_ir_code = getNextData(ref mute_info);
            IR_info.Add(mute_ir_code);
            ((App)CPRemoteApp.App.Current).deviceController.addVolumeDevice(name, IR_info);
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

        private async Task<string> getIRInfo()
        {
            string learn_ir_command = "-L";
            App.bm.rcvd_code = "";
            App.bm.OperateTVButton_Click(learn_ir_command);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += setTimeLeftFalse;
            timer.Start();
            time_left = true;
            while(App.bm.rcvd_code == "" && time_left)
            {
                await Task.Delay(TimeSpan.FromSeconds(.1));
            }
            timer.Stop();
            if(!time_left)
            {
                displayErrorMessage("Didn't receive remote input");
                await Task.Delay(TimeSpan.FromSeconds(2));
                return await getIRInfo();
            }
            string IR_info = App.bm.rcvd_code;
            return IR_info;
        }

        private void setTimeLeftFalse(Object sender, object e)
        {
            time_left = false;
        }

        private void displaySuccessMessage()
        {

        }

        private void displayErrorMessage(string message)
        {

        }

        private string getNextData(ref string info)
        {
            int index = info.IndexOf('.');
            string data = info.Substring(0, index);
            info = info.Substring(index + 1);
            return data;
        }


    }// End of AddDevicePopup Class
}
