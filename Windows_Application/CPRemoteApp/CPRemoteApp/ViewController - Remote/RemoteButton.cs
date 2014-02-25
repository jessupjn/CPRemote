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

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CPRemoteApp.ViewController___Remote
{
    public sealed class RemoteButton : Windows.UI.Xaml.Controls.Button
    {

        private int IR_code = 0;
        private string IR_protocol = "";
        private int IR_repititions = 1;
        private string name = "";
        private string abbreviation = "";
        private BitmapIcon icon;

        public RemoteButton()
        {
            this.DefaultStyleKey = typeof(RemoteButton);
        }

        public RemoteButton(string name_, string abbv, int code, int repititions, Uri icon_path = null)// Needs to initialize icon with a new parameter
        {
            this.DefaultStyleKey = typeof(RemoteButton);
            IR_code = code;
            IR_repititions = repititions;
            name = name_;
            abbreviation = abbv;
            //icon = icon_;

            // Initializing Button Content
            Grid btn_grid = new Grid();
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(3, GridUnitType.Star);
            btn_grid.RowDefinitions.Add(row1);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(1, GridUnitType.Star);
            btn_grid.RowDefinitions.Add(row2);
            if(icon == null)
            {

            }
            else
            {
                icon = new BitmapIcon();
                icon.UriSource = icon_path;
                Grid.SetRow(icon, 0);
            }
            TextBox btn_name = new TextBox();
            btn_name.Text = name;
            btn_name.FontSize = 64.0;
            btn_name.IsReadOnly = true;
            Grid.SetRow(btn_name, 1);
            this.Content = btn_grid;
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
            output.Add(icon.BaseUri.ToString());
            output.Add(IR_code.ToString());
            return output;
        }
    }
}
