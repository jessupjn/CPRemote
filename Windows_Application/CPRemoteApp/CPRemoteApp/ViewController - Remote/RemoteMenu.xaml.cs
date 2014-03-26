// CPRemoteApp Namespaces
using CPRemoteApp.Utility_Classes;
// System Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CPRemoteApp.ViewController___Remote
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoteMenu : Page
    {
        

        private int status = 0;
        private bool can_move = true;
        private DispatcherTimer timer = new DispatcherTimer();


        // ============================================================================================================================================
        // Constructor
        public RemoteMenu()
        {
            this.InitializeComponent();
            int offset = 200;

            // check for bluetooth status every 0.2 seconds
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Tick += checkBluetoothStatus;
            //timer.Start();


            // customization of _bluetooth_status_frame
            Canvas.SetLeft(_bluetooth_status_frame, Window.Current.Bounds.Width - 100);
            Canvas.SetTop(_bluetooth_status_frame, 25);

            double h_w = Window.Current.Bounds.Height / 2.5;
            _volume_img.Width = _volume_img.Height = _channel_img.Width = _channel_img.Height = h_w;
            _channel_highlight.Width = _volume_highlight.Width = _channel_highlight.Height = _volume_highlight.Height = h_w;

            _channel_highlight.RadiusX = _channel_highlight.RadiusY = h_w / 2;
            _volume_highlight.RadiusX = _volume_highlight.RadiusY = h_w / 2;
            load_devices();

            // setting the different properties of the back button.
            _backButton.Click += new RoutedEventHandler(backClick);
            Canvas.SetLeft(_backButton, 75);
            Canvas.SetTop(_backButton, 50);

            // customization of _volume
            _volume_bg.Width = Window.Current.Bounds.Width - offset;
            Canvas.SetLeft(_volume_bg, -(Window.Current.Bounds.Width/2-offset));
            Canvas.SetTop(_volume_bg, 0);
            _volume_color.Width = Window.Current.Bounds.Width;
            _volume_color.Height = Window.Current.Bounds.Height;

            Canvas.SetLeft(_volume_img, 0.75 * _volume_bg.Width - _volume_img.Width / 2); 
            Canvas.SetTop(_volume_img, (Window.Current.Bounds.Height - _volume_img.Height) / 2);
            Canvas.SetLeft(_volume_highlight, 0.75 * _volume_bg.Width - _volume_highlight.Width / 2);
            Canvas.SetTop(_volume_highlight, (Window.Current.Bounds.Height - _volume_highlight.Height) / 2);

            // customization of _channel
            _channel_bg.Width = Window.Current.Bounds.Width - offset;
            Canvas.SetLeft(_channel_bg, Window.Current.Bounds.Width / 2);
            Canvas.SetTop(_channel_bg, 0);
            _channel_color.Width = Window.Current.Bounds.Width;
            _channel_color.Height = Window.Current.Bounds.Height;
            
            Canvas.SetLeft(_channel_img, 0.25 * _channel_bg.Width - _channel_img.Width / 2); 
            Canvas.SetTop(_channel_img, (Window.Current.Bounds.Height - _channel_img.Height) / 2);
            Canvas.SetLeft(_channel_highlight, 0.25 * _channel_bg.Width - _channel_highlight.Width / 2);
            Canvas.SetTop(_channel_highlight, (Window.Current.Bounds.Height - _channel_highlight.Height) / 2);

            // customization of _divider
            Canvas.SetLeft(_divider, (Window.Current.Bounds.Width - _divider.Width) / 2);
            Canvas.SetTop(_divider, 0);

        } // constructor

        // ============================================================================================================================================
        // ============================================================================================================================================



        async void load_devices()
        {

            DeviceManager device_manager = ((App)(CPRemoteApp.App.Current)).deviceController;
            /*StorageFolder local_folder = App.appData.LocalFolder;
            StorageFolder devices_folder = await local_folder.CreateFolderAsync("devices_folder", CreationCollisionOption.OpenIfExists);

            //TODO: Should check the return value of device_manager to ensure devices were loaded properly
            await device_manager.initialize(devices_folder);*/
            
            int offset = 200;

            // Button Scanner Panel Formatting
            channel_scanner_panel.Height = Window.Current.Bounds.Height;
            channel_scanner_panel.Width = Window.Current.Bounds.Width - offset;
            volume_scanner_panel.Height = Window.Current.Bounds.Height;
            volume_scanner_panel.Width = Window.Current.Bounds.Width - offset;
            
            

            channel_scanner_panel.Children.Add(device_manager.channelController.buttonScanner);
            device_manager.channelController.buttonScanner.Width = volume_scanner_panel.Width;
            device_manager.channelController.buttonScanner.Height = volume_scanner_panel.Height;
            double image_dimension = 4 * channel_scanner_panel.Height / 5;
            device_manager.channelController.buttonScanner.setCurrentImage(image_dimension);
            device_manager.volumeController.buttonScanner.Width = volume_scanner_panel.Width;
            device_manager.volumeController.buttonScanner.Height = volume_scanner_panel.Height;
            device_manager.volumeController.buttonScanner.setCurrentImage(image_dimension);
            device_manager.volumeController.buttonScanner.PointerReleased += onVolumeButtonClicked;
            volume_scanner_panel.Children.Add(device_manager.volumeController.buttonScanner);
        }







        private void checkBluetoothStatus(object sender, object e)
        {
          bool connected = true;

          Color color; 
          if (connected)
          {
            color = Colors.GreenYellow;
            color.A = 160;
          }
          else
          {
            color = Colors.DarkRed;
            color.A = 160;
          }

          _bluetooth_status_indicator.Fill = new SolidColorBrush(color);

        }





        // ============================================================================================================================================
        // animation functions
        private void beginAnimationSequence(bool dir)
        {
            can_move = false;
            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationManager = new DoubleAnimation();
            _volume_highlight.Visibility = _channel_highlight.Visibility = Visibility.Collapsed;

            if (status != 0)
            {
                double animation_time = 0.2;

                _volume_highlight.Visibility = _channel_highlight.Visibility = Visibility.Collapsed;

                // show/hide channel button
                Storyboard.SetTarget(animationManager, _channel_img);
                Storyboard.SetTargetProperty(animationManager, "Opacity");
                animationManager.Duration = new Duration(TimeSpan.FromSeconds(animation_time / 1.5));
                animationManager.To = 0;
                storyboard.Children.Add(animationManager);

                // show/hide volume button
                animationManager = new DoubleAnimation();
                Storyboard.SetTarget(animationManager, _volume_img);
                Storyboard.SetTargetProperty(animationManager, "Opacity");
                animationManager.Duration = new Duration(TimeSpan.FromSeconds(animation_time / 1.5));
                animationManager.To = 0;
                storyboard.Children.Add(animationManager);
            }
            storyboard.Completed += delegate
            {
                slide(dir);
            };

            storyboard.Begin();

        }

        private void slide(bool dir){
            double animation_time = 0.2;
            int offset = 200;

            // animates the divider over.
            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _divider);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration( TimeSpan.FromSeconds( animation_time ) );
            if (dir) animationManager.To = Canvas.GetLeft(_divider) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_divider) - (Window.Current.Bounds.Width / 2 - offset);
            storyboard.Children.Add(animationManager);

            // animates the volume section over
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _volume_bg);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration( TimeSpan.FromSeconds( animation_time ) );
            if (dir) animationManager.To = Canvas.GetLeft(_volume_bg) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_volume_bg) - (Window.Current.Bounds.Width / 2 - offset);

            storyboard.Children.Add(animationManager);

            // animates the channel list section over.
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _channel_bg);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration( TimeSpan.FromSeconds( animation_time ) );
            if (dir) animationManager.To = Canvas.GetLeft(_channel_bg) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_channel_bg) - (Window.Current.Bounds.Width / 2 - offset); 
            storyboard.Children.Add(animationManager);


            // end story handler
            if (status == 0)
            {
                storyboard.Completed += delegate
                {
                  finishAnimationSequence();
                };
            }
            else if (status == -1)
            {
              volume_scanner_panel.Visibility = Visibility.Visible;
                ((App)(CPRemoteApp.App.Current)).deviceController.volumeController.buttonScanner.start();
            }
            else if (status == 1)
            {
              channel_scanner_panel.Visibility = Visibility.Visible;
                ((App)(CPRemoteApp.App.Current)).deviceController.channelController.buttonScanner.start();
            }

            storyboard.Completed += delegate {
              can_move = true;
            };
            System.Diagnostics.Debug.WriteLine("Before");

            storyboard.Begin();
            System.Diagnostics.Debug.WriteLine("After");


        }

        private void finishAnimationSequence()
        {

            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationManager = new DoubleAnimation();
            double animation_time = 0.2;

            // show/hide channel button
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _channel_img);
            Storyboard.SetTargetProperty(animationManager, "Opacity");
            animationManager.Duration = new Duration(TimeSpan.FromSeconds(animation_time / 1.5));
            animationManager.To = 100;
            storyboard.Children.Add(animationManager);

            // show/hide volume button
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _volume_img);
            Storyboard.SetTargetProperty(animationManager, "Opacity");
            animationManager.Duration = new Duration(TimeSpan.FromSeconds(animation_time / 1.5));
            animationManager.To = 100;
            storyboard.Children.Add(animationManager);

            storyboard.Begin();

        }
        // ============================================================================================================================================
        // ============================================================================================================================================









        // ============================================================================================================================================
        // event handlers for button clicks
        private void backClick(object sender, RoutedEventArgs e) { 
            this.Frame.GoBack();
            DeviceManager device_manager = ((App)(CPRemoteApp.App.Current)).deviceController;

            channel_scanner_panel.Children.Remove(device_manager.channelController.buttonScanner);
            volume_scanner_panel.Children.Remove(device_manager.volumeController.buttonScanner);


        }

        private void _volume_Click(object sender, RoutedEventArgs e)
        {
            if ((status == 0 || status == 1) && can_move)
                {
                    if(status == 1)
                    {
                      channel_scanner_panel.Visibility = Visibility.Collapsed;
                        ((App)(CPRemoteApp.App.Current)).deviceController.channelController.buttonScanner.stop();
                    }
                    status--;
                    beginAnimationSequence(true);
                }
        }

        private void _channelList_Click(object sender, RoutedEventArgs e)
        {
            if ((status == 0 || status == -1) && can_move)
                {
                    if (status == -1)
                    {
                      volume_scanner_panel.Visibility = Visibility.Collapsed;
                        ((App)(CPRemoteApp.App.Current)).deviceController.volumeController.buttonScanner.stop();
                    }
                    status++;
                    beginAnimationSequence(false);
                }
        }

        private void onVolumeButtonClicked(object sender, RoutedEventArgs e)
        {
            string to_send = "-S.";
            to_send += ((App)CPRemoteApp.App.Current).deviceController.volumeController.IR_protocol;
            to_send += ".";
            RemoteButton cur_button = ((App)CPRemoteApp.App.Current).deviceController.volumeController.buttonScanner.getCurrentButton();
            if (cur_button.abbreviation[0] == '-')
            {
                to_send += ((App)CPRemoteApp.App.Current).deviceController.volumeController.volume_down_ir_code;
            }
            else
            {
                to_send += ((App)CPRemoteApp.App.Current).deviceController.volumeController.volume_up_ir_code;
            }
            to_send += ".";
            to_send += ((App)CPRemoteApp.App.Current).deviceController.volumeController.IR_bits;
            to_send += ".";
            int change_increment = ((App)CPRemoteApp.App.Current).deviceController.volumeController.volume_increments * cur_button.getRepitions();
            to_send += change_increment.ToString();
            to_send += "/";
            App.bm.OperateTVButton_Click(to_send);
            System.Diagnostics.Debug.WriteLine(to_send);
        }

        private void pointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender.Equals(_volume_bg) && status == 0) _volume_highlight.Visibility = Visibility.Visible;
            else if (sender.Equals(_channel_bg) && status == 0) _channel_highlight.Visibility = Visibility.Visible;
        }
        private void pointerExited(object sender, PointerRoutedEventArgs e)
        {

            if (sender.Equals(_volume_bg)) _volume_highlight.Visibility = Visibility.Collapsed;
            else if (sender.Equals(_channel_bg)) _channel_highlight.Visibility = Visibility.Collapsed;

        }
        // ============================================================================================================================================
        // ============================================================================================================================================

    }
}
