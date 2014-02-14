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
        private int offset = 130;
        

        // -------------------------

        private Point clickOrigin;
        private int status = 0;
        private bool clickedDown = false;
        private Border buttonList;
        private bool canMove = true;
        private Image vol;
        private Image channel;


        // ============================================================================================================================================
        // Constructor
        public RemoteMenu()
        {
            this.InitializeComponent();

            // setting the different properties of the back button.
            _backButton.Click += new RoutedEventHandler(backClick);
            Canvas.SetLeft(_backButton, 75);
            Canvas.SetTop(_backButton, 50);

            // customization of _volume
            _volume.Height = Window.Current.Bounds.Height;
            _volume.Width = Window.Current.Bounds.Width;
            _volume.Fill = new SolidColorBrush(Windows.UI.Colors.Coral);
            Canvas.SetLeft(_volume, -Window.Current.Bounds.Width/2);
            Canvas.SetTop(_volume, 0);

            // customization of _channelList
            _channelList.Height = Window.Current.Bounds.Height;
            _channelList.Width = Window.Current.Bounds.Width;
            _channelList.Fill = new SolidColorBrush(Windows.UI.Colors.PaleGreen);
            Canvas.SetLeft(_channelList, Window.Current.Bounds.Width / 2);
            Canvas.SetTop(_channelList, 0);

            // customization of _divider
            _divider.Height = Window.Current.Bounds.Height;
            _divider.Width = 15;
            _divider.Fill = new SolidColorBrush(Windows.UI.Colors.White);
            Canvas.SetLeft(_divider, (Window.Current.Bounds.Width - _divider.Width) / 2);
            Canvas.SetTop(_divider, 0);

            // set up gestures;
            _swipeDetector.Width = Window.Current.Bounds.Width;
            _swipeDetector.Height = Window.Current.Bounds.Height;
            _swipeDetector.Fill = new SolidColorBrush(Windows.UI.Colors.White);
            _swipeDetector.Fill.Opacity = 0;

            Canvas.SetLeft(_swipeDetector,0);
            Canvas.SetTop(_swipeDetector, 0);
            _swipeDetector.PointerPressed += new PointerEventHandler(mouse_click_down);
            _swipeDetector.PointerReleased += new PointerEventHandler(mouse_check_swipe);
            _swipeDetector.PointerMoved += new PointerEventHandler(mouse_click_up);


            vol = new Image()
            {
                Width = 300,
                Height = 300,
                Source = new BitmapImage(new Uri(@"ms-appx://volume_symbol.png", UriKind.Absolute))
            };
            _bg.Children.Add(vol);
            Canvas.SetLeft(vol, (_volume.Width - vol.Width) / 2);
            Canvas.SetTop(vol, (Window.Current.Bounds.Height - vol.Height) / 2);

            channel = new Image()
            {
                Width = 300,
                Height = 300,
                //Source = new ImageSource("ms-appx:///img/volume_symbol.png");
            };
            _bg.Children.Add(channel);
            Canvas.SetLeft(channel, (Canvas.GetLeft(_channelList) + _channelList.Width - channel.Width) / 2);
            Canvas.SetTop(channel, (Window.Current.Bounds.Height - channel.Height) / 2);

            System.Diagnostics.Debug.WriteLine("DEBUGGER BEGINS");

        } // constructor

        // ============================================================================================================================================
        // ============================================================================================================================================







        // ============================================================================================================================================
        // Various touch screeen handlers that are used in this menu.

        // COPIES LOCATION OF THE MOUSE WHEN IT HAS BEEN CLICKED
        private void mouse_click_down(object sender, PointerRoutedEventArgs e)
        {
            clickOrigin = e.GetCurrentPoint( _swipeDetector ).Position;
            clickedDown = true;
        } // mouse_click_down



        // HANDLES THE SWIPE FEATURE ADDED TO OUR UI
        private void mouse_check_swipe(object sender, PointerRoutedEventArgs e)
        {
            Point newPoint = e.GetCurrentPoint(_swipeDetector).Position;
            if (clickedDown && Math.Abs(newPoint.X - clickOrigin.X) > 0.08 * _swipeDetector.Width && canMove)
            {
                clickedDown = false;


                // moving back towards the center;
                if (status != 0 && buttonList != null)
                {
                    _bg.Children.Remove(buttonList);
                    vol.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    channel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    vol.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    channel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }


                if ((newPoint.X - clickOrigin.X) > 0 && (status == 0 || status == 1))
                {
                    status -= 1;
                    animate(true);
                }
                else if ((newPoint.X - clickOrigin.X) < 0 && (status == 0 || status == -1))
                {
                    status += 1;
                    animate(false);
                }

            }
        } // mouse_check_swipe
        


        // WHEN A CLICK HAS BEEN MADE AND A COMMAND NEEDS TO BE SENT
        private void mouse_click_up(object sender, PointerRoutedEventArgs e)
        {
            Point newPoint = e.GetCurrentPoint(_swipeDetector).Position;
            if(status != 0 &&  Math.Abs(newPoint.X - clickOrigin.X) < 0.04 * _swipeDetector.Width)
            {
                System.Diagnostics.Debug.WriteLine("-- a click has been made and needs to be handled.");
            }
        } // mouse_click_up

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
            animationManager.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            if (dir) animationManager.To = Canvas.GetLeft(_divider) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_divider) - (Window.Current.Bounds.Width / 2 - offset);
            storyboard.Children.Add(animationManager);

            // animates the volume section over
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _volume);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            if (dir) animationManager.To = Canvas.GetLeft(_volume) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_volume) - (Window.Current.Bounds.Width / 2 - offset);

            storyboard.Children.Add(animationManager);

            // animates the channel list section over.
            animationManager = new DoubleAnimation();
            Storyboard.SetTarget(animationManager, _channelList);
            Storyboard.SetTargetProperty(animationManager, "(Canvas.Left)");
            animationManager.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            if (dir) animationManager.To = Canvas.GetLeft(_channelList) + (Window.Current.Bounds.Width / 2 - offset);
            else animationManager.To = Canvas.GetLeft(_channelList) - (Window.Current.Bounds.Width / 2 - offset); 
            storyboard.Children.Add(animationManager);

            // end story handler
            storyboard.Completed += delegate { buildButtonList(dir, offset + _divider.Width); };
            canMove = false;

            storyboard.Begin();

        }
        // ============================================================================================================================================
        // ============================================================================================================================================




        // ============================================================================================================================================
        // builds and displays the clickable list.
        private void buildButtonList(bool dir, double offset)
        {
            canMove = true;
            if (status == 0) return;

            System.Diagnostics.Debug.WriteLine("Created");

            buttonList = new Border();
            buttonList.Width = Window.Current.Bounds.Width - offset;
            buttonList.Height = Window.Current.Bounds.Height;
            buttonList.Background = new SolidColorBrush(Windows.UI.Colors.Purple);
            buttonList.Name = "scrollList";

            if (dir) Canvas.SetLeft(buttonList, 0);
            else Canvas.SetLeft(buttonList, Canvas.GetLeft(_divider) + _divider.Width);
            Canvas.SetTop(buttonList, 0);
            _bg.Children.Add(buttonList);
            Canvas.SetZIndex(buttonList, 10);

        }
        // ============================================================================================================================================
        // ============================================================================================================================================









        // ============================================================================================================================================
        // back button has been pressed.
        private void backClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
        // ============================================================================================================================================
        // ============================================================================================================================================

    }
}
