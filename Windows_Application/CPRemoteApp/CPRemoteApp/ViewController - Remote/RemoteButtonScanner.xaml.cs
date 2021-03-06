﻿// System Namespaces
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
        //public Button cur_button { get; set;}
        public bool clickEventSet { get; set; }
        public ObservableCollection<string> abbreviations = new ObservableCollection<string>();
        private int cur_index = 0;
        private DispatcherTimer timer = new DispatcherTimer();
        private const int MAX_BUTTONS_SHOWN = 5;
        // -------------------------------------

        public RemoteButtonScanner()
        {
            this.InitializeComponent();
            clickEventSet = false;
            buttons = new List<RemoteButton>();
            buttonList.ItemsSource = abbreviations;
            buttonList.SelectionMode = ListViewSelectionMode.Single;
            timer.Interval = TimeSpan.FromSeconds(CPRemoteApp.App.button_scanner_interval);
            timer.Tick += incrementScanner;
        }

        public void start()
        {
            buttonList.SelectedIndex = abbreviations.Count - 1;
            //buttonList.SelectionMode = ListViewSelectionMode.None;
            //buttonList.
            timer.Start();
        }

        public void stop()
        {
            timer.Stop();
        }

        public void setDimensions(double height_, double width_)
        {
            this.Height = height_;
            this.Width = width_;
            double image_dimension = 4 * height_ / 5;
            setCurrentImage(image_dimension);
            cover_panel.Height = height_;
            cover_panel.Width = width_;
            content_grid.Height = height_;
            content_grid.Width = width_;
        }

        public void setCurrentImage(double dimmension)
        {
            if (cur_index < buttons.Count)
            {
              this.cur_image.Margin = new Thickness(0, dimmension / 8, 0, 0);
              this.cur_image.Height = 5 * dimmension / 8;
              this.cur_image.Width = 5 * dimmension / 8;
              this.cur_image.Source = buttons[cur_index].icon;
            }
        }

        public void add_button(RemoteButton btn, bool reset_scanner)
        {
            buttons.Add(btn);
            if (reset_scanner)
            {
                resetScanner();
            }
            else if(abbreviations.Count < MAX_BUTTONS_SHOWN)
            {
                abbreviations.Insert(0, btn.getName());
            }
        }

        public void update_button(int index, RemoteButton btn)
        {
            RemoteButton old_button = buttons.ElementAt(index);
            buttons[index] = btn;
            resetScanner();
        }

        public void removeButton(int index)
        {
            RemoteButton btn = buttons.ElementAt(index);
            buttons.RemoveAt(index);
            resetScanner();
        }

        public List<RemoteButton> getButtons()
        {
            return buttons;
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

        private void resetScanner()
        {
            abbreviations.Clear();
            for (int i = 0; i < MAX_BUTTONS_SHOWN; ++i)
            {
                if (i == buttons.Count)
                {
                    break;
                }
                abbreviations.Insert(0, buttons[i].getName());
            }
            cur_index = 0;
            cur_image.Source = buttons[cur_index].icon;
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
