// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub service SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/service
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace back_end_application
{
    class BackEndApplication
    {
        private static ServiceClient s_serviceClient;
        
        // Connection string for your IoT Hub
        // az iot hub show-connection-string --hub-name {your iot hub name}
        private readonly static string s_connectionString = 
        "HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=BuPhIaEYb7S/FK9ojoDqxi8jyUhCttokrcXDTJGwoNI=";
        //"HostName=MyNewHub2.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7KZtl2QDsaiS7of40G56f7Ib27SQ5gx3XAmQkAN0Kks=";
        //"{Your service connection string here}";

        // Invoke the direct method on the device, passing the payload
        private static async Task InvokeMethod()
        {
            var methodInvocation = new CloudToDeviceMethod("SetTelemetryInterval") { ResponseTimeout = TimeSpan.FromSeconds(299) };
            methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(200);
            methodInvocation.SetPayloadJson("10");

 Console.WriteLine(" s_serviceClient.InvokeDeviceMethodAsync");
            // Invoke the direct method asynchronously and get the response from the simulated device.
            var response = await s_serviceClient.InvokeDeviceMethodAsync("MyNewDevice2", methodInvocation);

            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
        }

        private static void Mess(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #2 - Back-end application.\n");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            //// Create a ServiceClient to communicate with service-facing endpoint on your hub.
            //s_serviceClient = ServiceClient.CreateFromConnectionString(s_connectionString);
            //InvokeMethod().GetAwaiter().GetResult();
            Azure_IoTHub_Telemetry.BackEndApplication.Run(5,30,30,1,Mess).GetAwaiter().GetResult();
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
