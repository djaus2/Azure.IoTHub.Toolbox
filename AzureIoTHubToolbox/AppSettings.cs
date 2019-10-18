using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.Storage.Provider;
//Orig
namespace Azure_IoTHub_Toolbox_App
{
    // Class to save and load an app's settings that are public properties an sttaic class to the app's LocalSettings.
    // The static class is IoTHubConnectionDetails.
    // The class properties are saved as name-value pairs in a ApplicationDataCompositeValue instance called ComSettings
    public static class ApplicationSettings
    {

        // Load the ComDetails object from the application's local settings as an ApplicationDataCompositeValue instance.
        // Iterate through the properties in the static class of current app settings, that are in a static class.
        // If a property is in the ComDetails keys, assign the value for that key as in ComDetail, to the static class property.
        public static  void LoadConSettings()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains("ConDetail"))
            {
                Windows.Storage.ApplicationDataCompositeValue composite =
                        (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["ConDetail"];
                if (composite != null)
                {
                    //Ref: https://stackoverflow.com/questions/9404523/set-property-value-using-property-name
                    Type type = typeof(Pages.IoTHubConnectionDetails); // IoTHubConnectionDetails is static class with public static properties
                    foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                    {
                        string propertyName = property.Name;
                        if (composite.Keys.Contains(propertyName))
                        {
                            //Want to implement Cons.propertyName = composite[propertyName];
                            var propertyInfo = type.GetProperty(propertyName); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            propertyInfo.SetValue(type, composite[propertyName], null);
                        }
                    }
                }
            }
            else
            {
                //Pages.IoTHubConnectionDetails.
            }
        }

