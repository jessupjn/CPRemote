//CPRemote Namespaces
using CPRemoteApp.ViewController___Remote;
// System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Windows Namespaces
using Windows.UI.Xaml;
using Windows.Storage;


namespace CPRemoteApp.Utility_Classes
{
    public abstract class Device
    {
        public RemoteButtonScanner buttonScanner { set; get; }
        public string IR_protocol = "";
        public string IR_bits = "";
        public bool is_initialized { get; set; }
        protected StorageFile device_info_file;
        protected DispatcherTimer remote_timer = new DispatcherTimer();
        protected bool allow_IR_transmission = true;
        private string name = "";
        
        public Device()
        {
            buttonScanner = new RemoteButtonScanner();
            //device_info_file = new StorageFile();
        }

        public Device(string name_, StorageFile input_file)
        {
            name = name_;
            buttonScanner = new RemoteButtonScanner();
            device_info_file = input_file;
        }

        public Device(string name_, StorageFile input_file, List<string> IR_info)
        {
            name = name_;
            buttonScanner = new RemoteButtonScanner();
            device_info_file = input_file;
            IR_protocol = IR_info[0];
            IR_bits = IR_info[1];
        }

        public string get_name()
        {
            return name;
        }

        public void onButtonClick()
        {
            if (!allow_IR_transmission)
            {
                return;
            }
            buttonScanner.stop();
            allow_IR_transmission = false;
            //Function call to send IR
            sendIRInfo();
            buttonScanner.start();
            remote_timer.Start();
        }

        protected void setAllowIRTransmission(Object sender, object e)
        {
            allow_IR_transmission = true;
            remote_timer.Stop();
        }



        // ------------------------ Abstract Functions to be written by Derived Classes -------------------

        abstract public void sendIRInfo();

    }
}
