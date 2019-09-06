// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub service SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/service
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using static Azure_IoTHub_Connections.MyConnections;

namespace Azure_IoTHub_Telemetry
{
    public static class BackEndApplication
    {
        private static ActionReceivedText OnDeviceStatusUpdateD = null;

        private static ServiceClient s_serviceClient;

        // Connection string for your IoT Hub
        // az iot hub show-connection-string --hub-name {your iot hub name}
        private readonly static string s_connectionString =
            Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString;
        //"HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=BuPhIaEYb7S/FK9ojoDqxi8jyUhCttokrcXDTJGwoNI=";
        //"HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7KZtl2QDsaiS7of40G56f7Ib27SQ5gx3XAmQkAN0Kks=";
        //"{Your service connection string here}";

        // Invoke the direct method on the device, passing the payload
        private static async Task InvokeMethod(int delay, int timeout)
        {
            var methodInvocation = new CloudToDeviceMethod("SetTelemetryInterval") { ResponseTimeout = TimeSpan.FromSeconds(299) };
            methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(timeout);
            methodInvocation.SetPayloadJson(string.Format("{0}",delay));

 System.Diagnostics.Debug.WriteLine(" s_serviceClient.InvokeDeviceMethodAsync");
            // Invoke the direct method asynchronously and get the response from the simulated device.
            var response = await s_serviceClient.InvokeDeviceMethodAsync(Azure_IoTHub_Connections.MyConnections.DeviceId, methodInvocation);

            OnDeviceStatusUpdateD?.Invoke(string.Format("Response status: {0}, payload:", response.Status));
            System.Diagnostics.Debug.WriteLine("Response status: {0}, payload:", response.Status);
            string resp = response.GetPayloadAsJson();
            System.Diagnostics.Debug.WriteLine(resp);
            OnDeviceStatusUpdateD?.Invoke(resp);
        }

        public static async  Task  Run(int delay, int timeout, ActionReceivedText OnSvcStatusUpdate)
        {
            OnDeviceStatusUpdateD = OnSvcStatusUpdate;
            Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString =
                    "HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=BuPhIaEYb7S/FK9ojoDqxi8jyUhCttokrcXDTJGwoNI=";
             Azure_IoTHub_Connections.MyConnections.DeviceId = "MyNewDevice2";
            System.Diagnostics.Debug.WriteLine("IoT Hub Quickstarts #2 - Back-end application.\n");
            OnDeviceStatusUpdateD?.Invoke("IoT Hub Quickstarts #2 - Back-end application.");
            System.Diagnostics.Debug.WriteLine("Press Enter to continue.");
            //System.Diagnostics.Debug.ReadLine();
            // Create a ServiceClient to communicate with service-facing endpoint on your hub.
            s_serviceClient = ServiceClient.CreateFromConnectionString(s_connectionString);
            await InvokeMethod(delay, timeout);//.GetAwaiter().GetResult();
            OnDeviceStatusUpdateD?.Invoke("Backend Done");
            System.Diagnostics.Debug.WriteLine("Press Enter to exit.");
            //System.Diagnostics.Debug.ReadLine();
        }
    }
}
