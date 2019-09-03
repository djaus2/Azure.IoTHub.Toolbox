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

    public sealed partial class TelemetryPage : Page
    {

        bool IsRunningTelem = false;
        private async void BtnTelemDevice_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunningTelem)
            {
                IsRunningTelem = false;
                Azure_IoTHub_Telemetry.SimulatedDevice.ContinueLoop = false;
                return;
            }
            IsRunningTelem = true;
            Azure_IoTHub_Telemetry.SimulatedDevice.Configure(Azure_IoTHub_Connections.MyConnections.DeviceConnectionString, false, Azure_IoTHub_DeviceStreaming.DeviceStreamingCommon.device_transportType, true);
            string msg = await Azure_IoTHub_Telemetry.SimulatedDevice.Run(OnDeviceStatusUpdate, TelemMsg);

        }

        private void TelemMsg(string msg)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbDeviceMsgOut.Text = msg;
                });
            });
        }

        private async void BtnTelemSvc_Click_1(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                await Azure_IoTHub_Telemetry.ReadDeviceToCloudMessages.Run(OnSvcStatusUpdate, OnSvcRecvText);
            });
        }

        private void TbTelemetryDelay_LostFocus(object sender, RoutedEventArgs e)
        {          
            if (int.TryParse(tbTelemetryDelay.Text, out int delay))
            {
                Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings = delay;
            }
        }

        enum deviceStates { stopped, listening }
        deviceStates DeviceState = deviceStates.stopped;
        deviceStates SvcState = deviceStates.stopped;
        internal void Stopping()
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (SvcState == deviceStates.listening)
                        ButtonCanceLSvc_Click(null, null);
                    if (DeviceState == deviceStates.listening)
                        ButtonCanceLDevice_Click(null, null);
                });
            });
        }

        internal static void Stop()
        {
            telemetryPage?.Stopping();
        }
    }
}
