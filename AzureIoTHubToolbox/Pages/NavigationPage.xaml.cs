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

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                DoSettings();
            }
            else
            {
                switch (currentPage)
                {
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
                }
                var eventargs = (NavigationViewSelectionChangedEventArgs)args;
                if (int.TryParse( (string)((Control)eventargs.SelectedItem).Tag, out int newPage))
                {
                    if ((new List<int>() { 1, 2, 3, 4, 5 }).Contains(newPage))
                    {
                        currentPage = newPage;
                        switch (currentPage)
                        {
                            case 1:
                                NavigationFrame.Navigate(typeof(Pages.HomePage), null);
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
                            case 5:
                                NavigationFrame.Navigate(typeof(Pages.LinksPage), null);

                                break;
                        }
                    }
                }
            }
           
        }

        private void DoSettings()
        {
            NavigationFrame.Navigate(typeof(Pages.NewHub), null);

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(typeof(Pages.HomePage), null);
        }
    }
}
