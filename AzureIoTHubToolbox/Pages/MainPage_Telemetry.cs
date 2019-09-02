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

    public sealed partial class MainPage : Page
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


    }
}