        public static void LoadMyConnections()
        {

            Type type = typeof(AppSettingsValues); // IoTHubConnectionDetails is static class with public static properties
            foreach (var property in type.GetProperties()) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
            {
                string propertyName = property.Name;
                if (propertyName == "Settings")
                    continue;
                var val = property.GetValue(AppSettingsValues.Settings); // static classes cannot be instanced, so use null...

                Type type2 = typeof(Azure_IoTHub_Connections.MyConnections); // IoTHubConnectionDetails is static class with public static properties
                foreach (var property2 in type2.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                {
                    string propertyName2 = property2.Name;
                    if (propertyName2 == propertyName)
                    {
                       
                        var propertyInfo = type2.GetProperty(propertyName2); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        var info = propertyInfo.GetValue(type2, null);
                        propertyInfo.SetValue(type2, val, null);
                    }
                }
            }
        }

        public static void SaveMyConnections()
        {

            Type type2 = typeof(Azure_IoTHub_Connections.MyConnections); // IoTHubConnectionDetails is static class with public static properties
            foreach (var property2 in type2.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
            {
                string propertyName2 = property2.Name;
                var propertyInfo = type2.GetProperty(propertyName2); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var info = propertyInfo.GetValue(type2, null);

                Type type = typeof(AppSettingsValues); // IoTHubConnectionDetails is static class with public static properties
                foreach (var property in type.GetProperties()) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                {
                    string propertyName = property.Name;
                    if (propertyName == "Settings")
                        continue;
                    if (propertyName2 == propertyName)
                    {

                        var propertyInfo2 = type.GetProperty(propertyName); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        var info2 = propertyInfo2.GetValue(AppSettingsValues.Settings);
                        propertyInfo2.SetValue(AppSettingsValues.Settings,info);
                    }
                }
            }
        }

        public static async Task<string> SaveMyConnectionsToFile(string filename)
        {
            try
            {
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.Keys.Contains("ConDetail"))
                {
                    Windows.Storage.ApplicationDataCompositeValue composite =
                           (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["ConDetail"];
                    if (composite != null)
                    {
                        string cons = JsonConvert.SerializeObject(composite);
                        FileSavePicker filePicker = new FileSavePicker();
                        filePicker.SuggestedStartLocation = PickerLocationId.Downloads;
                        filePicker.FileTypeChoices.Add("Connections", new List<string>() { ".con", ".json", ".txt" });
                        // Default file name if the user does not type one in or select a file to replace
                        filePicker.SuggestedFileName = filename;

                        StorageFile file = await filePicker.PickSaveFileAsync();
                        if (file != null)
                        {
                            // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                            CachedFileManager.DeferUpdates(file);
                            // write to file
                            await FileIO.WriteTextAsync(file, cons);
                            // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                            // Completing updates may require Windows to ask for user input.
                            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                            if (status == FileUpdateStatus.Complete)
                            {
                                return "Save Connection File " + file.Name + " was saved.";
                            }
                            else
                            {
                                return "Save Connection File " + file.Name + " couldn't be saved.";
                            }
                        }
                        else
                        {
                            return "Save Connection Operation cancelled.";
                        }

                    }
                    return "Save Connection Operation  not completed.";
                }
                return "Save Connection Operation  not completed.";
            } catch (Exception ex)
            {
                return ("Save Connection Operation failed: " + ex.Message);
            }

        }

        public static void NewConSettings()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains("ConDetail"))
            {
                localSettings.Values.Remove("ConDetail");
                LoadConSettings();
            }
        }
        public static async Task<string> LoadMyConnectionsFromFile()
        {
            try { 
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                openPicker.FileTypeFilter.Add(".con");
                openPicker.FileTypeFilter.Add(".json");
                openPicker.FileTypeFilter.Add(".txt");

                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    string json = await FileIO.ReadTextAsync(file);
                    var composite = JsonConvert.DeserializeObject<Windows.Storage.ApplicationDataCompositeValue>(json);
                    if (composite != null)
                    {
                        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        if (localSettings.Values.Keys.Contains("ConDetail"))
                        {
                            localSettings.Values.Remove("ConDetail");
                        }
                        localSettings.Values.Add("ConDetail", composite);
                        LoadConSettings();
                        return "Loaded Hub Connection from: " + file.Name;
                    }
                    else
                        return "Load Hub Connection Operation failed from: " + file.Name;
                }
                else
                {
                    return "Load Hub Connection Operation cancelled.";
                }
            } catch (Exception ex)
            {
                return ("Load Hub Connection Operation failed: " + ex.Message);
            }
        }

            // Create a new instance of ApplicationDataCompositeValue object as ComDetail
            // Iterate through the properties of a static class and store each name value pair in a ComDetail
            // Save that to the application's local settings, replacing the existing object if it exists.
        public static void SaveSettingsToAppData()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains("ConDetail"))
            {
                localSettings.Values.Remove("ConDetail");
            }
            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();

            //Ref: https://stackoverflow.com/questions/12480279/iterate-through-properties-of-static-class-to-populate-list
            Type type = typeof(Pages.IoTHubConnectionDetails); // IoTHubConnectionDetails is static class with public static properties
            foreach (var property in type.GetProperties()) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
            {
                string propertyName = property.Name;
                var val = property.GetValue(null); // static classes cannot be instanced, so use null...

                System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", propertyName, val));
                composite[propertyName] = val;
            }
            localSettings.Values.Add("ConDetail", composite);

            if (localSettings.Values.Keys.Contains("AppSettingsValues"))
            {
                localSettings.Values.Remove("AppSettingsValues");
            }
        }

        public static string SaveSettingsToConsoleDeviceAppSettings()
        {

            var settings = new Settings();

            Type type = typeof(AppSettingsValues); // IoTHubConnectionDetails is static class with public static properties
            foreach (var property in type.GetProperties()) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
            {
                string propertyName = property.Name;
                if (propertyName == "Settings")
                    continue;
                var val = property.GetValue(AppSettingsValues.Settings); // static classes cannot be instanced, so use null...

                
                Type type2 = typeof(Settings); // IoTHubConnectionDetails is static class with public static properties
                foreach (var property2 in type2.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                {
                    string propertyName2 = property2.Name;
                    if (propertyName2 == propertyName)
                    {

                        var propertyInfo2 = type.GetProperty(propertyName); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        var info2 = propertyInfo2.GetValue(settings);
                        propertyInfo2.SetValue(settings, info2);
                    }
                }
                
            }
            settings.AutoStartDevice = true;
            settings.KeepDeviceListening = true;
            settings.iothub_cs = Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString;
            settings.device_cs = Azure_IoTHub_Connections.MyConnections.DeviceConnectionString;
            settings.device_id = Azure_IoTHub_Connections.MyConnections.DeviceId;
            settings.DeviceTimeout = Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.DeviceTimeout.Seconds;
            settings.DeviceAction = Azure_IoTHub_Connections.MyConnections.DeviceAction;
            string str = JsonConvert.SerializeObject(settings);
            return str;
        }

    }
}
