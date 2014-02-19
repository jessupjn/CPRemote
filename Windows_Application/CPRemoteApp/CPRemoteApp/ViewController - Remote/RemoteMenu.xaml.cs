using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CPRemoteApp.ViewController___Remote
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoteMenu : Page
    {
        // customizable variables
        // -------------------------
        private int offset = 200;
        private double animation_time = 0.2;

        // -------------------------

        private int status = 0;
        private bool can_move = true;


        // ============================================================================================================================================
        // Constructor
        public RemoteMenu()
        {
            this.InitializeComponent();

            double h_w = Window.Current.Bounds.Height / 2.5;
            _volume_img.Width = h_w;
            _volume_img.Height = h_w;
            _channel_img.Width = h_w;
            _channel_img.Height = h_w;

            // setting the different properties of the back button.
            _backButton.Click += new RoutedEventHandler(backClick);
            Canvas.SetLeft(_backButton, 75);
            Canvas.SetTop(_backButton, 50);

            // customization of _volume
            _volume.PointerReleased += _volume_Click;
            _volume.Width = Window.Current.Bounds.Width;
            _volume.Height = Window.Current.Bounds.Height;
            Canvas.SetLeft(_volume_bg, -Window.Current.Bounds.Width/2);
            Canvas.SetTop(_volume_bg, 0);
            Canvas.SetLeft(_volume_img, (0.75 * Window.Current.Bounds.Width) - (0.5 * _volume_img.Width));
            Canvas.SetTop(_volume_img, (Window.Current.Bounds.Height - _volume_img.Height) / 2);
            System.Diagnostics.Debug.WriteLine(_volume.Width);


            // customization of _channelList
            _channelList.PointerReleased += _channelList_Click;
            Canvas.SetLeft(_channelList, Window.Current.Bounds.Width / 2);
            Canvas.SetTop(_channelList, 0);

            // customization of _divider
            Canvas.SetLeft(_divider, (Window.Current.Bounds.Width - _divider.Width) / 2);
            Canvas.SetTop(_divider, 0);

            System.Diagnostics.Debug.WriteLine("DEBUGGER BEGINS");

        } // constructor

        // ============================================================================================================================================
        // ============================================================================================================================================







        // ============================================================================================================================================
        // animation function for _divider
        private void animate(bool dir)
        {

            Storyboard storyboard = new Storyboard();

            // animates the divider over.
            DoubleAnimation animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _divider);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration( TimeSpan.FromSeconds( animation_time ) );
            if (dir) animationManager.To = Canvas.GetLeft(_divider) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_divider) - (Window.Current.Bounds.Width / 2 - offset);
            storyboard.Children.Add(animationManager);

            // animates the volume section over
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _volume);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration( TimeSpan.FromSeconds( animation_time ) );
            if (dir) animationManager.To = Canvas.GetLeft(_volume) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_volume) - (Window.Current.Bounds.Width / 2 - offset);

            storyboard.Children.Add(animationManager);

            // animates the channel list section over.
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _channelList);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration( TimeSpan.FromSeconds( animation_time ) );
            if (dir) animationManager.To = Canvas.GetLeft(_channelList) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_channelList) - (Window.Current.Bounds.Width / 2 - offset); 
            storyboard.Children.Add(animationManager);


            // show/hide channel button
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _channel_img);
            Storyboard.SetTargetProperty(animationManager, "Opacity");
            animationManager.Duration = new Duration( TimeSpan.FromSeconds( animation_time / 2 ) );
            if (status != 0) animationManager.To = 0;
            else animationManager.To = 100;
            storyboard.Children.Add(animationManager);

            // show/hide volume button
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _volume_img);
            Storyboard.SetTargetProperty(animationManager, "Opacity");
            animationManager.Duration = new Duration(TimeSpan.FromSeconds(animation_time / 2));
            if (status != 0) animationManager.To = 0;
            else animationManager.To = 100;
            storyboard.Children.Add(animationManager);

            // end story handler
            storyboard.Completed += delegate { 
                buildButtonList(dir, offset + _divider.Width);
            };

            storyboard.Begin();

        }
        // ============================================================================================================================================
        // ============================================================================================================================================




        // ============================================================================================================================================
        // builds and displays the clickable list.
        private void buildButtonList(bool dir, double offset)
        {
            can_move = true;
            if (status == 0) return;

        }
        // ============================================================================================================================================
        // ============================================================================================================================================









        // ============================================================================================================================================
        // event handlers for button clicks
        private void backClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void _volume_Click(object sender, RoutedEventArgs e)
        {
            if ((status == 0 || status == 1) && can_move)
                {
                    can_move = false;
                    status--;
                    animate(true);
                }
        }

        private void _channelList_Click(object sender, RoutedEventArgs e)
        {
            if ((status == 0 || status == -1) && can_move)
                {
                    can_move = false;
                    status++;
                    animate(false);
                }
        }
        // ============================================================================================================================================
        // ============================================================================================================================================

    }
}
