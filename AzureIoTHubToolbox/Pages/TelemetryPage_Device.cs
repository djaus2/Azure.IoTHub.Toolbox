using Azure_IoTHub_DeviceStreaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Azure_IoTHub_Toolbox_App.Pages
{
    sealed partial class TelemetryPage : Page
    {
        int state = -1;
        bool toggle = false;

        private string OnDeviceRecvTextIO(string msgIn, out Microsoft.Azure.Devices.Client.Message message )
        {

            message = null;
            //Perform device side processing here. Eg read sensors.
            string msgOut = msgIn;
            switch (DeviceAction)
            {
                case 0:
                    msgOut = msgIn;
                    break;
                case 1:
                    msgOut = msgIn.ToUpper();
                    break;
                case 2:
                    string[] msg = msgIn.Split(new char[] { '-', ' ' });
                    if ((msg.Length > 1) || (msg[0].ToLower() == "help"))
                    {
                        switch (msg[0].ToLower().Substring(0, 3))
                        {
                            case "set":
                                msgOut = "Invalid request. Try Help";
                                if (msg.Length > 2)
                                {
                                    if (int.TryParse(msg[2], out int val))
                                    {
                                        switch (msg[1].Substring(0, 3).ToLower())
                                        {
                                            case "sta":
                                                state = val;
                                                msgOut = "setVal: OK";
                                                break;
                                            case "tog":
                                                toggle = val > 0 ? true : false;
                                                msgOut = "setVal: OK";
                                                break;
                                            default:
                                                msgOut = "Invalid request. Try Help";
                                                break;
                                        }
                                    }
                                    else if (bool.TryParse(msg[2], out bool bval))
                                    {
                                        switch (msg[1].Substring(0, 3).ToLower())
                                        {
                                            case "tog":
                                                toggle = bval;
                                                msgOut = "setbVal: OK";
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "get":
                                switch (msg[1].ToLower().Substring(0,3))
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
                                    case "sta":
                                        msgOut = string.Format("state = {0}", state);
                                        break;
                                    case "tog":
                                        msgOut = string.Format("toggle = {0}", toggle);
                                        break;
                                    default:
                                        msgOut = "Invalid request. Try Help";
                                        break;
                                }
                                break;
                            case "hel":
                                msgOut =   "Only first three characters of each word required.\r\nget: temperature,pressure,humidity,state,toggle,help\r\nset :state <int value>,toggle <0|1> (true|false)";
                                break;
                            default:
                                msgOut = "Invalid request. Try Help";
                                break;
                        }
                    }
                    else
                        msgOut = "try " + msgIn + " help";
                    break;
                case 21: //Old 2 case
                    switch (msgIn.Substring(0,3).ToLower())
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
                    msg = msgIn.Split(new char[] { '-', ' ' });
                    if (((msg.Length > 1) && (msg[0].ToLower() == "get")) || ( msg.Length>0))
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
                case 4:
                    msgOut = "Not implemented for desktop.\r\nTry with Win 10 IoT-Core (eg RPI) running UWP_BGAppAzDeviceStream_Device, as in GitHub Repository:\r\nhttps://github.com/djaus2/AziothubDeviceStreaming";
                    break;
            }
           

            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
             
                    tbDeviceMsgOut.Text = msgOut;
                });
            });
            return msgOut;
        }

        private void UpdateDeviceOutputText(string msgOut)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbDeviceMsgOut.Text = msgOut;
                });
            });
        }


        private void OnDeviceStatusUpdate(string msgIn)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (msgIn.ToLower().Contains("Simulated Device Started".ToLower()))
                    {
                        DeviceState = deviceStates.listening;
                        DeviceIsRunningLED.Fill = MainPage.Cols[1];
                    }
                    else if (msgIn.ToLower().Contains("Device Sending Messages".ToLower()))
                    {
                        DeviceState = deviceStates.listening;
                        DeviceIsRunningLED.Fill = MainPage.Cols[0];
                    }
                    else if (msgIn.ToLower().Contains("Device end".ToLower()))
                    {
                        DeviceState = deviceStates.stopped;
                        DeviceIsRunningLED.Fill = MainPage.Cols[MainPage.Cols.Count() - 1];
                    }
                    tbDevMode.Text = ListEnum2[Azure_IoTHub_Connections.MyConnections.DeviceAction];
                    tbDeviceStatus.Text = msgIn;
                });
            });
        }

        private void ActionCommand(bool isChecked,string val, int value,  int cmd )
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    switch (cmd)
                    {
                        case 0:
                            if (chkAutoStart.IsChecked != isChecked)
                                chkAutoStart.IsChecked = isChecked;
                            break;
                        case 1:
                            //if (chKeepDeviceListening.IsChecked != isChecked)
                            //    chKeepDeviceListening.IsChecked = isChecked;
                            break;
                    }
                    
                });
            });
        }


        private void ButtonCanceLDevice_Click(object sender, RoutedEventArgs e)
        {
            DeviceStream_Device.deviceStream_Device?.Cancel();
            Azure_IoTHub_Telemetry.SimulatedDevice.ContinueLoop = false;
        }

        public bool DeviceBasicMode { get; set; } = false;
        public bool DeviceUseCustomClass { get; set; } = false;

        private async void Button_Click_Device(object sender, RoutedEventArgs e)
        {
           
            await Task.Run(() =>
            {
                try
                {
                    if (DeviceBasicMode)
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO).GetAwaiter().GetResult();
                    if (!DeviceUseCustomClass)
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, AppSettingsValues.Settings.KeepDeviceListening ).GetAwaiter().GetResult();
                    else
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, AppSettingsValues.Settings.KeepDeviceListening, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Task cancelled");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Operation cancelled");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): " + ex.Message);
                }
            });
        }




        private void ChAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            Azure_IoTHub_Toolbox_App.AppSettingsValues.Settings.AutoStartDevice = (bool)((CheckBox)sender)?.IsChecked;
            if (Azure_IoTHub_Toolbox_App.AppSettingsValues.Settings.AutoStartDevice)
                Task.Run(async () => {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //chKeepDeviceListening.IsChecked = true;
                    });
                });           
        }

        
        private void ListviewTransports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListviewTransports2.SelectedIndex != -1)
            {
                Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType = (Microsoft.Azure.Devices.Client.TransportType)ListviewTransports2.SelectedItem;
                System.Diagnostics.Debug.WriteLine(string.Format("Device Transport set to: {0}", Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType));
                tbTransport.Text = string.Format("{0}",Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType);
                DeviceProcessingModeCommands.IsOpen = false;
                OnDeviceStatusUpdate(string.Format("Device Transport set to: {0}", Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType));
            }
        }

        private int DeviceAction = 2;
        private void DeviceAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstDeviceAction.SelectedIndex != -1)
            {
                Azure_IoTHub_Connections.MyConnections.DeviceAction = LstDeviceAction.SelectedIndex;
                DeviceAction = Azure_IoTHub_Connections.MyConnections.DeviceAction;
                DeviceProcessingModeCommands.IsOpen = false;
                tbDevMode.Text = ListEnum2[Azure_IoTHub_Connections.MyConnections.DeviceAction];
                OnDeviceStatusUpdate(string.Format("Device Processing set to: {0}", ListEnum2[DeviceAction]));
                if(ListEnum2[DeviceAction] == "Sim Telemetry")
                    Azure_IoTHub_Telemetry.SimulatedDevice.Configure(Azure_IoTHub_Connections.MyConnections.DeviceConnectionString, true, Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType, false);
            }
        }


    }
}
