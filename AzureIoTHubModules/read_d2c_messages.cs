﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Microsoft Azure Event Hubs Client for .NET
// For samples see: https://github.com/Azure/azure-event-hubs/tree/master/samples/DotNet
// For documenation see: https://docs.microsoft.com/azure/event-hubs/
using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace Azure_IoTHub_Telemetry
{
    public class ReadDeviceToCloudMessages
    {
        public static void Cancel()
        {
            OnSvcStatusUpdate?.Invoke("Telemetry - Cancelling");
            cts?.Cancel();
            ContinueLooping = false;
        }
        public static CancellationTokenSource cts = null;
        public static bool ContinueLooping = true;
        public delegate void ActionReceivedText(string recvTxt);
        // Event Hub-compatible endpoint
        // az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}
        //private readonly static string s_eventHubsCompatibleEndpoint = "sb://ihsuproddmres016dednamespace.servicebus.windows.net/";

        //// Event Hub-compatible name
        //// az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        //private readonly static string s_eventHubsCompatiblePath = "iothub-ehub-mynewhub-1918909-a3ba8a9102";

        //// az iot hub policy show --name service --query primaryKey --hub-name {your IoT Hub name}
        //private readonly static string s_iotHubSasKey = "Ek6Mw8PvsQQV8pdJZj+LxALA0pGB+9f0rorJKDrjfoU=";
        //private readonly static string s_iotHubSasKeyName = "service";
        private static EventHubClient s_eventHubClient;

        // Asynchronously create a PartitionReceiver for a partition and then start 
        // reading any messages sent from the simulated client.
        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            ContinueLooping = true;
            // Create the receiver using the default consumer group.
            // For the purposes of this sample, read only messages sent since 
            // the time the receiver is created. Typically, you don't want to skip any messages.
            var eventHubReceiver = s_eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(DateTime.Now));
            System.Diagnostics.Debug.WriteLine("Telemetry: Create receiver on partition: " + partition);
            OnSvcStatusUpdate?.Invoke("Telemetry: Create receiver on partition: " + partition);

            while (ContinueLooping)
            {
                if (ct.IsCancellationRequested) break;
                System.Diagnostics.Debug.WriteLine("Telemetry: Listening for messages on: " + partition);
                OnSvcStatusUpdate?.Invoke("Telemetry: Listening for messages on: " + partition);
                // Check for EventData - this methods times out if there is nothing to retrieve.
                var events = await eventHubReceiver.ReceiveAsync(100);

                // If there is data in the batch, process it.
                if (events == null) continue;

                foreach (EventData eventData in events)
                {
                    string data = Encoding.UTF8.GetString(eventData.Body.Array);
                    System.Diagnostics.Debug.WriteLine("Message received on partition {0}:", partition);
                    System.Diagnostics.Debug.WriteLine("  {0}:", data);
                    if (eventData.Properties != null)
                    {
                        if (eventData.Properties.Count != 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Application properties (set by device):");
                            foreach (var prop in eventData.Properties)
                            {
                                System.Diagnostics.Debug.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                            }
                        }
                        else
                            System.Diagnostics.Debug.WriteLine("No Application Properties (set by device)");
                    }
                    else
                        System.Diagnostics.Debug.WriteLine("No Application Properties (set by device");
                    if (eventData.SystemProperties != null)
                    {
                        if (eventData.SystemProperties.Count != 0)
                        {
                            System.Diagnostics.Debug.WriteLine("System properties (set by IoT Hub):");
                            foreach (var prop in eventData.SystemProperties)
                            {
                                System.Diagnostics.Debug.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                            }
                        }
                        else
                            System.Diagnostics.Debug.WriteLine("No System properties from IoT Hub");
                    }
                    else
                        System.Diagnostics.Debug.WriteLine("No System properties from IoT Hub");

                    OnSvcRecvText?.Invoke(Azure_IoTHub_Telemetry.SyntheticIoTMessage.EventData_ToString(eventData));
                }
            }
            OnSvcStatusUpdate?.Invoke("Telemetry: Exiting - Wait for \"All Threads Done\"");
        }

        private static ActionReceivedText OnSvcStatusUpdate = null;
        private static ActionReceivedText OnSvcRecvText = null;

        public static string GetSasKey(string ioTHubConnectionString)
        {

            var hubccs = ioTHubConnectionString;
            string[] split = hubccs.Split(new char[] { ';' });
            string saskey = "";
            foreach (var xx in split)
            {
                string[] split2 = xx.Split(new char[] { '=' });
                if (split2[0].ToLower() == "SharedAccessKey".ToLower())
                {
                    saskey = split2[1];
                    //The second split mat have removed = from end of saskey
                    if (hubccs[hubccs.Length - 1] == '=')
                        saskey += "=";
                    break;
                }

            }
            return saskey;
        }

        public static async Task Run( ActionReceivedText OnSvcStatusUpdateD = null, ActionReceivedText OnSvcRecvTextD = null)
        {
            OnSvcStatusUpdate = OnSvcStatusUpdateD;
            OnSvcRecvText = OnSvcRecvTextD;

            System.Diagnostics.Debug.WriteLine("IoT Hub Telemetry - Read device to cloud messages.");
            OnSvcStatusUpdate?.Invoke("IoT Hub Telemetry - Read device to cloud messages.");
            // Create an EventHubClient instance to connect to the
            // IoT Hub Event Hubs-compatible endpoint.
            //var connectionString1 = new EventHubsConnectionStringBuilder(new Uri(s_eventHubsCompatibleEndpoint), s_eventHubsCompatiblePath, s_iotHubSasKeyName, s_iotHubSasKey);


            //Get some of event cs properties from hub cs

            string iotHubSasKeyName  = Azure_IoTHub_Connections.MyConnections.IotHubKeyName;


            EventHubsConnectionStringBuilder EventHubConnectionString = null;

            if (Azure_IoTHub_Connections.MyConnections.EHMethod1)
                EventHubConnectionString = new EventHubsConnectionStringBuilder(Azure_IoTHub_Connections.MyConnections.EventHubsConnectionString);
            else
            EventHubConnectionString = new EventHubsConnectionStringBuilder(
                new Uri(Azure_IoTHub_Connections.MyConnections.EventHubsCompatibleEndpoint),
                Azure_IoTHub_Connections.MyConnections.EventHubsCompatiblePath,
                Azure_IoTHub_Connections.MyConnections.IotHubKeyName,
                Azure_IoTHub_Connections.MyConnections.EventHubsSasKey);


            s_eventHubClient = EventHubClient.CreateFromConnectionString(EventHubConnectionString.ToString());

            // Create a PartitionReciever for each partition on the hub.
            var runtimeInfo = await s_eventHubClient.GetRuntimeInformationAsync();
            var d2cPartitions = runtimeInfo.PartitionIds;
            cts = new CancellationTokenSource();

            //System.Diagnostics.Debug.CancelKeyPress += (s, e) =>
            //{
            //    e.Cancel = true;
            //    cts.Cancel();
            //    System.Diagnostics.Debug.WriteLine("Exiting...");
            //};

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }

            // Wait for all the PartitionReceivers to finsih.
            Task.WaitAll(tasks.ToArray());
            OnSvcStatusUpdate?.Invoke("Telemetry: All Threads Done.");
        }
    }
}
