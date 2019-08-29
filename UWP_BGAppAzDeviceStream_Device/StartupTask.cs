using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Azure_IoTHub_DeviceStreaming;
using System.Threading.Tasks;
using Azure_IoTHub_Telemetry;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BGAppAzDeviceStream_Device
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static int waitAtEndOfConsoleAppSecs = Azure_IoTHub_Connections.MyConnections.WaitAtEndOfConsoleAppSecs;
        private static int timeout = Azure_IoTHub_Connections.MyConnections.Timeout;

        private static int DeviceAction = Azure_IoTHub_Connections.MyConnections.DeviceAction;

        private static bool basicMode = Azure_IoTHub_Connections.MyConnections.basicMode;
        private static bool UseCustomClass = Azure_IoTHub_Connections.MyConnections.UseCustomClass;
        private static bool ResponseExpected = Azure_IoTHub_Connections.MyConnections.ResponseExpected;
        private static bool KeepAlive = Azure_IoTHub_Connections.MyConnections.KeepAlive;

        private static string service_cs = Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString;
        private static string device_id = Azure_IoTHub_Connections.MyConnections.DeviceId;
        private static string device_cs = Azure_IoTHub_Connections.MyConnections.DeviceConnectionString;

        private static bool KeepDeviceListening = Azure_IoTHub_Connections.MyConnections.KeepDeviceListening;

        //The next is superfulous as this device app will always autostart.
        private static bool AutoStartDevice = Azure_IoTHub_Connections.MyConnections.AutoStartDevice;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            basicMode = false;
            UseCustomClass = false;
            KeepDeviceListening = true;

            System.Diagnostics.Debug.WriteLine("Device starting.\n");

            RunDevice(device_cs, timeout);

            System.Diagnostics.Debug.WriteLine(string.Format("Device Done.\n\nApp will close in {0} seconds.\n", waitAtEndOfConsoleAppSecs));

            TimeSpan ts = TimeSpan.FromSeconds(waitAtEndOfConsoleAppSecs);
            Task.Delay(ts).GetAwaiter().GetResult();
            return;
        }

        private  string OnrecvTextIO(string msgIn)
        {
            System.Diagnostics.Debug.WriteLine(msgIn);
            string msgOut = msgIn.ToUpper();
            System.Diagnostics.Debug.WriteLine(msgOut);
            return msgOut;
        }

        private string OnDeviceRecvTextIO(string msgIn, out Microsoft.Azure.Devices.Client.Message message)
        {
            DeviceAction = Azure_IoTHub_Connections.MyConnections.DeviceAction;
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
                    Azure_IoTHub_Telemetry.SimulatedDevice.Configure(Azure_IoTHub_Connections.MyConnections.DeviceConnectionString, true, Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType, false);
                    msgOut = Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.DeviceInSimuatedDeviceModeStrn + Azure_IoTHub_Telemetry.SimulatedDevice.Run().GetAwaiter().GetResult();
                    message = SimulatedDevice.Message;
                    break;
                case 4:
                    msgOut = "AZX";// Azure_IoTHub_Sensors.Sensors.ProcessMsgRecvdByDevice(msgIn);
                    break;
            }


            System.Diagnostics.Debug.WriteLine(msgIn);
            System.Diagnostics.Debug.WriteLine(msgOut);
            return msgOut;
        }

        private static void OnDeviceStatusUpdate(string recvTxt)
        {
            //AppendMsg += recvTxt +"\r\n";
            System.Diagnostics.Debug.WriteLine(recvTxt);
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


        private void RunDevice(string device_cs, double ts)
        {
            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromMilliseconds(ts);
 
            try
            {
                if (basicMode)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO).GetAwaiter().GetResult();
                else if (!UseCustomClass)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening).GetAwaiter().GetResult();
                else
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
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
