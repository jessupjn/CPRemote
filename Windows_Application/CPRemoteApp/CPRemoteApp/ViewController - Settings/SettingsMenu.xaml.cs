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
        private DispatcherTimer timer = new DispatcherTimer();

        public SettingsMenu()
        {
            this.InitializeComponent();

            _scroller.Width = _scroll_canvas.Width = Window.Current.Bounds.Width;
            _scroller.Height = Window.Current.Bounds.Height - 100;

            Canvas.SetLeft(title_box, (Window.Current.Bounds.Width - title_box.Width) / 2);
            Canvas.SetLeft(_bt_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_volume_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channel_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channellist_label, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channellist_listbox, (Window.Current.Bounds.Width - 700) / 2);

            populateChannelList();

            //timer.Interval = TimeSpan.FromSeconds(0.2);
            //timer.Tick += populateChannelList;
            //timer.Start();
             
        }

        private void populateChannelList()
        {
          _channellist_listbox.Height = 40 * 4; // TODO: fill with number of channels.

          ListBoxItem item;
          ChannelList content;

          // TODO: for each in channellist... populate with items....
          for(int i = 0; i < 4; i++)
          {
            item = new ListBoxItem();
            content = new ChannelList();

            item.Content = content;
            _channellist_listbox.Items.Add(item);
          }
          item = new ListBoxItem();
          content = new ChannelList("Add a new channel...");

          item.Content = content;
          _channellist_listbox.Items.Add(item);

        }
        // ===================================================================================================
        // ===================================================================================================





        // ===================================================================================================
        // Click handlers for different objects on the screen.

        private void backClick(object sender, RoutedEventArgs e)
        { this.Frame.GoBack(); }

        private void bluetoothClick(object sender, RoutedEventArgs e)
        {  App.bm.ConnectButton_Click(sender, e); }

        private void channelClick(object sender, RoutedEventArgs e)
        { enumerateListAsync((sender as Canvas), false); }

        private void volumeClick(object sender, RoutedEventArgs e)
        { enumerateListAsync((sender as Canvas), true); }

        // ===================================================================================================
        // ===================================================================================================




        private async void enumerateListAsync(object sender, bool channel_or_volume) { await buildList( GetElementRect(sender as Canvas), channel_or_volume); }
        private async Task buildList(Rect invokerRect, bool channel_or_volume)
        {
          PopupMenu menu = new PopupMenu();

          //
          // GET DEVICES FROM SOURCE BASE ON channel_or_volume
          // true = volume devices
          // false = channel devices
          // if there is an iteam in the list, also add "Add new device..." at bottom of the list.

          var result = await menu.ShowForSelectionAsync(invokerRect);
          if (result == null)
          {
            menu.Commands.Add(new UICommand("Add new device...", new UICommandInvokedHandler(addNewDevice)));
            result = await menu.ShowForSelectionAsync(invokerRect);
          }

        }

        private async void addNewDevice(IUICommand command)
        {
          return;
        }
        private async void selectListItem(IUICommand command)
        {
          return;
        }


        private Rect GetElementRect(FrameworkElement element)
        {
          GeneralTransform buttonTransform = element.TransformToVisual(null);
          Point point = buttonTransform.TransformPoint(new Point());
          return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
    }
}
