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
                var eventargs = (NavigationViewSelectionChangedEventArgs)args;
                switch (((string)(
                    (Control)eventargs.SelectedItem).Tag))
                {
                    case "1":
                        NavigationFrame.Navigate(typeof(Pages.HomePage), null);
                        break;
                    case "2":
                        NavigationFrame.Navigate(typeof(Pages.MainPage), null);
                        break;
                    case "3":
                        NavigationFrame.Navigate(typeof(Pages.TelemetryPage), null);
                        break;
                    case "4":
                        break;
                }
            }
           
        }

        private void DoSettings()
        {
            NavigationFrame.Navigate(typeof(Pages.NewHub), null);

        }

    }
}
