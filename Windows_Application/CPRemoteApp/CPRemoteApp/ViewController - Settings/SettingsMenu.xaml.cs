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

namespace CPRemoteApp.ViewController___Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsMenu : Page
    {
        public SettingsMenu()
        {
            this.InitializeComponent();

            _scroller.Width = _scroll_canvas.Width = Window.Current.Bounds.Width;
            _scroller.Height = Window.Current.Bounds.Height - 100;

            Canvas.SetLeft(title_box, (Window.Current.Bounds.Width - title_box.Width) / 2);
//            Canvas.SetLeft(_bt_device, (Window.Current.Bounds.Width - _bt_device.Width) / 2);
//            Canvas.SetLeft(_volume_device, (Window.Current.Bounds.Width - _bt_device.Width) / 2);
//            Canvas.SetLeft(_channel_device, (Window.Current.Bounds.Width - _bt_device.Width) / 2);

             
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void bluetoothClick(object sender, RoutedEventArgs e)
        {
          App.bm.ConnectButton_Click(sender, e);
        }
    }
}
