// CPRemote Using Statements
using CPRemoteApp.Utility_Classes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CPRemoteApp.ViewController___Settings
{
    public sealed partial class AddDevicePopup : UserControl
    {
        private VolumeDevice vol_device = new VolumeDevice();
        private ChannelDevice chan_device = new ChannelDevice();
        private List<string> IR_info = new List<string>();
        private WeakReference<Popup> popup_ref;
        private string device_name = "";
        private int cur_digit = 0;
        // volume = true, channel = false
        private bool channel_or_volume = true;
        private bool time_left = true;
        
        public AddDevicePopup()
        {
            this.InitializeComponent();
        }

        public void validateName(object sender, RoutedEventArgs e)
        {
            // Check to make sure the name isn't blank or already used then change to first button to train screen
            device_name = device_name_text.Text;
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
                trainVolumeDevice();
            }
            else
            {
                // Start at train Zero and Populate all of the IR information, via next button clicks
                trainZero();
            }
        }

        public void setParentPopup(ref Popup p)
        {
            popup_ref = new WeakReference<Popup>(p);
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

        public void closePopup(object sender, RoutedEventArgs e)
        {
            Popup pop;
            popup_ref.TryGetTarget(out pop);
            pop.IsOpen = false;
        }

        private async void trainVolumeDevice()
        {
            // [0] protocol, [1] # IR Bits, [2] Vol Up IR Code, [3] Vol Down IR Code, [4] Mute IR Code
            // TODO: Set the UI
            try
            {
                // Get Volume Up Info
                setContent("Volume Up");
                string vol_up_info = await getIRInfo();
                //await Task.Delay(TimeSpan.FromSeconds(2)); // Testing Only
                // Prompt User to press button
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
                next_button.Click -= validateName;
                next_button.Click += trainVolumeDown;
                displaySuccessMessage("Volume Up IR Code Successfully Learned!", false);
               
            }
            catch(Exception except)
            {
                displayErrorMessage(except.Message);
            }
        }

        private async void trainVolumeDown(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get Volume Down Info
                setContent("Volume Down");
                string vol_down_info = await getIRInfo();
                //await Task.Delay(TimeSpan.FromSeconds(2));// Can be removed later. Just for show while not using bt
                getNextData(ref vol_down_info);
                string protocol_2 = getNextData(ref vol_down_info);
                if(protocol_2 != IR_info[0])
                {
                    throw new Exception("Error: Remote Protocol not Consistent. Please Try Again");
                }
                string vol_down_ir_code = getNextData(ref vol_down_info);
                IR_info.Add(vol_down_ir_code);
                System.Diagnostics.Debug.WriteLine(vol_down_ir_code);
                next_button.Click -= trainVolumeDown;
                next_button.Click += trainMute;
                displaySuccessMessage("Volume Down IR Code Successfully Learned!", false);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
        }

        private async void trainMute(object sender, RoutedEventArgs e)
        {
            try
            {
                setContent("Mute");
                string mute_info = await getIRInfo();
                //await Task.Delay(TimeSpan.FromSeconds(2));
                getNextData(ref mute_info);
                string mute_protocol = getNextData(ref mute_info);
                if (mute_protocol != IR_info[0])
                {
                    throw new Exception("Error: Remote Protocol not Consistent. Please Try Again");
                }
                string mute_ir_code = getNextData(ref mute_info);
                IR_info.Add(mute_ir_code);
                displaySuccessMessage("Mute IR Code Successfully Learned!", true);
                ((App)CPRemoteApp.App.Current).deviceController.addVolumeDevice(device_name, IR_info);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
        }

        private async void trainZero()
        {
            try
            {
                next_button.Click -= validateName;
                next_button.Click += trainOne;
                await trainDigit(0);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
        }

        private async void trainOne(object sender, RoutedEventArgs e)
        { 
            try
            {
                next_button.Click -= trainOne;
                next_button.Click += trainTwo;
                await trainDigit(1);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
        }

        private async void trainTwo(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainTwo;
                next_button.Click += trainThree;
                await trainDigit(2);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async void trainThree(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainThree;
                next_button.Click += trainFour;
                await trainDigit(3);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async void trainFour(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainFour;
                next_button.Click += trainFive;
                await trainDigit(4);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async void trainFive(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainFive;
                next_button.Click += trainSix;
                await trainDigit(5);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async void trainSix(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainSix;
                next_button.Click += trainSeven;
                await trainDigit(6);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async void trainSeven(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainSeven;
                next_button.Click += trainEight;
                await trainDigit(7);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async void trainEight(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainEight;
                next_button.Click += trainNine;
                await trainDigit(8);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async void trainNine(object sender, RoutedEventArgs e)
        {
            try
            {
                next_button.Click -= trainNine;
                await trainDigit(9);
                ((App)CPRemoteApp.App.Current).deviceController.addChannelDevice(device_name, IR_info);
            }
            catch (Exception except)
            {
                displayErrorMessage(except.Message);
            }
            
        }

        private async Task trainDigit(int digit)
        {
      
            string digit_str = digit.ToString();
            setContent(digit_str);
            string digit_info = await getIRInfo();
            //await Task.Delay(TimeSpan.FromSeconds(2));
            getNextData(ref digit_info);
            string protocol = getNextData(ref digit_info);
            if(digit == 0)
            {
                // Set the Protocol and IR Bits in the IR_info List
                IR_info.Add(protocol);
                string code = getNextData(ref digit_info);
                string bits = getNextData(ref digit_info);
                IR_info.Add(bits);
                IR_info.Add(code);
            }
            else if(protocol != IR_info[0]) // Protocols don't match
            {
                throw new Exception("Error: Remote Protocol not Consistent. Please Try Again");
            }
            else
            {
                string code = getNextData(ref digit_info);
                IR_info.Add(code);
            }
            displaySuccessMessage( digit_str + " IR Code Successfully Learned!", digit == 9);
        }

        // Parameters TBD, will set the content to notify the user of which button to train
        private void setContent(string message)
        {
            System.Diagnostics.Debug.WriteLine("Set Content: " + message);
            press_button_command_block.Text = "Please Press the " + message + " button on your remote";
            press_button_command_block.Visibility = Windows.UI.Xaml.Visibility.Visible;
            next_button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            success_message_panel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async Task<string> getIRInfo()
        {
            string learn_ir_command = "-L./";
            App.bm.rcvd_code = "";
            App.bm.OperateTVButton_Click(learn_ir_command);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15);
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
                Exception e = new Exception("Didn't receive remote input");
            }
            string IR_info = App.bm.rcvd_code;
            return IR_info;
        }

        private void setTimeLeftFalse(Object sender, object e)
        {
            time_left = false;
        }

        private void displaySuccessMessage(string success_msg, bool last_button)
        {
            System.Diagnostics.Debug.WriteLine("Display Success Message: " + success_msg);
            if(last_button)
            {
                success_msg += " All IR Codes have been learned." + 
                    " The device has been created and set as the default " + 
                    (channel_or_volume ? "volume" : "channel") + " device.";
                
                close_button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                next_button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            success_msg_block.Text = success_msg;
            press_button_command_block.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            success_msg_block.Visibility = Windows.UI.Xaml.Visibility.Visible;
            success_message_panel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void displayErrorMessage(string error_msg)
        {
            error_msg_block.Text = error_msg;
            error_message_panel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private string getNextData(ref string info)
        {
            int index = info.IndexOf('.');
            if(index == -1)
            {
                index = info.IndexOf('/');
                if(index == -1)
                {
                    return "";
                }
            }
            string data = info.Substring(0, index);
            info = info.Substring(index + 1);
            return data;
        }




    }// End of AddDevicePopup Class
}
