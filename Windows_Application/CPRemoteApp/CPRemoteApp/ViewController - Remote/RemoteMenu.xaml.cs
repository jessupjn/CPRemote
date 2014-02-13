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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CPRemoteApp.ViewController___Remote
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoteMenu : Page
    {
        private Point clickOrigin;
        private int status = 0;
        bool clickedDown = false;

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
            _divider.Width = 20;
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
            _swipeDetector.PointerPressed += new PointerEventHandler(swipe_click_down);
            _swipeDetector.PointerMoved += new PointerEventHandler(swipe_click_up);

            System.Diagnostics.Debug.WriteLine("DEBUGGER BEGINS");

        }

        private void swipe_click_down(object sender, PointerRoutedEventArgs e)
        {
            clickOrigin = e.GetCurrentPoint( _swipeDetector ).Position;
            clickedDown = true;
        }

        private void swipe_click_up(object sender, PointerRoutedEventArgs e)
        {
            Point newPoint = e.GetCurrentPoint(_swipeDetector).Position;
            if (clickedDown && Math.Abs(newPoint.X - clickOrigin.X) > 0.08 * _swipeDetector.Width)
            {
                clickedDown = false;
                if ((newPoint.X - clickOrigin.X) > 0)
                {

                    System.Diagnostics.Debug.WriteLine("Swipe Right");
                    if (status == 0 || status == 1)
                    {
                        animate(true);
                        status -= 1;
                    }

                }
                else if ((newPoint.X - clickOrigin.X) < 0)
                {
                    System.Diagnostics.Debug.WriteLine("Swipe Left");
                    if (status == 0 || status == -1)
                    {
                        animate(false);
                        status += 1;
                    }
                }
            }
        }

        private void animate(bool dir)
        {
            Storyboard storyboard = new Storyboard();
            int offset = 130;

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

            storyboard.Begin();


        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }


    
    }
}
