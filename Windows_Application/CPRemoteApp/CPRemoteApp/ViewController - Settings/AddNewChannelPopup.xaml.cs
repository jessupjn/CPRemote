using CPRemoteApp.Utility_Classes;
using CPRemoteApp.ViewController___Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CPRemoteApp.ViewController___Settings
{
  public sealed partial class AddNewChannelPopup : UserControl
  {
    private WeakReference<Popup> popup_ref;
    public event ChangedEventHander savePressed;

    public AddNewChannelPopup()
    {
      this.InitializeComponent();
    }

    public AddNewChannelPopup(string name, string channel_num, Uri img_uri)
    {
      this.InitializeComponent();
      _ch_name.Text = name;
      _ch_num.Text = channel_num;
    }

    public void setParentPopup(ref Popup p)
    {
        popup_ref = new WeakReference<Popup>(p);
    }

    public void closePopup(object sender, RoutedEventArgs e)
    {
        Popup pop;
        popup_ref.TryGetTarget(out pop);
        pop.IsOpen = false;
    }

    private void saveClicked(object sender, object e)
    {
      //
      // CHECK FOR CHANNEL NAME DUPLICATES.
      //
      ChannelDevice c = ((App)(CPRemoteApp.App.Current)).deviceController.channelController;
      foreach(RemoteButton b in c.buttonScanner.getButtons())
      {
        if(b.getName().ToLower() == _ch_name.Text.ToLower())
        {
          MessageDialog msgDialog = new MessageDialog("There is already a channel saved with that name! Please enter a unique name for the channel!", "Whoops!");
          UICommand okBtn = new UICommand("OK");
          okBtn.Invoked += delegate { };
          msgDialog.Commands.Add(okBtn);
          msgDialog.ShowAsync();
          return;
        }
      }

      //
      // CHECK THAT NUMBER IS A NUMBER
      //
      foreach(char a in _ch_num.Text)
      {
        string s = "1234567890";
        if(!s.Contains(a))
        {
          MessageDialog msgDialog = new MessageDialog("The number for the channel can only contain numbers! Please enter a positive integer for the channel number!", "Whoops!");
          UICommand okBtn = new UICommand("OK");
          okBtn.Invoked += delegate { };
          msgDialog.Commands.Add(okBtn);
          msgDialog.ShowAsync();
          return;
        }
      }


      if (_ch_name.Text != "" && _ch_num.Text != "")
      {
        this._save_button.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        if (savePressed != null) savePressed.Invoke(this, EventArgs.Empty);
        Uri temp_icon_path = new Uri("ms-appx:///img/unset.png");
        Debug.WriteLine(temp_icon_path.ToString());
        RemoteButton b = new RemoteButton(_ch_name.Text, _ch_name.Text, _ch_num.Text, 1, temp_icon_path);
        ((App)CPRemoteApp.App.Current).deviceController.channelController.add_channel(b);
        closePopup(null, null);
      }
    }

    private void uploadClicked(object sender, RoutedEventArgs e)
    {
      //
      // TODO: CODE TO UPLOAD AN IMAGE
      //

    }

  }
}
