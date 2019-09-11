using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azure_IoTHub_DeviceStreaming;
using System.Threading.Tasks;
using Azure_IoTHub_Telemetry;
using Newtonsoft.Json;

namespace DeviceDNCoreApp
{
    public  class Settings
    {
        public static Settings _Settings;

        public int runMode { get; set; }
        public  int waitAtEndOfConsoleAppSecs { get; set; } 
        public  int timeout { get; set; }

        public  int DeviceAction { get; set; }

        public  bool basicMode { get; set; } 
        public  bool UseCustomClass { get; set; }
        public  bool ResponseExpected { get; set; }
        public  bool KeepAlive { get; set; } 

        public  string service_cs { get; set; } 
        public  string device_id { get; set; }
        public  string device_cs { get; set; }  

        public  bool KeepDeviceListening { get; set; } 

        //The next is superfulous as this device app will always autostart.
        public  bool AutoStartDevice { get; set; } 


    }

    public static class Program
    {
 

        public static int Main(string[] args)
        {
           
            Console.WriteLine("Device starting.\n");

            //Type type2 = typeof(Settings); // IoTHubConnectionDetails is static class with public static properties
            //var TypeBlob = type2.GetProperties().ToDictionary(x => x.Name, x => x.GetValue(null));
            //string str =  JsonConvert.SerializeObject(TypeBlob);

            string str = System.IO.File.ReadAllText("Settings.json");

            Settings._Settings = JsonConvert.DeserializeObject<Settings>(str);
            //foreach (var x in settings)
            //{
            //    string name = x.Name;
            //    string value = x.Value.ToString();
            //    Type type = typeof(Settings); // IoTHubConnectionDetails is static class with public static properties
            //    foreach (var property in type.GetProperties())//System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            //    {
            //        string propertyName = property.Name;
            //        if (propertyName == "Settings")
            //            continue;
            //        if (name == propertyName)
            //        {

            //            var propertyInfo2 = type.GetProperty(propertyName); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            //            var xyyy = propertyInfo2.PropertyType;
            //            if (xyyy == "1".GetType())
            //            {
            //                propertyInfo2.SetValue(x.Value.ToString(), null);
            //            }
            //            else if (xyyy == true.GetType())
            //            {
            //                propertyInfo2.SetValue((bool)x.Value, null);
            //            }
            //            else if (xyyy== 1.GetType())
            //            {
            //                propertyInfo2.SetValue((int)x.Value, null);
            //            }


            //            var info = propertyInfo2.GetValue(type, null);
            //        }
            //    }

            //}

            //var sdf = settings["SKU"].ToString();
            switch (Settings._Settings.runMode)
            {
                case 1:
                    //Device Streaming
                    RunDevice(Settings._Settings.device_cs, Settings._Settings.timeout);
                    break;
                case 2:
                    //Telemetry
                    break;
                case 3:
                    ///Control Device
                    break;
                case 4:
                    //RPI IoT-Core Sensors
                    break;
                case 5:
                    //RPI  Raspbian Sensors
                    break;
            }

            Console.WriteLine(string.Format("Device Done.\n\nApp will close in {0} seconds.\n", Settings._Settings.waitAtEndOfConsoleAppSecs));

            TimeSpan ts = TimeSpan.FromSeconds(Settings._Settings.waitAtEndOfConsoleAppSecs);
            Task.Delay(ts).GetAwaiter().GetResult();
            return 0;
        }

        private static string OnrecvTextIO(string msgIn)
        {
            Console.WriteLine(msgIn);
            string msgOut = msgIn.ToUpper();
            Console.WriteLine(msgOut);
            return msgOut;
        }

