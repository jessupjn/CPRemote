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

namespace CPRemoteApp.ViewController___Remote
{
    public sealed partial class RemoteButtonScanner : UserControl
    {
        // -------------------------------------
        private List<Button> button_list;
        private int cur_index;

        // -------------------------------------

        public RemoteButtonScanner()
        {
            this.InitializeComponent();
        }

        //Should take in an input stream that it can read from to initialize all the buttons associated with it
        public void initialize(Windows.Storage.StorageFile file)
        {
            
        }
    }
}
