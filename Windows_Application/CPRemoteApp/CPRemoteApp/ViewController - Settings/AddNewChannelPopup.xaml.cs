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
        this._save_button.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        if(savePressed != null) savePressed.Invoke(this, EventArgs.Empty);
        Uri temp_icon_path = new Uri("ms-appx:///img/unset.png");
        Debug.WriteLine(temp_icon_path.ToString());
        RemoteButton b = new RemoteButton(_ch_name.Text, _ch_name.Text, _ch_num.Text, 1, temp_icon_path);
        ((App)CPRemoteApp.App.Current).deviceController.channelController.add_channel(b);
        closePopup(null, null);
    }

    private void uploadClicked(object sender, RoutedEventArgs e)
    {

    }

  }
}
