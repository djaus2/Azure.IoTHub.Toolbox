﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Azure_IoTHub_Telemetry;


namespace Azure_IoTHub_Telemetry
{

    public class SimulatedDevice
    {
        public delegate void ActionReceivedText(string recvTxt);

        private static DeviceClient s_deviceClient;
        public static bool ControlDeviceMode { get; set; } = false;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private static string s_connectionString = "{Your device connection string here}";

        public static bool ContinueLoop {get; set;}=false;

        public static string MessageString { get; set; } = "";

        public static Microsoft.Azure.Devices.Client.Message Message = null;
        public static string IOTMess { get; set; } = "";

        // Async method to send simulated telemetry
        private static async Task SendDeviceToCloudMessagesAsync()
        {
            ContinueLoop = true;
            OnDeviceStatusUpdate?.Invoke("IoT Hub Telemetry - Device sending messages.");
            while (ContinueLoop)
            {
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

                

                System.Diagnostics.Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, MessageString);
                SetDeviceSentMsg?.Invoke(string.Format("{0} > Sending message: {1}", DateTime.Now, MessageString));
                
                // Send the telemetry message
                if (!IsDeviceStreaming)
                {
                    await s_deviceClient.SendEventAsync(Message);
                    Delay = 1000 * Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings;
                  
                    await Task.Delay(Delay);
                    if (!ContinueLoop)
                        OnDeviceStatusUpdate?.Invoke("Cancelled Telemetry - Device end");
                }
                else
                {
                    ContinueLoop= false;
                }
                
            }
        }

        private static int Delay = Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings;

        private static bool IsDeviceStreaming = false;

        public static bool IsConfigured { get; set; } = false;

        private static ActionReceivedText SetDeviceSentMsg;
        private static ActionReceivedText OnDeviceStatusUpdate;


        public static void Configure(string device_cs, bool isDeviceStreaming, 
            TransportType transportType, bool loop=true)
        {

            IsDeviceStreaming = isDeviceStreaming;

            s_connectionString = device_cs;

            if (!IsDeviceStreaming)
            {
                s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, transportType);
            }
            ContinueLoop = loop;
        }

        private static int s_telemetryInterval = 1; // Seconds

        // Handle the direct method call
        public static Task<MethodResponse> SetTelemetryInterval(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);

            // Check the payload is a single integer value
            if (Int32.TryParse(data, out s_telemetryInterval))
            {
                Azure_IoTHub_Connections.MyConnections.TelemetryDelayBtwReadings = s_telemetryInterval;
                //Console.ForegroundColor = ConsoleColor.Green;
                System.Diagnostics.Debug.WriteLine("Telemetry interval set to {0} seconds", data);
                //Console.ResetColor();

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


        public static async Task<string>  Run(ActionReceivedText onDeviceStatusUpdateD  = null, ActionReceivedText setDeviceSentMsg = null)
        {
            SetDeviceSentMsg = setDeviceSentMsg;
            OnDeviceStatusUpdate = onDeviceStatusUpdateD;

            if (Azure_IoTHub_Sensors.Weather.CurrentWeather == null)
                Azure_IoTHub_Sensors.Weather.CurrentWeather = new Azure_IoTHub_Sensors.Weather_Random();
             MessageString = "";
            System.Diagnostics.Debug.WriteLine("IoT Hub Telemetry - Simulated device started.");
            OnDeviceStatusUpdate?.Invoke("IoT Hub Telemetry - Simulated device started.");
            // Connect to the IoT hub using the MQTT protocol
            if (ControlDeviceMode)
            {
                // Create a handler for the direct method call
                try
                {
                    await s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryInterval, null);//.Wait();
                } catch (Exception ex)
                {

                }
            }
            await SendDeviceToCloudMessagesAsync();
            if (!IsDeviceStreaming)
            {
                System.Diagnostics.Debug.WriteLine("IoT Hub Telemetry - Device done");
                OnDeviceStatusUpdate?.Invoke("IoT Hub Telemetry - Device done");
                await s_deviceClient.CloseAsync();
                MessageString = "";
            }

            return MessageString;
        }
    }
}

