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
        private  static string s_connectionString =
            Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString;
        //"HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=BuPhIaEYb7S/FK9ojoDqxi8jyUhCttokrcXDTJGwoNI=";
        //"HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7KZtl2QDsaiS7of40G56f7Ib27SQ5gx3XAmQkAN0Kks=";
        //"{Your service connection string here}";

        // Invoke the direct method on the device, passing the payload
        private static async Task InvokeMethod(int DelayBetweenTelemetryReadings, int ConnectionTimeout, int MethodTimeout, int MethodTag)
        {
            CloudToDeviceMethod methodInvocation= null;
            switch (MethodTag)
            {
                case 1:
                    methodInvocation = new CloudToDeviceMethod("SetTelemetryInterval") {};
                    //Method timeout
                    methodInvocation.ResponseTimeout = TimeSpan.FromSeconds(MethodTimeout);
                    //Timeout for device to come online
                    methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(ConnectionTimeout);
                    methodInvocation.SetPayloadJson(string.Format("{0}", DelayBetweenTelemetryReadings));
                    break;
                case 2:
                    methodInvocation = new CloudToDeviceMethod("SetTelemetryInterval2") {};
                    //Method timeout
                    methodInvocation.ResponseTimeout = TimeSpan.FromSeconds(MethodTimeout);
                    //Timeout for device to come online
                    methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(ConnectionTimeout);
                    methodInvocation.SetPayloadJson(string.Format("{0}", DelayBetweenTelemetryReadings));
                    break;
                default:
                    break;
            }
            if (methodInvocation != null)
            {
                System.Diagnostics.Debug.WriteLine(" s_serviceClient.InvokeDeviceMethodAsync");
                // Invoke the direct method asynchronously and get the response from the simulated device.
                var response = await s_serviceClient.InvokeDeviceMethodAsync(Azure_IoTHub_Connections.MyConnections.DeviceId, methodInvocation);

                OnDeviceStatusUpdateD?.Invoke(string.Format("Svc Control Set Response status: {0}, payload:", response.Status));
                System.Diagnostics.Debug.WriteLine("Svc Control Set Response status: {0}, payload:", response.Status);
                string resp = response.GetPayloadAsJson();
                System.Diagnostics.Debug.WriteLine(string.Format("Svc Control Set Response: {0}",resp));
                OnDeviceStatusUpdateD?.Invoke(string.Format("Svc Control SetResponse: {0}", resp));
            }
        }

        public static async  Task  Run(int DelayBetweenTelemetryReadings, int ConnectionTimeout, int MethodTimeout, int MethodTag , ActionReceivedText OnSvcStatusUpdate)
        {
            System.Diagnostics.Debug.WriteLine("1");
            OnDeviceStatusUpdateD = OnSvcStatusUpdate;
            //Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString =
            //        "HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=BuPhIaEYb7S/FK9ojoDqxi8jyUhCttokrcXDTJGwoNI=";
             Azure_IoTHub_Connections.MyConnections.DeviceId = "MyNewDevice2";
            System.Diagnostics.Debug.WriteLine("IoT Hub Quickstarts #2 - Back-end application.\n");
            OnDeviceStatusUpdateD?.Invoke("IoT Hub Quickstarts #2 - Back-end application.");
            //System.Diagnostics.Debug.ReadLine();
            // Create a ServiceClient to communicate with service-facing endpoint on your hub.
            System.Diagnostics.Debug.WriteLine("2");
            s_connectionString =
                Azure_IoTHub_Connections.MyConnections.IoTHubConnectionString;
            s_serviceClient = ServiceClient.CreateFromConnectionString(s_connectionString);
            System.Diagnostics.Debug.WriteLine("3");
            await InvokeMethod(DelayBetweenTelemetryReadings,  ConnectionTimeout,  MethodTimeout, MethodTag);//.GetAwaiter().GetResult();
            OnDeviceStatusUpdateD?.Invoke("Backend Done");
            System.Diagnostics.Debug.WriteLine("Press Enter to exit.");
            //System.Diagnostics.Debug.ReadLine();
        }
    }
}
