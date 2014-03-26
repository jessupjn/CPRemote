using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
  public sealed partial class SelectedDevice : UserControl
  {
    public event ChangedEventHander deletePressed;
    public event ChangedEventHander selectPressed;

    public SelectedDevice()
    {
      this.InitializeComponent();
    }

    private void deleteDevice(object sender, RoutedEventArgs e) { deletePressed.Invoke(this, EventArgs.Empty); }

    private void selectDevice(object sender, RoutedEventArgs e) { selectPressed.Invoke(this, EventArgs.Empty); }
  }
}
