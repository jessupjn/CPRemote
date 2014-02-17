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
            _goToRemote.Width = 0.7 * Window.Current.Bounds.Height;
            _goToRemote.Height = _goToRemote.Width;
            _goToRemote.Click += new RoutedEventHandler(remoteClick);
            Canvas.SetLeft(_goToRemote, (Window.Current.Bounds.Width -_goToRemote.Width) / 2 );
            Canvas.SetTop(_goToRemote, 0.15 * Window.Current.Bounds.Height);

        
            // customization of _goToSettings
            _goToSettings.Click += new RoutedEventHandler(settingsClick);
            Canvas.SetLeft(_goToSettings, Window.Current.Bounds.Width - 100);
            Canvas.SetTop(_goToSettings, 25 );


            // customization of titleText





        }

        private void remoteClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate( typeof(ViewController___Remote.RemoteMenu) );
        }

        private void settingsClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate( typeof(ViewController___Settings.SettingsMenu) );
        }
    }
}
