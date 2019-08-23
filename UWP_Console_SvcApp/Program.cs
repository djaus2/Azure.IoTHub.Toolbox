﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;

//Ref: https://github.com/PieEatingNinjas/UWPConsoleSvcApp/tree/master/Source/UwpConsole

namespace UWPConsoleSvcApp
{
    public static class Program
    {
        private static int waitAtEndOfConsoleAppSecs = AzureConnections.MyConnections.WaitAtEndOfConsoleAppSecs;
        private static int timeout = AzureConnections.MyConnections.Timeout;

        private static int DeviceAction = AzureConnections.MyConnections.DeviceAction;

        private static bool basicMode = AzureConnections.MyConnections.basicMode;
        private static bool UseCustomClass = AzureConnections.MyConnections.UseCustomClass;
        private static bool ResponseExpected = AzureConnections.MyConnections.ResponseExpected;
        private static bool KeepAlive = AzureConnections.MyConnections.KeepAlive;

        private static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        private static string device_id = AzureConnections.MyConnections.DeviceId;
        private static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        private static int DevKeepListening = 2; //No action
        private static int DevAutoStart = 2; //No action

        private static string msgOut = "Temp";

        public static int Main(string[] args)
        {
            Console.WriteLine("Svc: Starting.\n");

            RunSvc(service_cs, device_id, msgOut, timeout);

            Console.WriteLine(string.Format("Svc Done.\n\nApp will close in {0} seconds.\n", waitAtEndOfConsoleAppSecs));

            TimeSpan ts = TimeSpan.FromSeconds(waitAtEndOfConsoleAppSecs);
            Task.Delay(ts).GetAwaiter().GetResult();
            return 0;
        }

        private static void OnSvcRecvText(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void OnDeviceSvcUpdate(string recvTxt)
        {
            if (!string.IsNullOrEmpty(recvTxt))
                Console.WriteLine("Update: " + recvTxt);
        }

        private static void RunSvc(string servvicecs, string devid, string msgOut, double ts)
        {

            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromMilliseconds(ts);

            try
            {
                if (basicMode)
                    DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText).GetAwaiter().GetResult();
                else if (!UseCustomClass)
                    DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, DevKeepListening, DevAutoStart, OnDeviceSvcUpdate, KeepAlive, ResponseExpected).GetAwaiter().GetResult();

                else
                    DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, DevKeepListening, DevAutoStart, OnDeviceSvcUpdate, KeepAlive, ResponseExpected, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Hub connection failure");
            //}
            //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Device not found");
            //}
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0Error App.RunSvc(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Timeout");
                }
            }
        }
    }
}
