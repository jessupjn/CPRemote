// Systems Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Windows Namespaces
using Windows.Storage;

namespace CPRemoteApp.Utility_Classes
{
    public class DeviceManager
    {
        StorageFolder devices_folder;
        StorageFile devices_info_file;
        List<ChannelDevice> channel_devices;
        List<VolumeDevice> volume_devices;
        public VolumeDevice volumeController { private set; get; }
        public ChannelDevice channelController { private set; get; }
        public DeviceManager()
        {
            volumeController = new VolumeDevice();
            channelController = new ChannelDevice();
            channel_devices = new List<ChannelDevice>();
            volume_devices = new List<VolumeDevice>();
        }

        //Initialize "devices" list and Channel and volumeDevices
        public async Task initialize(StorageFolder devices_folder_)
        {
            // Device Manager Metadata Initialization 
            devices_folder = devices_folder_;
            devices_info_file = (StorageFile) await devices_folder.TryGetItemAsync("devices_info.txt");
            if(devices_info_file == null)
            {
                // TODO: Display Warning about no devices
                return;
                /*StorageFile chan_test = await devices_folder.CreateFileAsync("channel_test.txt", CreationCollisionOption.ReplaceExisting);
                List<string> channel_file_test_lines = new List<string>() {"7", "Disney Channel", "Disn", "ms-appx:///img/disney_icon.png", "754",
                                                                                                         "Cartoon Network", "Cart", "ms-appx:///img/cartoon_network_icon.png", "456",
                                                                                                         "Nickelodeon", "Nick", "ms-appx:///img/nickelodeon_icon.png", "987",
                                                                                                         "Toon Disney", "Toon", "ms-appx:///img/toon_disney_icon.png", "111",
                                                                                                         "ABC", "ABC", "ms-appx:///img/abc_icon.png", "222",
                                                                                                         "ESPN", "ESPN", "ms-appx:///img/espn_icon.jpg", "333",
                                                                                                         "NBC", "NBC", "ms-appx:///img/nbc_icon.png", "212"
                };
                await FileIO.WriteLinesAsync(chan_test, channel_file_test_lines);
                channelController = new ChannelDevice("Testing_channel", chan_test);
                await channelController.initialize();

                StorageFile vol_test = await devices_folder.CreateFileAsync("volume_test.txt", CreationCollisionOption.ReplaceExisting);
                List<string> vol_file_test_lines = new List<string>() { "1", "NEC", "32", "2011254795", "2011246606" };
                await FileIO.WriteLinesAsync(vol_test, vol_file_test_lines);
                volumeController = new VolumeDevice("Testing_volume", vol_test);
                await volumeController.initialize();
                return;*/
            }
            IList<string> input = await FileIO.ReadLinesAsync(devices_info_file);
            int num_channel_devices = Convert.ToInt32(input[0]);
            int cur_index = 1;
            String device_name;
            StorageFile cur_device_input_file;

          


            // Channel Device Initialization

            for(int i = 0; i < num_channel_devices; ++i)
            {
                device_name = input[cur_index++];
                cur_device_input_file = await get_input_file_from_name(device_name, 'c');
                ChannelDevice c_device = new ChannelDevice(device_name, cur_device_input_file);
                channel_devices.Add(c_device);
            }
            // Get Current Channel Device
            int chan_index = channel_devices.FindIndex(x => x.get_name().Equals(input[cur_index++]));
            if(chan_index < 0)
            {
                // Throw Exception about channel device not found
            }
            channelController = channel_devices[chan_index];
            await channelController.initialize();

            // Volume Device Initialization

            int num_vol_devices = Convert.ToInt32(input[cur_index++]);
            for(int i = 0; i < num_vol_devices; ++i)
            {
                device_name = input[cur_index++];
                cur_device_input_file = await get_input_file_from_name(device_name, 'v');
                VolumeDevice v_device = new VolumeDevice(device_name, cur_device_input_file);
                volume_devices.Add(v_device);
            }
            int vol_index = volume_devices.FindIndex(x => x.get_name().Equals(cur_index++));
            if(vol_index < 0)
            {
                // Throw Exception about Volume device not found
            }
            volumeController = volume_devices[vol_index];
            await volumeController.initialize();
        }

        public async void saveDeviceList()
        {
            // # of channel devices, name of each channel device, cur channel device, 
            StorageFile info_file = await devices_folder.CreateFileAsync("device_info.txt", CreationCollisionOption.ReplaceExisting);
            List<string> save_output = new List<string>();
            save_output.Add(channel_devices.Count.ToString());
            foreach(ChannelDevice cur_dev in channel_devices)
            {
                save_output.Add(cur_dev.get_name());
            }
            save_output.Add(channelController.get_name());
            //# of vol devices, name of vol devices, cur vol device
            save_output.Add(volume_devices.Count.ToString());
            foreach(VolumeDevice cur_dev in volume_devices)
            {
                save_output.Add(cur_dev.get_name());
            }
            save_output.Add(volumeController.get_name());
            await FileIO.WriteLinesAsync(info_file, save_output);
        }

        public async void addChannelDevice(string name, List<string> IR_info)
        {
            string i_file_name = get_input_file_name(name, 'c');
            StorageFile chan_file = await devices_folder.CreateFileAsync(i_file_name, CreationCollisionOption.ReplaceExisting);
            ChannelDevice channel_dev = new ChannelDevice(name, chan_file, IR_info);
            channel_devices.Add(channel_dev);
            channel_dev.saveDevice();
            saveDeviceList();
        }

        public async void addVolumeDevice(string name, List<string> IR_info)
        {
            string i_file_name = get_input_file_name(name, 'v');
            StorageFile vol_file = await devices_folder.CreateFileAsync(i_file_name, CreationCollisionOption.ReplaceExisting);
            VolumeDevice vol_dev = new VolumeDevice(name, vol_file, IR_info);
            volume_devices.Add(vol_dev);
            vol_dev.saveDevice();
            saveDeviceList();
        }

        public void removeChannelDevice()
        {

        }

        public void removeVolumeDevice()
        {

        }

        public void editChannelDevice()
        {

        }

        public void editVolumeDevice()
        {

        }

        private string get_input_file_name(string name, char postfix)
        {
            name.Replace(" ", "_");
            name += "_" + postfix;
            return name;
        }

        public async Task<bool> device_input_file_exists(string dev_name, bool channel_or_volume)
        {
            char postfix;
            if(channel_or_volume)
            {
                postfix = 'v';
            }
            else
            {
                postfix = 'c';
            }
            string file_name = get_input_file_name(dev_name, postfix);
            StorageFile input_file = (StorageFile) await devices_folder.TryGetItemAsync(file_name);
            bool r_val;
            if (input_file.IsEqual(null))
            {
                r_val = false;
            }
            else
            {
                r_val = true;
            }
            return r_val;
        }

        private async Task<StorageFile> get_input_file_from_name(string name, char postfix)
        {
            //name.Replace(" ", "_");
            //name += "_" + postfix;
            name = get_input_file_name(name, postfix);
            StorageFile input_file = (StorageFile) await devices_folder.TryGetItemAsync(name);
            if(input_file.IsEqual(null))
            {
                //TODO: Throw Null File Exception Error
            }
            return input_file;
        }

    }
}
