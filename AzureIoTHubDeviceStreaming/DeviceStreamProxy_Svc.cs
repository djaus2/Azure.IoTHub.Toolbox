// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure_IoTHub_DeviceStreaming
{
    public class DeviceStreamProxySvc
    {
        private ServiceClient _serviceClient;
        private String _deviceId;
        private int _localPort;

        public ActionReceivedText OnRecvdTextD = null;
        public ActionReceivedText OnStatusUpdateD = null;

        private static DeviceStreamProxySvc sample = null;

        public DeviceStreamProxySvc(ServiceClient deviceClient, String deviceId, int localPort, ActionReceivedText onRecvdTextD = null, ActionReceivedText onStatusUpdateD = null)
        {
            sample = this;
            OnRecvdTextD = onRecvdTextD;
            OnStatusUpdateD = onStatusUpdateD;
            _serviceClient = deviceClient;
            _deviceId = deviceId;
            _localPort = localPort;
        }

        private static void ErrorMsg(string Context, Exception ex)
        {
            sample?.OnStatusUpdateD?.Invoke(string.Format("Svc " + Context + " {0}", ex.Message));
        }

        private static void Update(string msg)
        {
            sample?.OnStatusUpdateD?.Invoke("Svc " + msg);
        }

        private static void Info(string msg)
        {
            sample?.OnRecvdTextD?.Invoke(msg);
        }

        private static async Task HandleIncomingDataAsync(NetworkStream localStream, ClientWebSocket remoteStream, CancellationToken cancellationToken)
        {
            byte[] bytes = new byte[10240];
            System.ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(bytes);
            while (localStream.CanRead)
            {
                var receiveResult = await remoteStream.ReceiveAsync(receiveBuffer, cancellationToken).ConfigureAwait(false);

                await localStream.WriteAsync(receiveBuffer.Array, 0, receiveResult.Count).ConfigureAwait(false);
            }
        }

        private static long counter = 0;
        private static async Task HandleOutgoingDataAsync(NetworkStream localStream, ClientWebSocket remoteStream, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[10240];

            while (remoteStream.State == WebSocketState.Open)
            {
                int receiveCount = await localStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                counter += receiveCount;
                Info(string.Format("Device count= {0}", counter));

                await remoteStream.SendAsync(new ArraySegment<byte>(buffer, 0, receiveCount), WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async void HandleIncomingConnectionsAndCreateStreams(string deviceId, ServiceClient serviceClient, TcpClient tcpClient)
        {
            DeviceStreamRequest deviceStreamRequest = new DeviceStreamRequest(
                streamName: "ProxyStreamFromSvc"
            );

            using (var localStream = tcpClient.GetStream())
            {
                Update("Awaiting connection");
                using (cancellationTokenSourceOuter = new CancellationTokenSource())
                {
                    DeviceStreamResponse result = await serviceClient.CreateStreamAsync(deviceId, deviceStreamRequest, cancellationTokenSourceOuter.Token).ConfigureAwait(false);

                    Update($"Stream response received: Name={deviceStreamRequest.StreamName} IsAccepted={result.IsAccepted}");

                    if (result.IsAccepted)
                    {
                        try
                        {
                            using (cancellationTokenSource = new CancellationTokenSource())
                            using (ClientWebSocket remoteStream = await DeviceStreamingCommon.GetStreamingClientAsync(result.Url, result.AuthorizationToken, cancellationTokenSource.Token).ConfigureAwait(false))
                            {
                                Update("Starting streaming");
                                counter = 0;
                                await Task.WhenAny(
                                    HandleIncomingDataAsync(localStream, remoteStream, cancellationTokenSource.Token),
                                    HandleOutgoingDataAsync(localStream, remoteStream, cancellationTokenSource.Token)).ConfigureAwait(false);
                            }

                            Update("Done streaming");
                        }
                        catch (Exception ex)
                        {
                            ErrorMsg("Service got an exception: {0}", ex);
                        }
                    }
                }
            }
            tcpClient.Close();
        }

        private static CancellationTokenSource cancellationTokenSource = null;
        private static CancellationTokenSource cancellationTokenSourceOuter = null;

        public  static void Cancel()
        {
            Update("Cancelling.");
            cancellationTokenSource?.Cancel();
            cancellationTokenSourceOuter?.Cancel();
            _cancel = true;
        }

        private Task task = null;
        private static bool _cancel = false;

        public static bool isRunning = false;

        public async Task RunSampleAsync()
        {
            //IPAddress ipAddress = Dns.GetHostAddresses("localhost")[0];//.Resolve("localhost").AddressList[0];
            try
            {
                

                //TcpListener tcpListener = new TcpListener(ipAddress, _localPort);

                //IPAddress ip = new IPAddress(new byte[] { 192, 168, 0, 10 });
                //var tcpListener = new TcpListener(ip, _localPort);

                _cancel = false;
                var tcpListener = new TcpListener(IPAddress.Loopback, _localPort);
                Update(string.Format("Creating listener @ address {0} on port {1}.", IPAddress.Loopback, _localPort));
                tcpListener.Start();
                isRunning = false;
                while (!_cancel)
                {
                    Update("Starting to listen.");
                    var tcpClient = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    Update("Got listener.");
                    isRunning = true;
                    HandleIncomingConnectionsAndCreateStreams(_deviceId, _serviceClient, tcpClient);
                    isRunning = false;
                }
            }
            catch (Exception e)
            {
                ErrorMsg("Exiting connection", e);
            }
            
        }
}
}
