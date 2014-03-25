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
 
        }

        public ChannelDevice(string name_, StorageFile input_file, List<string> dev_info) : base(name_, input_file, dev_info)
        {
            for (int i = 0; i < 10; ++i)
            {
                digit_IR_codes[i] = dev_info[i + 2];
            }
        }

        public void add_channel()
        {

        }

        public void modify_channel()
        {

        }

        public void remove_channel()
        {

        }

        public async Task initialize()
        {
            // Needs to initialize IR_Bits and IR_protocol
            IList<string> input = await FileIO.ReadLinesAsync(device_info_file);
            //Remove Comments
            /*Stack<int> comment_line_numbers = new Stack<int>();
            for(int i =0; i < input.Count; i++)
            {
                if(input[i].ElementAt(0) == '#')
               {
                    comment_line_numbers.Push(i);
                }
            }
            while(comment_line_numbers.Count != 0)
            {
                input.RemoveAt(comment_line_numbers.Pop());
            }*/
            // Protocol[0], IR_Bits[1], Digit IR Codes[2-11], Num Buttons[12]
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
                //BitmapIcon icon = new BitmapIcon();
                //icon.UriSource = img_uri;
                
                buttonScanner.add_button(new RemoteButton(chan_name, chan_abbv, chan_num, 1, img_uri));
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
