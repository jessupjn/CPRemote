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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CPRemoteApp
{
    /// <summary>
    /// The main menu and opening page for CPRemote.
    /// </summary>
    public sealed partial class Menu : Page
    {
        public Menu()
        {
            this.InitializeComponent();
            
            // customization of _goToRemote
            _goToRemote_frame.Width = 0.7 * Window.Current.Bounds.Height;
            _goToRemote_frame.Height = _goToRemote.Width = _goToRemote.Height = _goToRemote_indicator.Width = _goToRemote_indicator.Height = _goToRemote_frame.Width;
            _goToRemote_indicator.RadiusX = _goToRemote_indicator.RadiusY = _goToRemote_indicator.Width / 2;
            Canvas.SetLeft(_goToRemote_frame, (Window.Current.Bounds.Width - _goToRemote_frame.Width) / 2);
            Canvas.SetTop(_goToRemote_frame, 0.15 * Window.Current.Bounds.Height);

        
            // customization of _goToSettings
            Canvas.SetLeft(_goToSettings_frame, Window.Current.Bounds.Width - 100);
            Canvas.SetTop(_goToSettings_frame, 102 );


            // customization of _bluetooth_status_frame
            Canvas.SetLeft(_bluetooth_status_frame, Window.Current.Bounds.Width - 100);
            Canvas.SetTop(_bluetooth_status_frame, 25);





        }

        private void remoteClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate( typeof(ViewController___Remote.RemoteMenu) );
        }

        private void settingsClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate( typeof(ViewController___Settings.SettingsMenu) );
        }

        private void enterHighlight(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Color fill = Windows.UI.Colors.Black;
            if( sender.Equals(_goToRemote) ) _goToRemote_indicator.Fill = new SolidColorBrush(fill);
            else if (sender.Equals(_goToSettings)) _goToSettings_indicator.Fill = new SolidColorBrush(fill);

        }

        private void exitHighlight(object sender, PointerRoutedEventArgs e)
        {
            if (sender.Equals(_goToRemote)) _goToRemote_indicator.Fill = null;
            else if (sender.Equals(_goToSettings)) _goToSettings_indicator.Fill = null;
        }
    }
}
