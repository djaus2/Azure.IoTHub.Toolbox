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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Azure_IoTHub_Toolbox_App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        internal static void Stop()
        {
            
        }

        private  void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                bool? sho = (bool)e.Parameter;
                if (sho != null)
                {
                    if (sho == true)
                         Popup_SetConnectionDetails.IsOpen = true;
                }
            }

            // parameters.Name
            // parameters.Text
            // ...
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Popup_SetConnectionDetails.IsOpen = false;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ref: https://docs.microsoft.com/en-us/windows/uwp/monetize/request-ratings-and-reviews#show-a-rating-and-review-dialog-in-your-app
            bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9WZDNCRFHVJL"));
        }
    }
}
