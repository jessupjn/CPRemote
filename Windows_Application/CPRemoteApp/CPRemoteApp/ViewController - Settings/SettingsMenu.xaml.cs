using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CPRemoteApp.Utility_Classes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CPRemoteApp.ViewController___Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class SettingsMenu : Page
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private List<ListBoxItem> channels = new List<ListBoxItem>();


        public SettingsMenu()
        {
            this.InitializeComponent();

            _scroll_canvas.Width = Window.Current.Bounds.Width;
            _scroll_canvas.Height = Window.Current.Bounds.Height - 175;

            Canvas.SetLeft(title_box, (Window.Current.Bounds.Width - title_box.Width) / 2);
            Canvas.SetLeft(_bt_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_volume_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channel_device, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channellist_label, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channellist_listbox, (Window.Current.Bounds.Width - 700) / 2);
            Canvas.SetLeft(_channellist_border, (Window.Current.Bounds.Width - 700) / 2);

            _channellist_listbox.ItemsSource = channels;
            populateChannelList();

            //timer.Interval = TimeSpan.FromSeconds(0.2);
            //timer.Tick += populateChannelList;
            //timer.Start();
             
        }

        private void populateChannelList()
        {
          int num = 4; // number of channels.
          if (85 * (num + 1) > 600)
          {
            _channellist_listbox.Height = 600;
            _channellist_border.Height = 610;
          }
          else
          {
            _channellist_listbox.Height = 85 * (num + 1);
            _channellist_border.Height = 85 * (num + 1) + 10;
          }
          _channellist_listbox.Background = _channellist_listbox.Foreground = new SolidColorBrush(Colors.Transparent);

          ListBoxItem item;
          ChannelList content;
          // TODO: for each in channellist... populate with items....
          for(int i = 0; i < num; i++)
          {
            item = new ListBoxItem();
            content = new ChannelList("channel " + i.ToString(), channels.Count);
            item.Content = content;
            channels.Add(item);
          }
          item = new ListBoxItem();
          content = new ChannelList("Add New Channel", channels.Count);
          item.Content = content;
          channels.Add(item);

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

      private void clickerIn(object sender, RoutedEventArgs e)
      {
        Canvas source = sender as Canvas;
        SolidColorBrush color = new SolidColorBrush(Color.FromArgb(255, 29, 33, 99));
        switch (source.Name)
        {
          case "_bt_device":
            _bt_device_frame.Fill = color; 
            break;
          case "_volume_device":
            _volume_device_frame.Fill = color;  
            break;
          case "_channel_device":
            _channel_device_frame.Fill = color; 
            break;
        }
      }
      private void clickerOut(object sender, RoutedEventArgs e)
      {
        Canvas source = sender as Canvas;
        SolidColorBrush color = new SolidColorBrush(Colors.Transparent);
        switch (source.Name)
        {
          case "_bt_device":
            _bt_device_frame.Fill = color;
            break;
          case "_volume_device":
            _volume_device_frame.Fill = color;
            break;
          case "_channel_device":
            _channel_device_frame.Fill = color;
            break;
        }
      }


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
          if (result == null && channel_or_volume)
          {
            menu.Commands.Add(new UICommand("Add new volume device...", new UICommandInvokedHandler(addNewVolumeDevice)));
            result = await menu.ShowForSelectionAsync(invokerRect);
          }
          else if(result == null)
          {
              menu.Commands.Add(new UICommand("Add new channel device...", new UICommandInvokedHandler(addNewChannelDevice)));
              result = await menu.ShowForSelectionAsync(invokerRect);
          }

        }

        private async void addNewChannelDevice(IUICommand command)
        {
          return;
        }

        private async void addNewVolumeDevice(IUICommand command)
        {
            //StackPanel popup_content = new StackPanel();
            /*TextBox name_field = new TextBox();
            name_field.Text = "Please Enter Device Name";
            popup_content.Children.Add(name_field);
            Button next_button = new Button();
            TextBlock next_txt = new TextBlock();
            next_txt.Text = "Next";
            next_button.Content = next_txt;
            next_button.Click += () =>
                { };
            popup_content.Children.Add(next_button);*/
            AddDevicePopup popup_content = new AddDevicePopup();
            popup_content.setDevice(new VolumeDevice(), true);
            Border border = new Border
            {
                Child = popup_content,
                Background = new SolidColorBrush(Colors.LightBlue),
                BorderBrush = new SolidColorBrush(Colors.Red),
                BorderThickness = new Thickness(4),
                Padding = new Thickness(24),
            };

            //border.Background.Opacity = 0.5;
            Popup add_device_popup = new Popup
            {
                Child = border,
                IsLightDismissEnabled = true
            };

            border.Loaded += (loadedSender, loadedArgs) =>
                {
                    add_device_popup.HorizontalOffset = Window.Current.Bounds.Width / 2;
                    add_device_popup.VerticalOffset = Window.Current.Bounds.Height / 3;
                };

            add_device_popup.IsOpen = true;
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
