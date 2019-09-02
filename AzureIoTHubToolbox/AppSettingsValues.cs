using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure_IoTHub_Toolbox_App
{
    public  class AppSettingsValues : INotifyPropertyChanged
    {
        /// <summary>
        /// Use this instance for application settings.
        /// Note auto instatiated.
        /// Also these settings are autosaved when set.
        /// And auto loaded if saved with get. Valued stored here then used for subsequents gets.
        /// All property fields are nullable versions (and set to null here in code) 
        ///     which is used to determine if value needs loading upon get.
        /// </summary>
        private static AppSettingsValues applicationsSettings = null;
        public static AppSettingsValues Settings
        {
            get { if (applicationsSettings == null) applicationsSettings = new AppSettingsValues(); return applicationsSettings; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// App Mode bool settings
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////
        #region App Mode bool settings
        public bool? svcBasicMode = null;
        public bool SvcBasicMode { get { GetProperty(ref svcBasicMode); return (svcBasicMode.GetValueOrDefault()); } set { if (svcBasicMode != value) { svcBasicMode = value; OnPropertyChanged(value); } } }


        public bool? svcUseCustomClass = null;
        public bool SvcUseCustomClass { get { GetProperty(ref svcUseCustomClass); return (svcUseCustomClass.GetValueOrDefault()); } set { if (svcUseCustomClass != value) { svcUseCustomClass = value; OnPropertyChanged(value); } } }


        public bool? deviceBasicMode = null;
        public bool DeviceBasicMode { get { GetProperty(ref deviceBasicMode); return (deviceBasicMode.GetValueOrDefault()); } set { if (deviceBasicMode != value) { deviceBasicMode = value; OnPropertyChanged(value); } } }


        public bool? deviceUseCustomClass = null;
        public bool DeviceUseCustomClass { get { GetProperty(ref deviceTimeout); return (deviceUseCustomClass.GetValueOrDefault()); } set { if (deviceUseCustomClass != value) { deviceUseCustomClass = value; OnPropertyChanged(value); } } }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        //{ get { GetProperty(ref deviceTimeout); return (deviceTimeout.GetValueOrDefault()); } set { if (deviceTimeout != value) { deviceTimeout = value; OnPropertyChanged(value); } } }


        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// integer settings
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region integer settings

        private int? deviceAction = null;
        public int DeviceAction { get { GetProperty(ref deviceAction); return (deviceAction.GetValueOrDefault()); }
            set { if (deviceAction != value) { deviceAction = value; OnPropertyChanged(value); } } }


        private int? svcTimeout = null;
        public int SvcTimeout
        {
            get
            {
                GetProperty(ref svcTimeout);
                int val = (svcTimeout.GetValueOrDefault());
                return val;
            }
            set
            {
                Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.SvcTimeout = TimeSpan.FromSeconds(value);
                if (svcTimeout != value) { svcTimeout = value; OnPropertyChanged(value); }
            }
        }

        private int? deviceTimeout = null;
        public int DeviceTimeout
        {
            get
            {
                GetProperty(ref deviceTimeout);
                int val = (deviceTimeout.GetValueOrDefault());
                return val;
            }
            set
            {
                Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromSeconds(value);
                if (deviceTimeout != value) { deviceTimeout = value; OnPropertyChanged(value); }
            }
        }


        private int? telemetryDelay = null;
        public int TelemetryDelay
        {
            get
            {
                GetProperty(ref telemetryDelay);
                int val = telemetryDelay.GetValueOrDefault();
                return val;
            }
            set
            {
                Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings = value;
                if (telemetryDelay != value) { telemetryDelay = value; OnPropertyChanged(value); }
            }
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// More bool settings
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region More bool settings
        private bool? autoStartSvc = null;
        public bool AutoStartSvc { get { GetProperty(ref autoStartSvc); return (autoStartSvc.GetValueOrDefault()); }
            set {
                if (autoStartSvc != value) { autoStartSvc = value; OnPropertyChanged(value); } } }


        private bool? keepAliveingSvc = null;
        public bool KeepAliveSvc { get { GetProperty(ref keepAliveingSvc); return (keepAliveingSvc.GetValueOrDefault()); }
            set {
                if (keepAliveingSvc != value) { keepAliveingSvc = value; OnPropertyChanged(value); } } }

        private bool? expectResponse = null;
        public bool ExpectResponse { get { GetProperty(ref expectResponse); return (expectResponse.GetValueOrDefault()); }
            set {
                if (expectResponse != value) { expectResponse = value; OnPropertyChanged(value); } } }


        private bool? autoStartDevice = null;
        public bool AutoStartDevice { get { GetProperty(ref autoStartDevice); return (autoStartDevice.GetValueOrDefault());  }
            set {
                if (autoStartDevice != value) { autoStartDevice = value; OnPropertyChanged(value); } } }


        private bool? keepDeviceListening = null;
        public  bool KeepDeviceListening { get { GetProperty(ref keepDeviceListening); return (keepDeviceListening.GetValueOrDefault()); }
            set {
                if (keepDeviceListening != value) { keepDeviceListening = value; OnPropertyChanged(value); } } }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generic setting get and changed handlers
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////
        #region Generic setting get and changed handlers
        private void GetProperty<T>(ref T value,
            [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (value == null)
            {
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.Keys.Contains(propertyName))
                {
                    T temp = value;
                    //if (localSettings.Values[propertyName].GetType() == Nullable.GetUnderlyingType(value.GetType()))
                    try
                    {
                        value = (T)localSettings.Values[propertyName];
                    }
                    catch (Exception)
                    {
                        //If setting is wrong delete it and restore existing value
                        localSettings.Values.Remove(propertyName);
                        value = temp;
                    }
                }
            }
        }

        public  event PropertyChangedEventHandler PropertyChanged;

        private   void OnPropertyChanged(object value,
        [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains(propertyName))
            {
                localSettings.Values.Remove(propertyName);
            }
            if (!localSettings.Values.Keys.Contains(propertyName))
                localSettings.Values.Add(propertyName, value);

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));

                switch (propertyName)
                {

                }
            }
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


}
