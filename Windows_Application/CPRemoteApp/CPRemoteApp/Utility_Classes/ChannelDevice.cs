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
//using Windows.UI.Xaml.Controls;

namespace CPRemoteApp.Utility_Classes
{
    public class ChannelDevice : Device
    {
        private int num_channels = 0;

        public ChannelDevice() { }

        public ChannelDevice(string name_, StorageFile input_file) : base(name_, input_file)
        {
 
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
 
            IList<string> input = await FileIO.ReadLinesAsync(device_info_file);
            //Remove Comments
            Stack<int> comment_line_numbers = new Stack<int>();
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
            }
            num_channels = Convert.ToInt32(input[0]);
            int IR_code;
            string img_path;
            string chan_name;
            string chan_abbv;
            int index = 1;
            for(int i = 0; i < num_channels; i++)
            {
                chan_name = input[index++];
                chan_abbv = input[index++];
                img_path = input[index++];
                IR_code = Convert.ToInt32(input[index++]);

                //Uri img_uri = new Uri(img_path);
                //BitmapIcon icon = new BitmapIcon();
                //icon.UriSource = img_uri;
                
                buttonScanner.add_button(new RemoteButton(chan_name, chan_abbv, IR_code, 1));
            }
        }

        public async void saveDevice()
        {
            List<string> output = new List<string>();
            output.Add(num_channels.ToString());
            output.AddRange(buttonScanner.get_save_output());
            await FileIO.WriteLinesAsync(device_info_file, output);
        }
    }
}
