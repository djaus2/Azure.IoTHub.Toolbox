using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Azure_IoTHub_Toolbox_App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        private int currentPage  =-1;
        public NavigationPage()
        {
            this.InitializeComponent();
        }

        private  void  NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                DoSettings();
            }
            else
            {
                switch (currentPage)
                {
                    case 0:
                        NewHub.Stop();
                        break;
                    case 1:
                        HomePage.Stop();
                        break;
                    case 2:
                        MainPage.Stop();
                        break;
                    case 3:
                        TelemetryPage.Stop();
                        break;
                    case 4:
                        ControlDeviceTelemetryPage.Stop();
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                }
                var eventargs = (NavigationViewSelectionChangedEventArgs)args;
                if (int.TryParse( (string)((Control)eventargs.SelectedItem).Tag, out int newPage))
                {
                    if ((new List<int>() { 1, 2, 3, 4, 5, 6, 7 }).Contains(newPage))//,8}).Contains(newPage))
                    {
                        currentPage = newPage;
                           
                        switch (currentPage)
                        {
                            case 1:
                                Pages.HomePage.ShowPanel = 1;
                                NavigationFrame.Navigate(typeof(Pages.HomePage), false);
                                break;
                            case 2:
                                NavigationFrame.Navigate(typeof(Pages.MainPage), null);
                                break;
                            case 3:
                                NavigationFrame.Navigate(typeof(Pages.TelemetryPage), null);
                                break;
                            case 4:
                                NavigationFrame.Navigate(typeof(Pages.ControlDeviceTelemetryPage), null);
                                break;
                            //case 8:
                            //    NavigationFrame.Navigate(typeof(Pages.ProxyPage), null);
                            //    break;
                            case 5:
                                NavigationFrame.Navigate(typeof(Pages.DeployToDevice), null);
                                break;
                            case 6:
                                NavigationFrame.Navigate(typeof(Pages.LinksPage), null);
                                break;

                            case 7:
                                    currentPage = 1;
                                Pages.HomePage.ShowPanel = 2;
                                    NavigationFrame.Navigate(typeof(Pages.HomePage),true);
                                break;
                        }
                    }
                }
            }
           
        }

        private void DoSettings()
        {
            currentPage = 0;
            NavigationFrame.Navigate(typeof(Pages.NewHub), null);

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(Pages.HomePage), null);
        }

        //private async void Button1_Click(object sender, RoutedEventArgs e)
        //{
        //    // Show the content dialog
        //    await DiaplyContentDialog();
        //}

        //// Custom method to create and diaplay a content dialog
        //private async Task DiaplyContentDialog()
        //{
        //    Package package = Package.Current;
        //    PackageId packageId = package.Id;
        //    PackageVersion version = packageId.Version;
        //    Windows.UI.Xaml.Controls.r
        //    string info1 = package.Description;
        //    string info2 = "Publisher: " +package.PublisherDisplayName;

        //    StackPanel sp = new StackPanel();
        //    sp.Children.Add(new TextBlock());
            

        //    string info = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            
        //        ContentDialog dialog = new ContentDialog()
        //    {
        //        Title = package.DisplayName,
        //        Content = "sp;
        //        PrimaryButtonText = "Ok",
           
                 
        //    };

        //    // Finally, show the dialog
        //    await dialog.ShowAsync();
        //}
    }
}
