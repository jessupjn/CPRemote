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
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CPRemoteApp.ViewController___Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public delegate void ChangedEventHander(object sender, EventArgs e);

    public sealed partial class SettingsMenu : Page
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private ObservableCollection<ListBoxItem> channels = new ObservableCollection<ListBoxItem>();
        private Popup popup_control;

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

            string cur_v_device_name = ((App)CPRemoteApp.App.Current).deviceController.volumeController.get_name();
            if(cur_v_device_name != "")
            {
                _volume_device_selected.Text = cur_v_device_name;
            }

            string cur_c_device_name = ((App)CPRemoteApp.App.Current).deviceController.channelController.get_name();
            if(cur_c_device_name != "")
            {
                _channel_device_selected.Text = cur_c_device_name;
            }

            _channellist_listbox.ItemsSource = channels;
            populateChannelList();

            string bt_name = App.bm.connectedDeviceName();
            if (bt_name != null) _bt_device_selected.Text = bt_name;
            App.bm.bt_name_changed += delegate
            {
              string s = App.bm.connectedDeviceName();
              if (s != null) _bt_device_selected.Text = s;
              else _bt_device_selected.Text = "No Device Selected.";
            };
        }

        private void changeSelectedText()
        {
            string cur_v_device_name = ((App)CPRemoteApp.App.Current).deviceController.volumeController.get_name();
            if (cur_v_device_name != "")
            {
                _volume_device_selected.Text = cur_v_device_name;
            }

            string cur_c_device_name = ((App)CPRemoteApp.App.Current).deviceController.channelController.get_name();
            if (cur_c_device_name != "")
            {
                _channel_device_selected.Text = cur_c_device_name;
            }
        }

        private void populateChannelList()
        {
          channels.Clear();

          List<RemoteButton> blist = ((App)(CPRemoteApp.App.Current)).deviceController.channelController.buttonScanner.getButtons();

          ListBoxItem item;
          ChannelList content;
          for (int i = 0; i < blist.Count; i++)
          {
            item = new ListBoxItem();
            content = new ChannelList(blist[i].getName(), i);
            content.deletePressed += channelButtonDeletePressed;
            content.editPressed += channelButtonEditPressed;
            item.Content = content;
            channels.Add(item);
          }
          item = new ListBoxItem();
          content = new ChannelList("Add New Channel", -1);
          item.Content = content;

          content.Changed += delegate
          {

            AddNewChannelPopup popup_content = new AddNewChannelPopup();
            Border border = new Border
            {
              Child = popup_content,
              Width = 840,
              Height = 280,
              Background = new SolidColorBrush(Colors.LightBlue),
              BorderBrush = new SolidColorBrush(Colors.Black),
              BorderThickness = new Thickness(4),
              Padding = new Thickness(20, 10, 20, 0)
            };

            popup_control = new Popup
            {
              Child = border,
              IsLightDismissEnabled = true
            };

            popup_control.Closed += add_channel_popup_Closed;

            border.Loaded += (loadedSender, loadedArgs) =>
            {
              popup_control.HorizontalOffset = (Window.Current.Bounds.Width - border.ActualWidth) / 2;
              popup_control.VerticalOffset = 100;
            };
            popup_content.setParentPopup(ref popup_control);
            popup_control.IsOpen = true;
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

          var result = await menu.ShowForSelectionAsync(invokerRect);
          if (result == null && channel_or_volume)
          {

            List<VolumeDevice> vList = ((App)CPRemoteApp.App.Current).deviceController.getVolumeDevices();
            foreach(VolumeDevice d in vList)
            {
              menu.Commands.Add(new UICommand(d.get_name(), new UICommandInvokedHandler(selectListItem)));
            }
            menu.Commands.Add(new UICommand("Add new volume device...", new UICommandInvokedHandler(addNewVolumeDevice)));
            result = await menu.ShowForSelectionAsync(invokerRect);
          }
          else if(result == null)
          {
            List<ChannelDevice> cList = ((App)CPRemoteApp.App.Current).deviceController.getChannelDevices();
            foreach (ChannelDevice d in cList)
            {
              menu.Commands.Add(new UICommand(d.get_name(), new UICommandInvokedHandler(selectListItem)));
            }
            menu.Commands.Add(new UICommand("Add new channel device...", new UICommandInvokedHandler(addNewChannelDevice)));
            result = await menu.ShowForSelectionAsync(invokerRect);
          }

        }

        private void addNewChannelDevice(IUICommand command) { addNewDevice(command, false); }

        private void addNewVolumeDevice(IUICommand command) {  addNewDevice(command, true); }

        private void addNewDevice(IUICommand command, bool chan_or_vol)
        {
            AddDevicePopup popup_content = new AddDevicePopup();
            popup_content.setDeviceType(chan_or_vol);
            Border border = new Border
            {
                Child = popup_content,
                Background = new SolidColorBrush(Colors.LightBlue),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(4),
                Padding = new Thickness(20,10,20,0),
            };

            popup_control = new Popup
            {
                Child = border,
                IsLightDismissEnabled = true
            };

            popup_control.Closed +=add_device_popup_Closed;

            border.Loaded += (loadedSender, loadedArgs) =>
                {
                    popup_control.HorizontalOffset = (Window.Current.Bounds.Width - border.ActualWidth) / 2;
                    popup_control.VerticalOffset = 100;
                };
            popup_content.setParentPopup(ref popup_control);
            popup_control.IsOpen = true;
            return;
        }

        // Called when the pop-up is closed. Needs to cancel bluetooth learning process
        private void add_device_popup_Closed(object sender, object e)
        {
          string cur_c_device_name = ((App)CPRemoteApp.App.Current).deviceController.channelController.get_name();
          if (cur_c_device_name != "")
          {
            _channel_device_selected.Text = cur_c_device_name;
          }
          string cur_v_device_name = ((App)CPRemoteApp.App.Current).deviceController.volumeController.get_name();
          if (cur_v_device_name != "")
          {
            _volume_device_selected.Text = cur_v_device_name;
          }
        }

        private void add_channel_popup_Closed(object sender, object e) { populateChannelList(); }

        private async void selectDevice(string name)
        {
            if(await ((App)(CPRemoteApp.App.Current)).deviceController.selectChannelDevice(name))
            {
                populateChannelList();
            }
            ((App)(CPRemoteApp.App.Current)).deviceController.selectVolumeDevice(name);
            changeSelectedText();
        }

        private async void selectListItem(IUICommand command)
        {
          SelectedDevice popup_content = new SelectedDevice();
          popup_content.deletePressed += delegate
          {
              ((App)(CPRemoteApp.App.Current)).deviceController.removeChannelDevice(command.Label);
              ((App)(CPRemoteApp.App.Current)).deviceController.removeVolumeDevice(command.Label);
              ((popup_content.Parent as Border).Parent as Popup).IsOpen = false;
          };
          popup_content.selectPressed += delegate
          {
              selectDevice(command.Label);
            ((popup_content.Parent as Border).Parent as Popup).IsOpen = false;
          };
          Border border = new Border
          {
            Child = popup_content,
            Width = 840,
            Height = 280,
            Background = new SolidColorBrush(Colors.LightBlue),
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(4),
            Padding = new Thickness(20,10,20,0)
          };

          popup_control = new Popup
          {
            Child = border,
            IsLightDismissEnabled = true
          };

          popup_control.Closed += add_channel_popup_Closed;

          border.Loaded += (loadedSender, loadedArgs) =>
          {
            popup_control.HorizontalOffset = (Window.Current.Bounds.Width - border.ActualWidth) / 2;
            popup_control.VerticalOffset = 100;
          };
            popup_control.IsOpen = true;
        }


        private Rect GetElementRect(FrameworkElement element)
        {
          GeneralTransform buttonTransform = element.TransformToVisual(null);
          Point point = buttonTransform.TransformPoint(new Point());
          return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        // opens a add new channel with the information previously stored in the button.
        // deletes the old one when save is pressed.
        private void channelButtonEditPressed(object sender, EventArgs e)
        {
          List<RemoteButton> blist = ((App)(CPRemoteApp.App.Current)).deviceController.channelController.buttonScanner.getButtons();
          int ch_tag = (sender as ChannelList).tag;
          AddNewChannelPopup popup_content = new AddNewChannelPopup(blist[ch_tag].getName(), blist[ch_tag].getChannelNumber(), blist[ch_tag].getImgUri());
          Border border = new Border
          {
            Child = popup_content,
            Width = 840,
            Height = 280,
            Background = new SolidColorBrush(Colors.LightBlue),
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(4),
            Padding = new Thickness(20, 10, 20, 0)
          };

          popup_control = new Popup
          {
            Child = border,
            IsLightDismissEnabled = true
          };

          popup_control.Closed += add_channel_popup_Closed;

          border.Loaded += (loadedSender, loadedArgs) =>
          {
            popup_control.HorizontalOffset = (Window.Current.Bounds.Width - border.ActualWidth) / 2;
            popup_control.VerticalOffset = 100;
          };
          popup_content.setParentPopup(ref popup_control);
          popup_control.IsOpen = true;

          popup_content.savePressed += delegate
          {
            // if save is pressed, also delete the button currently being edited to save the new one.
            channelButtonDeletePressed(sender, EventArgs.Empty);
          };
          
        }

        private void channelButtonDeletePressed(object sender, EventArgs e)
        {
          //
          // TODO: LUKE
          // THIS IS WHERE THE CHANNEL NEEDS TO BE DELETED.
          //
          ChannelList c = sender as ChannelList;
          ((App)(CPRemoteApp.App.Current)).deviceController.channelController.remove_channel(c.tag);
          populateChannelList();
        }
    }
}
