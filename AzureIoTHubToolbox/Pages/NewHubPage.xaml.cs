using Azure_IoTHub_Connections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
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
    public sealed partial class NewHub : Page
    {
        public static NewHub newHub = null;
        public void CreateNewEntity(string g)
        {
            System.Diagnostics.Debug.WriteLine("CreateNewEntity " + g);
            switch (g)
            {
                case "Device":
                    Azure_IoTHub_Connections.MyConnections.AddDeviceAsync(Data1.IoTHubConnectionString, Data1.DeviceId);
                    Data1.DeviceConnectionString = IoTHubConnectionDetails.DeviceConnectionString;
                    break;
            }
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Data1.Commit();
        }

        public void DeleteEntity(string g)
        {
            System.Diagnostics.Debug.WriteLine("DeleteEntity " + g);
            switch (g)
            {
                case "Device":
                    Azure_IoTHub_Connections.MyConnections.RemoveDeviceAsync(Data1.IoTHubConnectionString, Data1.DeviceId);
                    Data1.DeviceConnectionString = "";
                    break;
            }
        }

        public void GenerateEntityInfo(string g)
        {
            System.Diagnostics.Debug.WriteLine("GenerateEntityInfo " + g);
            switch (g)
            {
                case "Hub":
                    GetHubNameFromHubCS();
                    break;
                case "Device":
                    GetDeviceIdFromDeviceCS();
                    break;
                case "CSHub":
                    //GetHubNameFromHubCS();
                    break;
                case "CSDevice":
                    //GetHubNameFromHubCS();
                    break;
            }
        }

        public Data Data1 = new Data();

        







        public NewHub()
        {
            newHub = this;
            //NGCode.DataContext = Data1.NewGroupCode;
            //this.DataContext = Data1;

            this.InitializeComponent();
            //if (this.Frame.CanGoBack)
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            //else
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            //NG.ValueChanged = ValueChangedGroup;
            //NH.ValueChanged = ValueChangedHub;

            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

        }

        public void Update()
        {
            Task.Run(async () =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //if (!string.IsNullOrEmpty(Cons.ResourceGroupName))
                    //    NG.Text = Cons.ResourceGroupName;
                    //if (!string.IsNullOrEmpty(Cons.IoTHubName))
                    //    NH.Text = Cons.IoTHubName;
                    //if (!string.IsNullOrEmpty(Cons.DeviceId))
                    //    ND.Text = Cons.DeviceId;
                    //if (!string.IsNullOrEmpty(Cons.IoTHubConnectionString))
                    //    NCS1.Text = Cons.IoTHubConnectionString;
                    //if (!string.IsNullOrEmpty(Cons.DeviceConnectionString))
                    //    NCS2.Text = Cons.DeviceConnectionString;
                    //ResourceGroupName = NG.Text;
                    //IoTHubName = NH.Text;

                    //Login.Code = LoginCode;
                    //NGCode.Code = NewGroupCode;
                    //NHCode.Code = NewHubCode;
                    //DelHub.Code = DeleteHubCode;
                    //DelGrp.Code = DeleteGroupCode;
                    //HubOwnerConString.Code = iotownerconstring;
                    //HubServoceConString.Code = serviceconstring;
                    //Multicom.Code = "To create a new Device connection to the Hub you need the iothubowner ConnectionString."
                    //+"To run the DeviceStreaming functionality you only need the Service ConnectionString but can use the iothubowner ConnectionString. " 
                    //+"To create a new Device, return and choose [ADD New IoT Hub Device ...] from the Service menu.";

                });
            });
        }

        //public void ValueChangedGroup(string val)
        //{
        //    ResourceGroupName = val;
        //    Update();         
        //}

      

        private void GetSasKey(string property)
        {
            Data1.EventHubsSasKey= Azure_IoTHub_Telemetry.ReadDeviceToCloudMessages.GetSasKey(Data1.IoTHubConnectionString);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            h1.SubRegion = this.GettingStarted;
            h2.SubRegion = this.Info;
            h2.IsExpanded = true;
            h3.SubRegion = this.ConnectingAndGroup;
            h4.SubRegion = this.NewHubRegion;
            h5_1.SubRegion = this.DeviceConStringParagraph;
            h5.SubRegion = this.NewHDevice;
            h6.SubRegion = this.Cleanup;
            h7.SubRegion = this.Misc;
            h2ev.SubRegion = this.EventHubInfo;
            EventHubMethod1Heading.SubRegion = this.EventHubMethod1;
            EventHubMethod2Heading.SubRegion = this.EventHubMethod2;

            NEHSharedAcessKey.GenerateEntityInfo = this.GetSasKey; 
            //if (!string.IsNullOrEmpty(Cons.IoTHubConnectionString))
            //    NCS1.TextInfo = Cons.IoTHubConnectionString;
            //if (!string.IsNullOrEmpty(Cons.DeviceConnectionString))
            //    NCS2.TextInfo = Cons.DeviceConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.SKU))
            {
                if (IoTHubConnectionDetails.SKU == "F1")
                {
                    rbF1.IsChecked = true;
                }
                else
                    rbS1.IsChecked = true;
            }
            else
                rbS1.IsChecked = true;
            //if (!string.IsNullOrEmpty(Cons.ResourceGroupName))
            //    NG.Text = Cons.ResourceGroupName;
            //if (!string.IsNullOrEmpty(Cons.IoTHubName))
            //    NH.Text = Cons.IoTHubName;
            //if (!string.IsNullOrEmpty(Cons.DeviceId))
            //    ND.Text = Cons.DeviceId;
            //if (!string.IsNullOrEmpty(Cons.IoTHubConnectionString))
            //    NCS1.Text = Cons.IoTHubConnectionString;
            //if (!string.IsNullOrEmpty(Cons.DeviceConnectionString))
            //    NCS2.Text = Cons.DeviceConnectionString;
            //if (!string.IsNullOrEmpty(Cons.SKU))
            //{
            //    if (Cons.SKU == "F1")
            //    {
            //        rbF1.IsChecked = true;
            //    }
            //    else
            //        rbS1.IsChecked = true;
            //}
            //else
            //    rbS1.IsChecked = true;


            //Update();
            //NewHubElement.ValueChanged = ValueChangedD;

            NH.GenerateEntityInfo = GenerateEntityInfo;
            ND.CreateNewEntity = CreateNewEntity;
            ND.DeleteEntity = DeleteEntity;
            ND.GenerateEntityInfo = GenerateEntityInfo;
            NCS1.GenerateEntityInfo = GenerateEntityInfo;
            NCS2.GenerateEntityInfo = GenerateEntityInfo;
        }

        private void RbF1_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is Control)
            {
                if (((Control)sender).Tag != null)
                {
                    string tag = (string)((Control)sender).Tag;
                    if (!string.IsNullOrEmpty(tag))
                    {
                        switch (tag)
                        {
                            case "0":
                                Data1.sku = "F1";
                                break;
                            case "1":
                                Data1.sku = "S1";
                                break;
                        }
                        Update();
                    }
                }
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
   
            var cb = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            string HubconString = await cb.GetTextAsync();
            if (!string.IsNullOrEmpty(HubconString))
            {
                Data1.IoTHubName = HubconString;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Azure_IoTHub_Connections.MyConnections.AddDeviceAsync(Data1.IoTHubConnectionString, Data1.DeviceId);
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.DeviceConnectionString))
            {
                Data1.DeviceConnectionString = IoTHubConnectionDetails.DeviceConnectionString;
            }
        }

        private async void GetHubNameFromHubCS()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (string.IsNullOrEmpty(Data1.IoTHubConnectionString))
                    return;
                string cshub = Data1.IoTHubConnectionString;
                string[] parts = cshub.Split(new char[] { '=' });
                if (parts.Length > 1)
                {
                    if (!string.IsNullOrEmpty(parts[1]))
                    {
                        parts = parts[1].Split(new char[] { '.' });
                        if (parts.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(parts[0]))
                            {
                                Data1.IoTHubName = parts[0];
                            }
                        }
                    }
                }
            });
        }

        private async void GetDeviceIdFromDeviceCS()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (string.IsNullOrEmpty(Data1.DeviceConnectionString))
                    return;
                string cshub = Data1.DeviceConnectionString;
                string[] parts = cshub.Split(new char[] { ';' });
                if (parts.Length > 1)
                {
                    if (!string.IsNullOrEmpty(parts[1]))
                    {
                        parts = parts[1].Split(new char[] { '=' });
                        if (parts.Length > 1)
                        {
                            if (!string.IsNullOrEmpty(parts[1]))
                            {
                                Data1.DeviceId = parts[1];
                            }
                        }
                    }
                }
            });
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Data1.ResetData();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Data1.Commit();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // //var pageheight =  e.NewSize.Height;

            // //PageBody.Height = pageheight - PageHeading.Height;
            GeneralTransform gt = PageBody.TransformToVisual(this);
            Point offset = gt.TransformPoint(new Point(0, 0));
            double controlTop = offset.Y;
            //double controlLeft = offset.X;
            double newHeight = e.NewSize.Height - controlTop -PageHeading.Height;
            //if (1=1)
            //{
            PageBody.Height = newHeight;

            // }
        }

        private void RbEHMethod1_Checked(object sender, RoutedEventArgs e)
        {

        }

        public static void Stop()
        {
            newHub.Data1.Commit();
        }
    }
}
