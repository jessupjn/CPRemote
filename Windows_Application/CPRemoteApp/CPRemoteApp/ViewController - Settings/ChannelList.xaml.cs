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
  public sealed partial class ChannelList : UserControl
  {
    public ChannelList()
    {
      this.InitializeComponent();
      this.Background = new SolidColorBrush(Colors.Transparent);
      _channel_name.Text = "name";
    }

    public ChannelList(string name)
    {
      this.InitializeComponent();
      this.Background = new SolidColorBrush(Colors.Transparent);
      _channel_name.Text = name;

    }


    private void editClicked(object sender, object e)
    {
      Debug.WriteLine("edit");
      // SHOW EDIT MENU
    }

    private void deleteClicked(object sender, object e)
    {
      Debug.WriteLine("delete");
    }


  }
}
