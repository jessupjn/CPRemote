using CPRemoteApp.ViewController___Remote;
using System;
using System.Collections.Generic;
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
    public AddNewChannelPopup()
    {
      this.InitializeComponent();
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
        Uri temp_icon_path = new Uri("ms-appx:///img/media_volume_down.png");
        RemoteButton b = new RemoteButton(_ch_name.Text, _ch_name.Text, _ch_num.Text, 1, temp_icon_path);
        ((App)CPRemoteApp.App.Current).deviceController.channelController.add_channel(b);
        closePopup(null, null);
    }

    private void uploadClicked(object sender, RoutedEventArgs e)
    {

    }

  }
}
