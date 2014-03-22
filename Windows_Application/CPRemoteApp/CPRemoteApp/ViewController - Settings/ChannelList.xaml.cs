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
    private int tag;

    public ChannelList()
    {
      this.InitializeComponent();
      this.Background = new SolidColorBrush(Colors.Transparent);
      _channel_name.Text = "name";
    }

    public ChannelList(string name, int _tag)
    {
      this.InitializeComponent();
      this.Background = new SolidColorBrush(Colors.Transparent);
      _channel_name.Text = name;

      tag = _tag;

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

    private void clickerIn(object sender, object e)
    {
      Canvas c = sender as Canvas;
      if( c.Name == "_edit_button") _edit_button_bg.Fill = new SolidColorBrush(Color.FromArgb(255,118,195,197));
      else _remove_button_bg.Fill = new SolidColorBrush(Color.FromArgb(255,226,139,139));
    }

    private void clickerOut(object sender, object e)
    {
      Canvas c = sender as Canvas;
      if( c.Name == "_edit_button") _edit_button_bg.Fill = new SolidColorBrush(Color.FromArgb(255,54,153,156));
      else _remove_button_bg.Fill = new SolidColorBrush(Color.FromArgb(255,180,72,72));
    }

  }
}
