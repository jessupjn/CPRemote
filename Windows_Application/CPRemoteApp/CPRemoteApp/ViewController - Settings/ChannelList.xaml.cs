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
    public int tag;
    public event ChangedEventHander Changed;
    public event ChangedEventHander deletePressed;
    public event ChangedEventHander editPressed;

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

      if(_tag == -1)
      {
        _edit_button.Visibility = Visibility.Collapsed;
        _remove_button.Visibility = Visibility.Collapsed;
      }
      tag = _tag;

    }

    private void click(object sender, object e) { if(Changed != null) Changed.Invoke(this, EventArgs.Empty); }

    private void editClicked(object sender, object e) { Debug.WriteLine("Edit Pressed"); if (editPressed != null) editPressed.Invoke(this, EventArgs.Empty); }

    private void deleteClicked(object sender, object e) { if(deletePressed != null) deletePressed.Invoke(this, EventArgs.Empty); }

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
