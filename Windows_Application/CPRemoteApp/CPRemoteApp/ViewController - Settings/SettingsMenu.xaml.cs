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
using CPRemoteApp.ViewController___Remote;

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
        private Popup add_device_popup = new Popup();
        private Popup add_channel_popup = new Popup();

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
             
        }

        private void populateChannelList()
        {
          if(_channellist_listbox.Items.Count > 0) _channellist_listbox.Items.Clear();

          List<RemoteButton> blist = ((App)(CPRemoteApp.App.Current)).deviceController.channelController.buttonScanner.getButtons();
          int num = blist.Count; // number of channels.
          if (85 * (num + 1) > 450)
          {
            _channellist_listbox.Height = 450;
            _channellist_border.Height = 460;
          }
          else
          {
            _channellist_listbox.Height = 85 * (num + 1);
            _channellist_border.Height = 85 * (num + 1) + 10;
          }
          _channellist_listbox.Background = _channellist_listbox.Foreground = new SolidColorBrush(Colors.Transparent);

          ListBoxItem item;
          ChannelList content;
          for(int i = 0; i < num; i++)
          {
            item = new ListBoxItem();
            content = new ChannelList(blist[i].getName(), i);
            item.Content = content;
            channels.Add(item);
          }
          item = new ListBoxItem();
          content = new ChannelList("Add New Channel", -1);
          item.Content = content;
          item.PointerMoved += delegate
          {
            Debug.WriteLine("ADD OBJECT");
            AddNewChannelPopup popup_content = new AddNewChannelPopup();
            Border border = new Border
            {
              Child = popup_content,
              Width = 500,
              Height = 470,
              Background = new SolidColorBrush(Color.FromArgb(255, 18, 11, 66)),
              BorderBrush = new SolidColorBrush(Colors.Black),
              BorderThickness = new Thickness(4),
              Padding = new Thickness(50,20,50,20)
            };

            add_channel_popup = new Popup
            {
              Child = border,
              IsLightDismissEnabled = true
            };

            add_channel_popup.Closed += add_channel_popup_Closed;

            border.Loaded += (loadedSender, loadedArgs) =>
            {
              add_channel_popup.HorizontalOffset = (Window.Current.Bounds.Width - border.ActualWidth) / 2;
              add_channel_popup.VerticalOffset = 100;

            };
            add_channel_popup.IsOpen = true;
          };
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

          //
          // TODO: populate list with devices
          // 

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
            popup_content.setDeviceType(true);
            Border border = new Border
            {
                Child = popup_content,
                Background = new SolidColorBrush(Colors.LightBlue),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(4),
                Padding = new Thickness(24),
            };

            //border.Background.Opacity = 0.5;
            add_device_popup = new Popup
            {
                Child = border,
                IsLightDismissEnabled = true
            };

            add_device_popup.Closed +=add_device_popup_Closed;

            border.Loaded += (loadedSender, loadedArgs) =>
                {
                    add_device_popup.HorizontalOffset = (Window.Current.Bounds.Width - border.ActualWidth) / 2;
                    add_device_popup.VerticalOffset = (Window.Current.Bounds.Height - border.ActualHeight) / 2;
                };
            popup_content.setParentPopup(ref add_device_popup);
            add_device_popup.IsOpen = true;
            return;
        }

        // Called when the pop-up is closed. Needs to cancel bluetooth learning process
        private void add_device_popup_Closed(object sender, object e)
        {
            //TODO: 
       
        }

        private void add_channel_popup_Closed(object sender, object e)
        {
          //TODO: 

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
