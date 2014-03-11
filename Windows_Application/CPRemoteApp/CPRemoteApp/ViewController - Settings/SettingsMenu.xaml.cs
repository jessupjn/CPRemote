using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
            Canvas.SetLeft(_bt_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_volume_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channel_device, (Window.Current.Bounds.Width - 700) / 2);

             
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void bluetoothClick(object sender, RoutedEventArgs e)
        {
          App.bm.ConnectButton_Click(sender, e);
        }

        private void channelClick(object sender, RoutedEventArgs e)
        {
          enumerateListAsync((sender as Canvas), false);
        }

        private void volumeClick(object sender, RoutedEventArgs e)
        {
          enumerateListAsync((sender as Canvas), true);
        }




        private async void enumerateListAsync(object sender, bool channel_or_volume) { await buildList((sender as Canvas), channel_or_volume); }
        private async Task buildList(Canvas sender, bool channel_or_volume)
        {
          PopupMenu menu = new PopupMenu();

          var result = await menu.ShowForSelectionAsync( GetElementRect(sender) );
          if (result == null)
          {
            menu.Commands.Add(new UICommand("No device found."));
            result = await menu.ShowForSelectionAsync( GetElementRect(sender) );
          }

        }


        private Rect GetElementRect(FrameworkElement element)
        {
          GeneralTransform buttonTransform = element.TransformToVisual(null);
          Point point = buttonTransform.TransformPoint(new Point());
          return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
    }
}
