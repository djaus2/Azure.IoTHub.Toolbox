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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Azure_IoTHub_Toolbox_App.Pages
{

    public class IoTHubConnectionDetails
    {

        public static bool EHMethod1 { get { return MyConnections.EHMethod1; } set { MyConnections.EHMethod1 = value; } }
        public static string ResourceGroupName { get { return MyConnections.AzureGroup; } set { MyConnections.AzureGroup = value; } }
        public static string IoTHubName { get { return MyConnections.IoTHubName; } set { MyConnections.IoTHubName = value; } }
        public static string DeviceId { get { return MyConnections.DeviceId; } set { MyConnections.DeviceId = value; } }

        public static string IoTHubConnectionString { get { return MyConnections.IoTHubConnectionString; } set { MyConnections.IoTHubConnectionString = value; } }

        public static string DeviceConnectionString { get { return MyConnections.DeviceConnectionString; } set { MyConnections.DeviceConnectionString = value; } }

        public static string EventHubsConnectionString { get { return MyConnections.EventHubsConnectionString; } set { MyConnections.EventHubsConnectionString = value; } }

        public static string EventHubsCompatibleEndpoint { get { return MyConnections.EventHubsCompatibleEndpoint; } set { MyConnections.EventHubsCompatibleEndpoint = value; } }

        public static string EventHubsCompatiblePath { get { return MyConnections.EventHubsCompatiblePath; } set { MyConnections.EventHubsCompatiblePath = value; } }

        public static string EventHubsSasKey { get { return MyConnections.EventHubsSasKey; } set { MyConnections.EventHubsSasKey = value; } }

        public static string IotHubKeyName { get { return MyConnections.IotHubKeyName; } set { MyConnections.IotHubKeyName = value; } }

        public static string IoTHubLocation { get { return MyConnections.IoTHubLocation; } set { MyConnections.IoTHubLocation = value; } }

        public static string SKU { get { return MyConnections.SKU; } set { MyConnections.SKU = value; } }

    }
    [Windows.UI.Xaml.Data.Bindable]
    public class Data : INotifyPropertyChanged
    {

        //public string Code { get; set; } = "MyCode";
        //public string TextInfo { get; set; } = "MyText";
        //public string InfoValue { get; set; } = "MyInfoValue";

        private string sku1 = "F1";
        private string location1 = "centralus";
        private string resourceGroupName = "GroupName";
        private string ioTHubName = "HubName";
        private string deviceId = "";
        private string deviceConnectionString = "";
        private string ioTHubConnectionString = "";

        public Data()
        {
            ResetData();
        }

        public void ResetData()
        {
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.ResourceGroupName))
                ResourceGroupName = IoTHubConnectionDetails.ResourceGroupName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IoTHubName))
                IoTHubName = IoTHubConnectionDetails.IoTHubName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.DeviceId))
                DeviceId = IoTHubConnectionDetails.DeviceId;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IoTHubConnectionString))
                IoTHubConnectionString = IoTHubConnectionDetails.IoTHubConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.DeviceConnectionString))
                DeviceConnectionString = IoTHubConnectionDetails.DeviceConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsCompatiblePath))
                EventHubsCompatiblePath = IoTHubConnectionDetails.EventHubsCompatiblePath;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsCompatibleEndpoint))
                EventHubsCompatibleEndpoint = IoTHubConnectionDetails.EventHubsCompatibleEndpoint;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsConnectionString))
                EventHubsConnectionString = IoTHubConnectionDetails.EventHubsConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IotHubKeyName))
                IotHubKeyName = IoTHubConnectionDetails.IotHubKeyName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IotHubKeyName))
                IotHubKeyName = IoTHubConnectionDetails.IotHubKeyName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsSasKey))
                EventHubsSasKey = IoTHubConnectionDetails.EventHubsSasKey;
            EHMethod1 = IoTHubConnectionDetails.EHMethod1;
        }

        public void Commit()
        {
            if (!string.IsNullOrEmpty(ResourceGroupName))
                IoTHubConnectionDetails.ResourceGroupName = ResourceGroupName;
            if (!string.IsNullOrEmpty(IoTHubName))
                IoTHubConnectionDetails.IoTHubName = IoTHubName;
            if (!string.IsNullOrEmpty(DeviceId))
                IoTHubConnectionDetails.DeviceId = DeviceId;
            if (!string.IsNullOrEmpty(IoTHubConnectionString))
                IoTHubConnectionDetails.IoTHubConnectionString = IoTHubConnectionString;
            if (!string.IsNullOrEmpty(DeviceConnectionString))
                IoTHubConnectionDetails.DeviceConnectionString = DeviceConnectionString;
            if (!string.IsNullOrEmpty(EventHubsCompatiblePath))
                IoTHubConnectionDetails.EventHubsCompatiblePath = EventHubsCompatiblePath;
            if (!string.IsNullOrEmpty(EventHubsCompatibleEndpoint))
                IoTHubConnectionDetails.EventHubsCompatibleEndpoint = EventHubsCompatibleEndpoint;
            if (!string.IsNullOrEmpty(EventHubsConnectionString))
                IoTHubConnectionDetails.EventHubsConnectionString = EventHubsConnectionString;
            if (!string.IsNullOrEmpty(IotHubKeyName))
                IoTHubConnectionDetails.IotHubKeyName = IotHubKeyName;
            if (!string.IsNullOrEmpty(EventHubsSasKey))
                IoTHubConnectionDetails.EventHubsSasKey = EventHubsSasKey;
            IoTHubConnectionDetails.EHMethod1 = EHMethod1;
            ApplicationSettings.SaveSettingsToAppData();
        }

        private bool eHMethod1 = true;
        public bool EHMethod1 { get => eHMethod1; set { if (eHMethod1 != value) { eHMethod1 = value; OnPropertyChanged(); OnPropertyChanged("EHMethod2"); } } }
        public bool EHMethod2 { get { return !eHMethod1; } }

        private string eventHubsConnectionString = "";
        private string eventHubsCompatibleEndpoint = "";
        private string eventHubsCompatiblePath = "";
        private string eventHubsSasKey = "";


        private string iotHubKeyName = "";

        public string IotHubKeyName { get => iotHubKeyName; set { if (iotHubKeyName != value) { iotHubKeyName = value; OnPropertyChanged(); } } }

        /* Relevant Code:
            var EventHubConnectionString = new EventHubsConnectionStringBuilder(
            new Uri(Azure_IoTHub_Connections.MyConnections.EventHubsCompatibleEndpoint),
            Azure_IoTHub_Connections.MyConnections.EventHubsCompatiblePath,
            Azure_IoTHub_Connections.MyConnections.IotHubKeyName,
            Azure_IoTHub_Connections.MyConnections.Saskey);

            OR

            EventHubConnectionString = new EventHubsConnectionStringBuilder("Endpoint=sb://ihsuproddmres016dednamespace.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=onANjo3Aj7/ess/UO9dcnmBeZkCbr1WPXFz6x0HQdc0=;EntityPath=iothub-ehub-mynewhub-1918909-a3ba8a9102");

            s_eventHubClient = EventHubClient.CreateFromConnectionString(EventHubConnectionString.ToString());
         */

        //Use if Method1:
        public string EventHubsConnectionString { get => eventHubsConnectionString; set { if (eventHubsConnectionString != value) { eventHubsConnectionString = value; OnPropertyChanged(); } } }

        //Use if Method 2:
        public string EventHubsCompatibleEndpoint { get => eventHubsCompatibleEndpoint; set { if (eventHubsCompatibleEndpoint != value) { eventHubsCompatibleEndpoint = value; OnPropertyChanged(); } } }
        public string EventHubsCompatiblePath
        {
            get => eventHubsCompatiblePath; set { if (eventHubsCompatiblePath != value) { eventHubsCompatiblePath = value; OnPropertyChanged(); } }
        }
        public string EventHubsSasKey { get => eventHubsSasKey; set { if (eventHubsSasKey != value) { eventHubsSasKey = value; OnPropertyChanged(); } } }


        public string DeviceConnectionString { get => deviceConnectionString; set { if (deviceConnectionString != value) { deviceConnectionString = value; OnPropertyChanged(); } } }
        public string IoTHubConnectionString { get => ioTHubConnectionString; set { if (ioTHubConnectionString != value) { ioTHubConnectionString = value; OnPropertyChanged(); } } }
        public string ResourceGroupName { get => resourceGroupName; set { if (resourceGroupName != value) { resourceGroupName = value; OnPropertyChanged(); } } }
        public string IoTHubName { get => ioTHubName; set { if (ioTHubName != value) { ioTHubName = value; OnPropertyChanged(); } } }
        public string DeviceId { get => deviceId; set { if (deviceId != value) { deviceId = value; OnPropertyChanged(); } } }

        public string location { get => location1; set { if (location1 != value) { location1 = value; OnPropertyChanged(); } } }
        public string sku { get => sku1; set { if (sku1 != value) { sku1 = value; OnPropertyChanged(); } } }










        public string EventHubEnpointhCode
        {
            set { }
            get { return string.Format("az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {0}", IoTHubName); }
        }
        public string EventHubCompatiblePathCode
        {
            set { }
            get { return string.Format("az iot hub show --query properties.eventHubEndpoints.events.path --name {0}", IoTHubName); }
        }

        public string EventHubPrimaryKeyCode
        {
            set { }
            get { return string.Format("az iot hub policy show --name {0} --query primaryKey --hub-name {1}", IotHubKeyName, IoTHubName); }
        }

        public string LoginCode
        {
            get { return "az login"; }
        }
        public string NewGroupCode
        {
            get { return string.Format("az group create --name {0} --location centralus", ResourceGroupName, location); }
        }
        public string NewHubCode
        {

            get { return string.Format("az iot hub create --name {0}    --resource-group {1} --sku {2}", IoTHubName, ResourceGroupName, sku); }
        }
        public string DeleteHubCode
        {
            get { return string.Format("az iot hub delete --name {0}   --resource-group {1}", IoTHubName, ResourceGroupName); }
        }
        public string DeleteGroupCode
        {
            get { return string.Format("az group delete --name {0}", ResourceGroupName); }
        }
        public string iotownerconstring
        {
            get { return string.Format("az iot hub show-connection-string --name {0} --policy-name iothubowner --key primary  --resource-group {1} --output table", IoTHubName, ResourceGroupName); }
        }
        public string serviceconstring
        {
            get { return string.Format("az iot hub show-connection-string --name {0} --policy-name service --key primary  --resource-group {1} --output table", IoTHubName, ResourceGroupName); }
        }

        //az iot hub device-identity show-connection-string --hub-name MyNewHub --device-id MyNewDevice--output table
        public string deviceconstring
        {
            get { return string.Format("az iot hub device-identity show-connection-string --hub-name {0} --device-id {1} --output table", IoTHubName, DeviceId); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
        [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));

                switch (propertyName)
                {
                    case "ResourceGroupName":
                        handler(this, new PropertyChangedEventArgs("NewGroupCode"));
                        handler(this, new PropertyChangedEventArgs("NewHubCode"));
                        handler(this, new PropertyChangedEventArgs("DeleteGroupCode"));
                        handler(this, new PropertyChangedEventArgs("DeleteHubCode"));
                        handler(this, new PropertyChangedEventArgs("iotownerconstring"));
                        handler(this, new PropertyChangedEventArgs("serviceconstring"));
                        break;
                    case "sku":
                        handler(this, new PropertyChangedEventArgs("NewHubCode"));
                        break;
                    case "location":
                        handler(this, new PropertyChangedEventArgs("NewGroupCode"));
                        break;
                    case "IoTHubName":
                        handler(this, new PropertyChangedEventArgs("NewHubCode"));
                        handler(this, new PropertyChangedEventArgs("DeleteHubCode"));
                        handler(this, new PropertyChangedEventArgs("iotownerconstring"));
                        handler(this, new PropertyChangedEventArgs("serviceconstring"));
                        handler(this, new PropertyChangedEventArgs("deviceconstring"));
                        //handler(this, new PropertyChangedEventArgs("EventHubEnpointhCode"));
                        //handler(this, new PropertyChangedEventArgs("EventHubCompatiblePathCode"));
                        //handler(this, new PropertyChangedEventArgs("EventHubPrimaryKeyCode"));
                        break;
                    case "DeviceId":
                        handler(this, new PropertyChangedEventArgs(propertyName));
                        handler(this, new PropertyChangedEventArgs("iotownerconstring"));
                        handler(this, new PropertyChangedEventArgs("serviceconstring"));
                        handler(this, new PropertyChangedEventArgs("deviceconstring"));
                        break;
                }
            }
            //if (!string.IsNullOrEmpty(Property))
            //    ValueChanged?.Invoke(Property);
        }
    }
}

