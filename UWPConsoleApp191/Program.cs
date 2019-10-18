// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DNStandardDeviceStreaming;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Samples
{
    public static class Program
    {
        public class Settings
        {
            public static Settings _Settings;

            public string Port { get; set; }

            public int runMode { get; set; }
            public int waitAtEndOfConsoleAppSecs { get; set; }
            public int DeviceTimeout { get; set; }

            public int DeviceAction { get; set; }

            public bool basicMode { get; set; }
            public bool UseCustomClass { get; set; }
            public bool ResponseExpected { get; set; }
            public bool KeepAlive { get; set; }

            public string device_id { get; set; }
            public string device_cs { get; set; }

            public string connectionString { get; set; }

            public bool KeepDeviceListening { get; set; }

            //The next is superfulous as this device app will always autostart.
            public bool AutoStartDevice { get; set; }


        }


        private static CancellationTokenSource source = new CancellationTokenSource();
        private static CancellationToken token;

        public static int Main(string[] args)
        {
            Console.WriteLine("Device starting.\n");


            string str = System.IO.File.ReadAllText("Settings.json");

            Settings._Settings = JsonConvert.DeserializeObject<Settings>(str);


            source = new CancellationTokenSource();
            token = source.Token;
            int port = int.Parse(Settings._Settings.Port, CultureInfo.InvariantCulture);
            Microsoft.Azure.Devices.TransportType s_transportType = Microsoft.Azure.Devices.TransportType.Amqp;

            try
            {
                using (ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Settings._Settings.connectionString, s_transportType))
                {
                    DeviceStreamProxySvc sample = new DeviceStreamProxySvc(serviceClient, Settings._Settings.device_id, port, OnSvcRecvText, OnSvcStatusUpdate);
                    sample.RunSampleAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                OnSvcStatusUpdate(string.Format("Proxy Svc fail {0}", ex.Message));
            }


            Console.WriteLine("Done.\n");
            return 0;
        }

        private static void OnSvcStatusUpdate(string recvTxt)
        {
            Console.WriteLine("ProxySvc Update: " + recvTxt);
        }

        private static void OnSvcRecvText(string recvTxt)
        {
            Console.WriteLine("ProxySvc Output: " + recvTxt);
        }
    }
}
