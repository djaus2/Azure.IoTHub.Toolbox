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

namespace DNCoreDeviceStreaming
{
    public  class DeviceStreamProxySvc
    {
        private ServiceClient _serviceClient;
        private String _deviceId;
        private int _localPort;
        public ActionReceivedText OnRecvdTextD = null;
        public ActionReceivedText OnStatusUpdateD = null;

        private  DeviceStreamProxySvc sample = null;

        public  DeviceStreamProxySvc(ServiceClient deviceClient, String deviceId, int localPort, ActionReceivedText onRecvdTextD = null, ActionReceivedText onStatusUpdateD = null)
        {
            sample = this;
            OnRecvdTextD = onRecvdTextD;
            OnStatusUpdateD = onStatusUpdateD;

            _serviceClient = deviceClient;
            _deviceId = deviceId;
            _localPort = localPort;
        }

        private  async Task HandleIncomingDataAsync(NetworkStream localStream, ClientWebSocket remoteStream, CancellationToken cancellationToken)
        {
            byte[] receiveBuffer = new byte[10240];

            while (localStream.CanRead)
            {
                var receiveResult = await remoteStream.ReceiveAsync(receiveBuffer, cancellationToken).ConfigureAwait(false);

                await localStream.WriteAsync(receiveBuffer, 0, receiveResult.Count).ConfigureAwait(false);
            }
        }

        private   long counter = 0;
        private  async Task HandleOutgoingDataAsync(NetworkStream localStream, ClientWebSocket remoteStream, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[10240];

            while (remoteStream.State == WebSocketState.Open)
            {
                int receiveCount = await localStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                counter += receiveCount;
                Console.WriteLine(string.Format("Device cont= {0}", counter));

                await remoteStream.SendAsync(new ArraySegment<byte>(buffer, 0, receiveCount), WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
        }

        private  async void HandleIncomingConnectionsAndCreateStreams(string deviceId, ServiceClient serviceClient, TcpClient tcpClient)
        {
            DeviceStreamRequest deviceStreamRequest = new DeviceStreamRequest(
                streamName: "TestStream"
            );

            using (var localStream = tcpClient.GetStream())
            {
                DeviceStreamResponse result = await serviceClient.CreateStreamAsync(deviceId, deviceStreamRequest, CancellationToken.None).ConfigureAwait(false);

                Console.WriteLine($"Stream response received: Name={deviceStreamRequest.StreamName} IsAccepted={result.IsAccepted}");

                if (result.IsAccepted)
                {
                    try
                    {
                        using (var cancellationTokenSource = new CancellationTokenSource())
                        using (var remoteStream = await DeviceStreamingCommon.GetStreamingClientAsync(result.Url, result.AuthorizationToken, cancellationTokenSource.Token).ConfigureAwait(false))
                        {
                            Console.WriteLine("Starting streaming");
                            counter = 0;
                            await Task.WhenAny(
                                HandleIncomingDataAsync(localStream, remoteStream, cancellationTokenSource.Token),
                                HandleOutgoingDataAsync(localStream, remoteStream, cancellationTokenSource.Token)).ConfigureAwait(false);
                        }

                            Console.WriteLine("Done streaming");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Service got an exception: {0}", ex);
                    }
                }
            }
            tcpClient.Close();
        }

        public async Task RunSampleAsync()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, _localPort);
            tcpListener.Start();

            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

                HandleIncomingConnectionsAndCreateStreams(_deviceId, _serviceClient, tcpClient);
            }
        }
    }
}
