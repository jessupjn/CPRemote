// System Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.ObjectModel;
// Windows Namespaces
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CPRemoteApp.ViewController___Remote
{
    public sealed partial class RemoteButtonScanner : UserControl
    {
        // -------------------------------------
        private List<RemoteButton> buttons;
        public Button cur_button { get; set;}
        public ObservableCollection<string> abbreviations = new ObservableCollection<string>();
        private int cur_index = 0;
        private DispatcherTimer timer = new DispatcherTimer();
        private static int MAX_BUTTONS_SHOWN = 5;
        // -------------------------------------

        public RemoteButtonScanner()
        {
            this.InitializeComponent();

            imgCanvas.Width = cur_button_panel.Width;
            imgCanvas.Height = cur_button_panel.Height;

            buttons = new List<RemoteButton>();
            buttonList.ItemsSource = abbreviations;
            buttonList.SelectionMode = ListViewSelectionMode.Single;
            timer.Interval = TimeSpan.FromSeconds(CPRemoteApp.App.button_scanner_interval);
            timer.Tick += incrementScanner;
        }

        public void start()
        {
            buttonList.SelectedIndex = abbreviations.Count - 1;
            timer.Start();
        }

        public void stop()
        {
            timer.Stop();
        }

        public void setCurrentImage(double dimmension)
        {
            //Canvas.SetLeft(this.cur_image, (this.imgCanvas.Width - this.cur_image.Width) / 2);
            //Canvas.SetTop(this.cur_image, (this.imgCanvas.Height - this.cur_image.Height) / 2);
            if (cur_index < buttons.Count)
            {
                this.cur_image.Height = dimmension;
                this.cur_image.Width = dimmension;
                this.cur_image.Source = buttons[cur_index].icon;
            }
        }

        

        public void add_button(RemoteButton btn)
        {
            buttons.Add(btn);
            if(abbreviations.Count < MAX_BUTTONS_SHOWN)
            {
                abbreviations.Insert(0, btn.getName());
            }
        }

        //Should take in an input stream that it can read from to initialize all the buttons associated with it
        public List<String> get_save_output()
        {
            List<String> output = new List<String>();
            foreach(RemoteButton btn in buttons)
            {
                output.AddRange(btn.get_save_output());
            }
            return output;
        }

        public RemoteButton getCurrentButton()
        {
          if (buttons.Count > 0) return buttons[cur_index];
          else return new RemoteButton();
        }

        // Changes the interval for the button scanner to change buttons
        public void updateTimerInterval()
        {
            timer.Interval = TimeSpan.FromSeconds(CPRemoteApp.App.button_scanner_interval);
        }

        private void incrementScanner(Object sender, object e)
        {
            // Check for current index going past the end
            if(buttons.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No Buttons in Button Scanner");
                return;
            }
            if(cur_index == buttons.Count - 1)
            {
                cur_index = 0;
            }
            else
            {
                cur_index++;
            }
            if (buttons.Count <= MAX_BUTTONS_SHOWN)
            {
                string to_add = abbreviations.ElementAt(abbreviations.Count - 1);
                abbreviations.RemoveAt(abbreviations.Count - 1);
                abbreviations.Insert(0, to_add);
                buttonList.SelectedIndex = abbreviations.Count - 1;
                
            }
            else
            {
                int index_to_add = cur_index + (MAX_BUTTONS_SHOWN - 1);
                if (index_to_add >= buttons.Count)
                {
                    index_to_add -= buttons.Count;
                }
                abbreviations.RemoveAt(abbreviations.Count - 1);
                abbreviations.Insert(0, buttons.ElementAt(index_to_add).getName());
                buttonList.SelectedIndex = abbreviations.Count - 1;
            }
            this.cur_image.Source = buttons[cur_index].icon;
        }
    }// End of RemoteButtonScanner Class
}
