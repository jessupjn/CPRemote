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
    public AddNewChannelPopup()
    {
      this.InitializeComponent();
    }

    private void saveClicked(object sender, object e)
    {

    }

    private void clickerIn(object sender, object e)
    {
      Canvas c = sender as Canvas;
      _save_button_bg.Fill = new SolidColorBrush(Color.FromArgb(255, 118, 195, 197));
    }

    private void clickerOut(object sender, object e)
    {
      Canvas c = sender as Canvas;
      _save_button_bg.Fill = new SolidColorBrush(Color.FromArgb(255, 18, 11, 66));
    }
  }
}
