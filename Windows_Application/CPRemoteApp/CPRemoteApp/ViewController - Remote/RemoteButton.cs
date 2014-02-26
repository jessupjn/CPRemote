// System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
// Windows Namespaces
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CPRemoteApp.ViewController___Remote
{
    public class RemoteButton
    {

        private int IR_code = 0;
        private string IR_protocol = "";
        private int IR_repititions = 1;
        private string name = "";
        private string abbreviation = "";
        public BitmapImage icon { get; set; }

        public RemoteButton()
        {
            icon = new BitmapImage();
        }

        public RemoteButton(string name_, string abbv, int code, int repititions, Uri icon_path)
        {
            IR_code = code;
            IR_repititions = repititions;
            name = name_;
            abbreviation = abbv;
            icon = new BitmapImage(icon_path);
        }

        public string getName()
        {
            return name;
        }

        public string getAbbreviation()
        {
            return abbreviation;
        }

        public List<string> get_save_output()
        {
            List<string> output = new List<string>();
            output.Add(name);
            output.Add(abbreviation);
            output.Add(icon.UriSource.AbsolutePath);
            output.Add(IR_code.ToString());
            return output;
        }
    }
}
