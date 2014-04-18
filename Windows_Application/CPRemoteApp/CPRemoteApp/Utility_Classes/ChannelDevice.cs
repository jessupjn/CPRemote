// CPRemoteApp Namespaces
using CPRemoteApp.ViewController___Remote;
//System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Windows Namespaces
using Windows.Storage;
using Windows.Media;
using Windows.UI.Xaml.Controls;

namespace CPRemoteApp.Utility_Classes
{
    public class ChannelDevice : Device
    {
        private int num_channels = 0;
        private string[] digit_IR_codes = new string[10];
        
        public ChannelDevice() { }

        // Used when creating a channel device from a text file
        public ChannelDevice(string name_, StorageFile input_file) : base(name_, input_file)
        {
            // remote timer info set in initialize.
        }

        public ChannelDevice(string name_, StorageFile input_file, List<string> dev_info) : base(name_, input_file, dev_info)
        {
            for (int i = 0; i < 10; ++i)
            {
                digit_IR_codes[i] = dev_info[i + 2];
            }
            is_initialized = true;
            remote_timer.Interval = TimeSpan.FromSeconds(CPRemoteApp.App.button_scanner_interval);
            remote_timer.Tick += setAllowIRTransmission;
        }

        public void add_channel(RemoteButton new_chan)
        {
            num_channels++;
            buttonScanner.add_button(new_chan);
            saveDevice();
        }

        public void modify_channel()
        {

        }

        public void remove_channel(int index)
        {
            num_channels--;
            buttonScanner.removeButton(index);
            saveDevice();
        }

        public async Task initialize()
        {
            // Needs to initialize IR_Bits and IR_protocol
            IList<string> input = await FileIO.ReadLinesAsync(device_info_file);
            IR_protocol = input[0];
            IR_bits = input[1];
            // Initialize the Digit IR Codes
            for(int i = 0; i < 10; ++i)
            {
                digit_IR_codes[i] = input[i + 2];
            }
            string chan_num;
            string img_path;
            string chan_name;
            string chan_abbv;
            num_channels = Convert.ToInt32(input[12]);
            int index = 13;
            for(int i = 0; i < num_channels; i++)
            {
                chan_name = input[index++];
                chan_abbv = input[index++];
                img_path = input[index++];
                chan_num = input[index++];

                Uri img_uri = new Uri(img_path);
                buttonScanner.add_button(new RemoteButton(chan_name, chan_abbv, chan_num, 1, img_uri));
            }
            is_initialized = true;
            remote_timer.Interval = TimeSpan.FromSeconds(CPRemoteApp.App.button_scanner_interval);
            remote_timer.Tick += setAllowIRTransmission;
        }

        override public async void sendIRInfo()
        {
            try
            {
                string chan_number_str = buttonScanner.getCurrentButton().getChannelNumber();
                int num_digits = chan_number_str.Length;
                string bt_msg;
                string cur_IR_code;
                int cur_digit;
                for(int i = 0; i < num_digits; ++i)
                {
                    bt_msg = "-S." + IR_protocol + ".";
                    cur_digit = Convert.ToInt32(chan_number_str[i].ToString());
                    cur_IR_code = digit_IR_codes[cur_digit];
                    bt_msg += cur_IR_code + ".";
                    // .1/ for Number of Repitions
                    bt_msg += IR_bits + ".1/";
                    App.bm.OperateTVButton_Click(bt_msg);
                    System.Diagnostics.Debug.WriteLine(bt_msg);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception except)
            {
                System.Diagnostics.Debug.WriteLine("In Channel Device on Button Click" + except.Message);
            }
        }

        public async void saveDevice()
        {
            List<string> output = new List<string>();
            output.Add(IR_protocol);
            output.Add(IR_bits);
            for (int i = 0; i < 10; i++ )
            {
                output.Add(digit_IR_codes[i]);
            }
            output.Add(num_channels.ToString());
            output.AddRange(buttonScanner.get_save_output());
            await FileIO.WriteLinesAsync(device_info_file, output);
        }
    }
}
