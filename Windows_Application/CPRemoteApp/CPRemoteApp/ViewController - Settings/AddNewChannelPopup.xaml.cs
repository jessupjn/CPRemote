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
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;


// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CPRemoteApp.ViewController___Settings
{
  public sealed partial class AddNewChannelPopup : UserControl
  {
    private WeakReference<Popup> popup_ref;
    public event ChangedEventHander savePressed;
    public int chan_ind { get; set; }
    private bool is_edit;
    private Uri uri;

    public AddNewChannelPopup()
    {
      if( !((App)(CPRemoteApp.App.Current)).deviceController.channelController.is_initialized )
      {
        MessageDialog msgDialog = new MessageDialog("You must have a channel device set in order to save channels to it!", "Whoops!");
        UICommand okBtn = new UICommand("OK");
        okBtn.Invoked += delegate { };
        msgDialog.Commands.Add(okBtn);
        msgDialog.ShowAsync();
        return;
      }
      else
      {
        this.InitializeComponent();
        is_edit = false; 
      }

    }

    public AddNewChannelPopup(int channel_index, string name, string channel_num, Uri img_uri)
    {
      this.InitializeComponent();
      _ch_name.Text = name;
      _ch_num.Text = channel_num;
      ImageSource imgSource = new BitmapImage(img_uri);
      _img.Source = imgSource;
      is_edit = true;
      chan_ind = channel_index;
      header_text.Text = "Edit Channel";
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
        if(is_edit)
        {
            if (validateChannel())
            {
                RemoteButton btn = createButton();
                ((App)(CPRemoteApp.App.Current)).deviceController.channelController.update_channel(chan_ind, btn);
            }
            closePopup(null, null);
            return;
        }
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

        BitmapImage bi = _img.Source as BitmapImage;
        if (bi == null)
        {
            uri = new Uri("ms-appx:///img/unset.png"); 
        }
        else
        {
            uri = bi.UriSource;

        }
       

        RemoteButton b = new RemoteButton(_ch_name.Text, _ch_name.Text, _ch_num.Text, 1, uri);
        ((App)CPRemoteApp.App.Current).deviceController.channelController.add_channel(b);
        closePopup(null, null);
      }
      else
      {
        MessageDialog msgDialog = new MessageDialog("The channel name and channel number fields are required!", "Whoops!");
        UICommand okBtn = new UICommand("OK");
        okBtn.Invoked += delegate { };
        msgDialog.Commands.Add(okBtn);
        msgDialog.ShowAsync();
        return;
      }
    }

    public bool validateChannel()
    {
        if (!is_edit)
        {
            //
            // CHECK FOR CHANNEL NAME DUPLICATES.
            //
            ChannelDevice c = ((App)(CPRemoteApp.App.Current)).deviceController.channelController;
            foreach (RemoteButton b in c.buttonScanner.getButtons())
            {
                if (b.getName().ToLower() == _ch_name.Text.ToLower())
                {
                    MessageDialog msgDialog = new MessageDialog("There is already a channel saved with that name! Please enter a unique name for the channel!", "Whoops!");
                    UICommand okBtn = new UICommand("OK");
                    okBtn.Invoked += delegate { };
                    msgDialog.Commands.Add(okBtn);
                    msgDialog.ShowAsync();
                    return false;
                }
            }
        }

        //
        // CHECK THAT NUMBER IS A NUMBER
        //
        int n;
        bool isNumeric = int.TryParse(_ch_num.Text.ToString(), out n);
        if(!isNumeric)
        {
            MessageDialog msgDialog = new MessageDialog("The number for the channel can only contain numbers! Please enter a positive integer for the channel number!", "Whoops!");
            UICommand okBtn = new UICommand("OK");
            okBtn.Invoked += delegate { };
            msgDialog.Commands.Add(okBtn);
            msgDialog.ShowAsync();
            return false;
        }


        if (_ch_name.Text.ToString() != "" && _ch_num.Text.ToString() != "")
        {
            this._save_button.Focus(Windows.UI.Xaml.FocusState.Programmatic);

            BitmapImage bi = _img.Source as BitmapImage;
            if (bi == null)
            {
                uri = new Uri("ms-appx:///img/unset.png");
            }
            else
            {
                uri = bi.UriSource;

            }
            return true;
        }
        return false;
    }

    public RemoteButton createButton()
    {
        RemoteButton btn = new RemoteButton(_ch_name.Text, _ch_name.Text, _ch_num.Text, 1, uri);
        return btn;
    }

    async private void uploadClicked(object sender, RoutedEventArgs e)
    {
        FileOpenPicker openPicker = new FileOpenPicker();
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.Downloads;    
        openPicker.FileTypeFilter.Add(".jpg");
        openPicker.FileTypeFilter.Add(".jpeg");
        openPicker.FileTypeFilter.Add(".png");

        StorageFile file = await openPicker.PickSingleFileAsync();
        Popup pop;
        if (file != null)
        {
            StorageFolder img_folder = await  App.appData.LocalFolder.CreateFolderAsync("images_folder", CreationCollisionOption.OpenIfExists);
          
            StorageFile file_name = await file.CopyAsync(img_folder, file.Name,  NameCollisionOption.GenerateUniqueName);
                      
            popup_ref.TryGetTarget(out pop);
            Uri uri = new Uri(file_name.Path);
            Debug.WriteLine(uri.ToString()); 
            ImageSource imgSource = new BitmapImage(uri);
            _img.Source = imgSource; 
            pop.IsOpen = true;
        }
 

    }

    public async Task<bool> IfStorageItemExist(StorageFolder folder, string itemName)
    {
        try
        {
            IStorageItem item = await folder.TryGetItemAsync(itemName);
            return (item != null);
        }
        catch (Exception)
        {
            // Should never get here 
            return false;
        }
    } 


  }
}
