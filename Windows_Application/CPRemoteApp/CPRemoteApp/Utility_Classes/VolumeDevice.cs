//CPRemoteApp Namespaces
using CPRemoteApp.ViewController___Remote;
// System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Windows Namespaces
using Windows.Storage;
using Windows.Storage.Streams;

namespace CPRemoteApp.Utility_Classes
{
    public class VolumeDevice : Device
    {
        int volume_increments = 0;
        int volume_up_ir_code = 0;
        int volume_down_ir_code = 0;

        public VolumeDevice() { }

        public VolumeDevice(string name_, StorageFile input_file) : base(name_, input_file)
        {

        }

        // Must return a Task, so that it can be awaited on
        public async Task initialize()
        {
            IList<string> input = await FileIO.ReadLinesAsync(device_info_file);
            int data_lines_read = 0;
            //Read Volume Settings from Input File
            for(int i = 0; i < input.Count; i++)
            {
                if(input[i].ElementAt(0) == '#')
                {
                    continue;
                }
                switch (data_lines_read)
                {
                    case 0 :
                        volume_increments = Convert.ToInt32(input[i]);
                        break;
                    case 1 :
                        volume_up_ir_code = Convert.ToInt32(input[i]);
                        break;
                    case 2 :
                        volume_down_ir_code = Convert.ToInt32(input[i]);
                        break;
                }
                data_lines_read++;
            }
            // Create Volume buttons
            string down_name;
            string down_abbv;
            string up_name;
            string up_abbv;
            for(int x = 1; x < 4; x++)
            {
                down_name = "Down " + x.ToString();
                down_abbv = "-" + x.ToString();
                RemoteButton down_btn = new RemoteButton(down_name, down_abbv, volume_down_ir_code, x * volume_increments);
                up_name = "Up " + x.ToString();
                up_abbv = "+" + x.ToString();
                RemoteButton up_btn = new RemoteButton(up_name, up_abbv, volume_up_ir_code, x * volume_increments);
                buttonScanner.add_button(down_btn);
                buttonScanner.add_button(up_btn);
            }
        }

        public async void saveDevice()
        {
            List<String> output = new List<string>() {volume_increments.ToString(), volume_up_ir_code.ToString(), volume_down_ir_code.ToString() };
            await FileIO.WriteLinesAsync(device_info_file, output);
        }

        
    }// End of Volume Device Class
}// End of Namespace
