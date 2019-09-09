// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using static Azure_IoTHub_Connections.MyConnections;

namespace Azure_IoTHub_Telemetry
{
    public static class SimulatedDevice_2
    {
        private static DeviceClient s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private readonly static string s_connectionString =
        "HostName=MyNewHub2.azure-devices.net;DeviceId=MyNewDevice2;SharedAccessKey=uIPA9ftcG4F5aPI/WTD3CDUDt7BuT0AEYCW0aMAIn2o=";
        //"HostName=MyNewHub2.azure-devices.net;DeviceId=MyNewDevice2;SharedAccessKey=uIPA9ftcG4F5aPI/WTD3CDUDt7BuT0AEYCW0aMAIn2o=";
        //"{Your device connection string here}";

        private static int s_telemetryInterval = 1; // Seconds

        // Handle the direct method call
        private static Task<MethodResponse> SetTelemetryInterval(MethodRequest methodRequest, object userContext)
        {
            System.Diagnostics.Debug.WriteLine("1");
            System.Diagnostics.Debug.WriteLine(methodRequest.Name);
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            System.Diagnostics.Debug.WriteLine(data);
            if (methodRequest.Name == "SetTelemetryInterval")
            {
                System.Diagnostics.Debug.WriteLine("Interval");
                // Check the payload is a single integer value
                if (Int32.TryParse(data, out s_telemetryInterval))
                {
                    Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings = s_telemetryInterval;
                    System.Diagnostics.Debug.WriteLine("Telemetry interval set to {0} seconds", data);
                    OnDeviceStatusUpdateD?.Invoke(string.Format("Telemetry interval set to {0} seconds", data));

                    // Acknowlege the direct method call with a 200 success message
                    string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
                }
                else
                {
                    // Acknowlege the direct method call with a 400 error message
                    string result = "{\"result\":\"Invalid parameter\"}";
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
                }
            }
            else if (methodRequest.Name == "SetTelemetryInterval2")
            {
                System.Diagnostics.Debug.WriteLine("Toggle");
                System.Diagnostics.Debug.WriteLine("SetTelemetryInterval2 {0} seconds", data);
                OnDeviceStatusUpdateD?.Invoke(string.Format("LED Toggle {0} seconds", data));
                // Acknowlege the direct method call with a 200 success message
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Other");
                // Acknowlege the direct method call with a 400 error message
                string result = "{\"result\":\"Invalid method request\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }


        }

        //private static Task<MethodResponse> SetTelemetryInterval2(MethodRequest methodRequest, object userContext)
        //{
        //    System.Diagnostics.Debug.WriteLine("1");
        //    System.Diagnostics.Debug.WriteLine(methodRequest.Name);
        //    var data = Encoding.UTF8.GetString(methodRequest.Data);
        //    System.Diagnostics.Debug.WriteLine(data);
        //    if (methodRequest.Name == "SetTelemetryInterval")
        //    {
        //        System.Diagnostics.Debug.WriteLine("Interval");
        //        // Check the payload is a single integer value
        //        if (Int32.TryParse(data, out s_telemetryInterval))
        //        {
        //            System.Diagnostics.Debug.WriteLine("Telemetry interval set to {0} seconds", data);
        //            OnDeviceStatusUpdateD?.Invoke(string.Format("Telemetry interval set to {0} seconds", data));

        //            // Acknowlege the direct method call with a 200 success message
        //            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
        //            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        //        }
        //        else
        //        {
        //            // Acknowlege the direct method call with a 400 error message
        //            string result = "{\"result\":\"Invalid parameter\"}";
        //            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        //        }
        //    }
        //    else if (methodRequest.Name == "SetTelemetryInterval2")
        //    {
        //        System.Diagnostics.Debug.WriteLine("Toggle");
        //        System.Diagnostics.Debug.WriteLine("SetTelemetryInterval2 {0} seconds", data);
        //        OnDeviceStatusUpdateD?.Invoke(string.Format("LED Toggle {0} seconds", data));
        //        // Acknowlege the direct method call with a 200 success message
        //        string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
        //        return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        //    }
        //    else
        //    {
        //        System.Diagnostics.Debug.WriteLine("Other");
        //        // Acknowlege the direct method call with a 400 error message
        //        string result = "{\"result\":\"Invalid method request\"}";
        //        return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        //    }


        //}



        // Async method to send simulated telemetry
        private static async Task SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                // Send the tlemetry message
                await s_deviceClient.SendEventAsync(message);
                System.Diagnostics.Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                SentMsgD?.Invoke(string.Format("{0} > Sending message: {1}", DateTime.Now, messageString));

                s_telemetryInterval = Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings;
                await Task.Delay(s_telemetryInterval * 1000);
            }
        }

        private static ActionReceivedText SentMsgD = null;
        private static ActionReceivedText OnDeviceStatusUpdateD = null;
        public static async Task Run(int delay, int timeout, int tag, ActionReceivedText SentMsg, ActionReceivedText OnDeviceStatusUpdate)
        { 
            SentMsgD = SentMsg;
            OnDeviceStatusUpdateD = OnDeviceStatusUpdate;
            System.Diagnostics.Debug.WriteLine("IoT Hub Quickstarts #2 - Simulated device. Ctrl-C to exit.\n");
            OnDeviceStatusUpdateD?.Invoke("IoT Hub Quickstarts #2 - Simulated device. ");
            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);

            // Create a handler for the direct method call
            await s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryInterval, null);//.Wait();
            await s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval2", SetTelemetryInterval, null);//.Wait();
            await SendDeviceToCloudMessagesAsync();
            //System.Diagnostics.Debug.ReadLine();
            System.Diagnostics.Debug.WriteLine("IoT Hub Quickstarts #2 - Simulated device: Done.\n");
            OnDeviceStatusUpdateD?.Invoke("IoT Hub Quickstarts #2 - Simulated device: Done ");
        }
    }
}
