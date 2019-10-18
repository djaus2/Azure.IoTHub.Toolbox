using System.ComponentModel;

namespace Azure_IoTHub_Toolbox_App
{
    public interface IAppSettingsValues
    {
        bool AutoStartDevice { get; set; }
        bool AutoStartSvc { get; set; }
        int DeviceAction { get; set; }
        bool DeviceBasicMode { get; set; }
        int DeviceTimeout { get; set; }
        bool DeviceUseCustomClass { get; set; }
        bool ExpectResponse { get; set; }
        bool KeepAliveSvc { get; set; }
        bool KeepDeviceListening { get; set; }
        bool SvcBasicMode { get; set; }
        int SvcTelemetryDelayBtwReadingsToSend { get; set; }
        int SvcTimeout { get; set; }
        bool SvcUseCustomClass { get; set; }
        int TelemetryDelayBtwReadings { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }



    public class Settings
    {

        public int runMode { get; set; } = 1;
        public int waitAtEndOfConsoleAppSecs { get; set; } = 5;
        public int DeviceTimeout { get; set; }

        public int DeviceAction { get; set; }

        public bool basicMode { get; set; }
        public bool UseCustomClass { get; set; }
        public bool ResponseExpected { get; set; }
        public bool KeepAlive { get; set; }

        public string device_id { get; set; }
        public string device_cs { get; set; }

        public bool KeepDeviceListening { get; set; }

        //The next is superfulous as this device app will always autostart.
        public bool AutoStartDevice { get; set; }
        public string iothub_cs { get; internal set; }
    }
}