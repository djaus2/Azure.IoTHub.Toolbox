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

        public static bool ContinueLoop { get; set; } = false;

        public static string MessageString { get; set; } = "";

        public static Microsoft.Azure.Devices.Client.Message Message = null;
        public static string IOTMess { get; set; } = "";
        // Async method to send simulated telemetry
        private static async Task SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            //double minTemperature = 20;
            //double minHumidity = 60;
            //Random rand = new Random();
            OnDeviceStatusUpdateD?.Invoke("IoT Hub Telemetry #2- Device sending messages.");
            ContinueLoop = true;
            while (ContinueLoop)
            {
                //double currentTemperature = minTemperature + rand.NextDouble() * 15;
                //double currentHumidity = minHumidity + rand.NextDouble() * 20;

                //// Create JSON message
                //var telemetryDataPoint = new
                //{
                //    temperature = currentTemperature,
                //    humidity = currentHumidity
                //};
                //var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                //var message = new Message(Encoding.ASCII.GetBytes(messageString));

                //// Add a custom application property to the message.
                //// An IoT hub can filter on these properties without access to the message body.
                //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");
                //await s_deviceClient.SendEventAsync(message);


                Azure_IoTHub_Sensors.TelemetryDataPoint telemetryDataPoint;
               
                if (Azure_IoTHub_Sensors.Weather.CurrentWeather.DoAsync)
                    telemetryDataPoint = await Azure_IoTHub_Sensors.Weather.CurrentWeather.GetWeatherAsync();
                else
                    telemetryDataPoint = Azure_IoTHub_Sensors.Weather.CurrentWeather.GetWeather();

                MessageString = JsonConvert.SerializeObject(telemetryDataPoint);

                Message = new Message(Encoding.ASCII.GetBytes(MessageString));
                //Message.UserId = Azure_IoTHub_Connections.MyConnections.IoTHubName;
                Message.Properties.Add("temperatureAlert", (telemetryDataPoint.temperature > 30) ? "true" : "false");
                Message.Properties.Add("humidityAlert", (telemetryDataPoint.humidity > 80) ? "true" : "false");
                Message.Properties.Add("pressureAlert", (telemetryDataPoint.pressure > 1010) ? "true" : "false");
                Azure_IoTHub_Telemetry.SyntheticIoTMessage iotmessage = new Azure_IoTHub_Telemetry.SyntheticIoTMessage(Message);
                MessageString = iotmessage.Serialise();

                SetDeviceSentMsg?.Invoke(string.Format("{0} > Sending message: {1}", DateTime.Now, MessageString));



                // Send the tlemetry message
                await s_deviceClient.SendEventAsync(Message);
                SetDeviceSentMsg?.Invoke(string.Format("{0} > Sending message: {1}", DateTime.Now, MessageString));

                s_telemetryInterval = Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings;
                await Task.Delay(s_telemetryInterval * 1000);
                if (!ContinueLoop)
                    OnDeviceStatusUpdateD?.Invoke("Cancelled Telemetry - Device end");
            }
        }

        private static ActionReceivedText SetDeviceSentMsg = null;
        private static ActionReceivedText OnDeviceStatusUpdateD = null;
        public static async Task Run(int delay, int timeout, int tag, ActionReceivedText setDeviceSentMsg, ActionReceivedText OnDeviceStatusUpdate)
        {
            SetDeviceSentMsg = setDeviceSentMsg;

            if (Azure_IoTHub_Sensors.Weather.CurrentWeather == null)
                Azure_IoTHub_Sensors.Weather.CurrentWeather = new Azure_IoTHub_Sensors.Weather_Random();

            
            OnDeviceStatusUpdateD = OnDeviceStatusUpdate;
            OnDeviceStatusUpdateD?.Invoke("IoT Hub Telemetry #2 - Simulated device. ");
            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);

            // Create a handler for the direct method call
            await s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryInterval, null);//.Wait();
            await s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval2", SetTelemetryInterval, null);//.Wait();
            await SendDeviceToCloudMessagesAsync();
            //System.Diagnostics.Debug.ReadLine();
            OnDeviceStatusUpdateD?.Invoke("IoT Hub Telemetry #2 - Device done");
        }
    }
}