        private static string OnDeviceRecvTextIO(string msgIn, out Microsoft.Azure.Devices.Client.Message message)
        {
            message = null;
            //Perform device side processing here. Eg read sensors.
            string msgOut = msgIn;
            switch (Settings._Settings.DeviceAction)
            {
                case 0:
                    msgOut = msgIn;
                    break;
                case 1:
                    msgOut = msgIn.ToUpper();
                    break;
                case 2:
                    switch (msgIn.Substring(0, 3).ToLower())
                    {
                        case "tem":
                            msgOut = "45 C";
                            break;
                        case "pre":
                            msgOut = "1034.0 hPa";
                            break;
                        case "hum":
                            msgOut = "67%";
                            break;
                        default:
                            msgOut = "Invalid request";
                            break;
                    }
                    break;
                case 3:
                    //Azure_IoTHub_Telemetry.SimulatedDevice.Configure(
                    //    Azure_IoTHub_Connections.MyConnections.DeviceConnectionString, true,
                    //    Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType, false);
 
                    //msgOut = Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.DeviceInSimuatedDeviceModeStrn + Azure_IoTHub_Telemetry.SimulatedDevice.Run().GetAwaiter().GetResult();
                    //message = SimulatedDevice.Message;
          
                    string []msg = msgIn.Split(new char[] { '-', ' ' });
                    if (((msg.Length > 1) && (msg[0].ToLower() == "get")) || (msg.Length > 0))
                    {
                        switch (msg[0].ToLower().Substring(0, 3))
                        {
                            case "set":
                                if (int.TryParse(msg[1], out int index))
                                {
                                    msgOut = "OK";
                                    switch (index)
                                    {
                                        case 0:
                                            Azure_IoTHub_Sensors.Weather.CurrentWeather = null;
                                            msgOut = "Cleared";
                                            break;
                                        case 1:
                                            Azure_IoTHub_Sensors.Weather.CurrentWeather = new Azure_IoTHub_Sensors.Weather_Fixed();
                                            break;
                                        case 2:
                                            Azure_IoTHub_Sensors.Weather.CurrentWeather = new Azure_IoTHub_Sensors.Weather_Random();
                                            break;
                                        case 3:
                                            Azure_IoTHub_Sensors.Weather.CurrentWeather = new Azure_IoTHub_Sensors.Weather_FromCities();
                                            break;
                                        case 4:
                                            Azure_IoTHub_Sensors.Weather.CurrentWeather = new Azure_IoTHub_Sensors.Weather_FromHardware();
                                            break;
                                        default:
                                            Azure_IoTHub_Sensors.Weather.CurrentWeather = null;
                                            msgOut = "Invalid option. Cleared.";
                                            break;
                                    }
                                }
                                break;
                            case "get":
                                msgOut = Azure_IoTHub_Sensors.TelemetryDataPoint.Prefix +
                                    Azure_IoTHub_Sensors.Weather.CurrentWeather?.ToString();
                                break;
                            default:
                                msgOut = "Help\r\nset 0|1|2|3|4 Choose the weather class.\r\n0=clear,1=Fixed values,2=random values,\r\n3=Rotating from cities,4=From Arduino device\r\nget Get the weather from the weather class chosen";
                                break;
                        }
                    }
                    else
                        msgOut = "try " + msgIn + " help";
                    //message = Azure_IoTHub_Telemetry.SimulatedDevice.Message;
                    break;

                    break;
                case 4:
                    msgOut = "Coming. Not yet implemented. This is a pace holder for now.";
                    break;
            }

            Console.WriteLine(msgIn);
            Console.WriteLine(msgOut);

            System.Diagnostics.Debug.WriteLine(msgIn);
            System.Diagnostics.Debug.WriteLine(msgOut);
            return msgOut;
        }

        private static void OnDeviceStatusUpdate(string recvTxt)
        {
            //AppendMsg += recvTxt +"\r\n";
            System.Diagnostics.Debug.WriteLine(recvTxt);
            Console.WriteLine(recvTxt);
        }


        private static void ActionCommand(bool flag, string msg, int al, int cmd)
        {
            switch (cmd)
            {
                case 0:
                    //if (chkAutoStart.IsChecked != isChecked)
                    //    chkAutoStart.IsChecked = isChecked;
                    break;
                case 1:
                    //if (chKeepDeviceListening.IsChecked != isChecked)
                    //    chKeepDeviceListening.IsChecked = isChecked;
                    break;
            }
        }


        private static void RunDevice(string device_cs, double ts)
        {
            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromSeconds(ts);
 
            try
            {
                if (Settings._Settings.basicMode)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO).GetAwaiter().GetResult();
                else if (!Settings._Settings.UseCustomClass)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, Settings._Settings.KeepDeviceListening).GetAwaiter().GetResult();
                else
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, Settings._Settings.KeepDeviceListening, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Hub connection failure");
            //}
            //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Device not found");
            //}
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Timeout");
                }
            }        
        }
    }
}
