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
using Azure_IoTHub_Connections;
using Azure_IoTHub_DeviceStreaming;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI;
using System.Text;

namespace Azure_IoTHub_Toolbox_App.Pages
{

    public sealed partial class ControlDevicePage : Page
    {
        internal static ControlDevicePage mainPage;
        string service_cs = Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString;
        string device_id = Azure_IoTHub_Connections.MyConnections.DeviceId;
        string device_cs = Azure_IoTHub_Connections.MyConnections.DeviceConnectionString;

        public static List<Brush> Cols = new List<Brush>()
        {
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.LightGreen),
            new SolidColorBrush(Colors.LightBlue),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Yellow),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.Pink),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.Red)

        };
            
            
        //    = new List<Brush>
        //{
        //};

        public  List<Microsoft.Azure.Devices.Client.TransportType> ListEnum { get { return typeof(Microsoft.Azure.Devices.Client.TransportType).GetEnumValues().Cast<Microsoft.Azure.Devices.Client.TransportType>().ToList(); } }
        public List<string> ListEnum2 = new List<string> { "Echo", "Uppercase", "Sim Environ", "Sim Telemetry","IoT Hardware" };

        public MainPage()
        {
            this.InitializeComponent();
            IsFirstTime = true;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
  
        }

        private bool IsFirstTime = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainPage = this;
            if (IsFirstTime)
                LoadConSettings();
            
            service_cs = Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString;
            device_id = Azure_IoTHub_Connections.MyConnections.DeviceId;
            device_cs = Azure_IoTHub_Connections.MyConnections.DeviceConnectionString;


            if (IsFirstTime)
            {
                AppBarButton_Click(BtnFeatureMode, null);
                AppBarButton_Click(BtnFeatureMode2, null);

                ListviewTransports2.ItemsSource = ListEnum;
                ListviewTransports2.SelectedItem = Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType;
                ListviewTransports2.ScrollIntoView(ListviewTransports2.SelectedItem);
                Azure_IoTHub_Connections.MyConnections.OnStatusUpdateD = OnSvcStatusUpdate;
                LstDeviceAction.ItemsSource = ListEnum2;
                LstDeviceAction.SelectedItem = ListEnum2[1];
                LstDeviceAction.ScrollIntoView(ListEnum2[1]);
                if (Azure_IoTHub_Toolbox_App.AppSettingsValues.Settings.AutoStartDevice)
                {
                    Button_Click_Device(null, null);
                }

                if (Azure_IoTHub_Toolbox_App.AppSettingsValues.Settings.AutoStartSvc)
                {
                    Button_Click_Svc(null, null);

                }
                IsFirstTime = false;
                //tbTelemetryDelay.DataContext = Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings;
                this.DataContext = Azure_IoTHub_Toolbox_App.AppSettingsValues.Settings;
            }
        }



        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GeneralTransform gt = tbSvcMsgOut.TransformToVisual(this);
            Point offset = gt.TransformPoint(new Point(0, 0));
            //double controlTop = offset.Y;
            double controlLeft = offset.X;
            double newWidth =  e.NewSize.Width - controlLeft - 20;
            if (newWidth > tbSvcMsgOut.MinWidth)
            {
                tbSvcMsgOut.Width = newWidth;
                tbDeviceMsgIn.Width = newWidth;
                tbSvcMsgIn.Width = newWidth;
                tbDeviceMsgOut.Width = newWidth;
            }
        }


        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Control cntrl = (Control)sender;

            bool issvcMode = false;
            bool isDeviceMode = false;
            bool isSvc2ndMenu = false;
            bool isDevice2ndMenu = false;
            switch((string)cntrl.Tag)
            {
                case "0":
                    this.Frame.Navigate(typeof(NewHub), null);
                    break;
                case "1":
                    this.BtnBasicMode.Foreground = new SolidColorBrush(Colors.Yellow);
                    this.BtnFeatureMode.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnExtendedMode.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnBasicMode.Background = new SolidColorBrush(Colors.Green);
                    this.BtnFeatureMode.Background = SvcCommands2.Background;
                    this.BtnExtendedMode.Background = SvcCommands2.Background;
                    svcBasicMode = true;
                    svcCustomClassMode = false;
                    issvcMode = true;
                    break;
                case "2":
                    this.BtnBasicMode.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnFeatureMode.Foreground = new SolidColorBrush(Colors.Yellow);
                    this.BtnExtendedMode.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnBasicMode.Background = SvcCommands2.Background;
                    this.BtnFeatureMode.Background = new SolidColorBrush(Colors.Green);
                    this.BtnExtendedMode.Background = SvcCommands2.Background;
                    svcBasicMode = false;
                    svcCustomClassMode = false;
                    issvcMode = true;
                    break;
                case "3":
                    this.BtnBasicMode.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnFeatureMode.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnExtendedMode.Foreground = new SolidColorBrush(Colors.Yellow);
                    this.BtnBasicMode.Background = SvcCommands2.Background;
                    this.BtnFeatureMode.Background = SvcCommands2.Background;
                    this.BtnExtendedMode.Background = new SolidColorBrush(Colors.Green);
                    svcBasicMode = false;
                    svcCustomClassMode = true;
                    issvcMode = true;
                    break;
                case "11":
                    this.BtnBasicMode2.Foreground = new SolidColorBrush(Colors.Yellow);
                    this.BtnFeatureMode2.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnExtendedMode2.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnBasicMode2.Background = new SolidColorBrush(Colors.Red);
                    this.BtnFeatureMode2.Background = DeviceProcessingModeCommands.Background;
                    this.BtnExtendedMode2.Background = DeviceProcessingModeCommands.Background;
                    DeviceBasicMode = true;
                    DeviceUseCustomClass = false;
                    isDeviceMode = true;
                    break;
                case "12":
                    this.BtnBasicMode2.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnFeatureMode2.Foreground = new SolidColorBrush(Colors.Yellow);
                    this.BtnExtendedMode2.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnBasicMode2.Background = DeviceProcessingModeCommands.Background;
                    this.BtnFeatureMode2.Background = new SolidColorBrush(Colors.Red);
                    this.BtnExtendedMode2.Background = DeviceProcessingModeCommands.Background;
                    DeviceBasicMode = false;
                    DeviceUseCustomClass = false;
                    isDeviceMode = true;
                    break;
                case "13":
                    this.BtnBasicMode2.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnFeatureMode2.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    this.BtnExtendedMode2.Foreground = new SolidColorBrush(Colors.Yellow);
                    this.BtnBasicMode2.Background = DeviceProcessingModeCommands.Background;
                    this.BtnFeatureMode2.Background = DeviceProcessingModeCommands.Background;
                    this.BtnExtendedMode2.Background = new SolidColorBrush(Colors.Red);
                    DeviceBasicMode = false;
                    DeviceUseCustomClass = true;
                    isDeviceMode = true;
                    break;
                case "4":

                    if (AppBarToggle5.IsChecked == true)
                        AppBarToggle5.IsChecked = false;
                    break;
                case "5":
                    if (AppBarToggle4.IsChecked == true)
                        AppBarToggle4.IsChecked = false;
                    break;
                case "6":
                    if (AppBarToggle7.IsChecked == true)
                        AppBarToggle7.IsChecked = false;
                    break;
                case "7":
                    if (AppBarToggle6.IsChecked == true)
                        AppBarToggle6.IsChecked = false;
                    break;

            }
            if (isSvc2ndMenu)
            {
                if ((AppBarToggle4.IsChecked == true) && !(AppBarToggle5.IsChecked == true))
                    DevAutoStart = 0;
                else if ((AppBarToggle5.IsChecked == true) && (!(AppBarToggle4.IsChecked == true)))
                    DevAutoStart = 1;
                else
                    DevAutoStart = 2;

                if ((AppBarToggle5.IsChecked == true) && !(AppBarToggle7.IsChecked == true))
                    DevKeepListening = 0;
                else if ((AppBarToggle7.IsChecked == true) && (!(AppBarToggle6.IsChecked == true)))
                    DevKeepListening = 1;
                else
                    DevKeepListening = 2;
            }
            else if (isDevice2ndMenu)
            {

            }

            if( issvcMode )
            {

            }
            if( isDeviceMode)
            {

            }

            SvcCommands2.IsOpen = false;
        }

        private void ClearAllToggles()
        {

            AppBarToggle5.IsChecked = false;
            AppBarToggle4.IsChecked = false;
            AppBarToggle7.IsChecked = false;
            AppBarToggle6.IsChecked = false;
            DevAutoStart = 2;
            DevKeepListening = 2;
            SvcCommands2.IsOpen = false;
        }

        private void LoadConSettings()
        {
            ApplicationSettings.LoadConSettings();
        }

        public static void SaveSettingsToAppData()
        {
            ApplicationSettings.SaveSettingsToAppData();
        }

        private void DeviceProcessingModeCommands_Opening(object sender, object e)
        {

        }



        private void CommandBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CommandBar cb = (sender is CommandBar) ? (CommandBar)sender : null;
            if(cb != null)
            {
                BtnSettings.IsCompact = !BtnSettings.IsCompact;
                BtnBasicMode.IsCompact = BtnSettings.IsCompact;
                BtnFeatureMode.IsCompact = BtnSettings.IsCompact;
                BtnExtendedMode.IsCompact = BtnSettings.IsCompact;
            }
            
        }

        private void DeviceProcessingModeCommands_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BtnBasicMode2.IsCompact = !BtnBasicMode2.IsCompact;
            BtnFeatureMode2.IsCompact = BtnBasicMode2.IsCompact;
            BtnExtendedMode2.IsCompact = BtnBasicMode2.IsCompact;
        }

        private void BtnClrRecvdText_Click(object sender, RoutedEventArgs e)
        {
            tbSvcMsgIn.Text = "";
            tbDeviceMsgIn.Text = "";
            tbDeviceMsgOut.Text = "";
        }




    }
}
