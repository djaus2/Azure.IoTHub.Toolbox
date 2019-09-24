// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure_IoTHub_DeviceStreaming
{
    public class DeviceStreamSample
    {
        private DeviceClient _deviceClient;
        private String _host;
        private int _port;

        public DeviceStreamSample(DeviceClient deviceClient, String host, int port)
        {
            _deviceClient = deviceClient;
            _host = host;
            _port = port;
        }

        private static async Task HandleIncomingDataAsync(NetworkStream localStream, ClientWebSocket remoteStream, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[10240];
            System.ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(buffer);

            while (remoteStream.State == WebSocketState.Open)
            {
                var receiveResult = await remoteStream.ReceiveAsync(receiveBuffer, cancellationToken).ConfigureAwait(false);

                await localStream.WriteAsync(receiveBuffer.Array, 0, receiveResult.Count).ConfigureAwait(false);
            }
        }

        private static long counter = 0;
        private static async Task HandleOutgoingDataAsync(NetworkStream localStream, ClientWebSocket remoteStream, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[10240];

            while (localStream.CanRead)
            {
                int receiveCount = await localStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                counter += receiveCount;
                Console.WriteLine(string.Format("Device count = {0}", counter));

                await remoteStream.SendAsync(new ArraySegment<byte>(buffer, 0, receiveCount), WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task RunSampleAsync(CancellationTokenSource cancellationTokenSource)
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    await RunSampleAsync(true, cancellationTokenSource).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Device got connection exception: {0}", ex);
                }
                Console.WriteLine("Device waiting again...");
            }
        }

        public async Task RunSampleAsync(bool acceptDeviceStreamingRequest, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                DeviceStreamRequest streamRequest = await _deviceClient.WaitForDeviceStreamRequestAsync(cancellationTokenSource.Token).ConfigureAwait(false);

                if (streamRequest != null)
                {
                    if (acceptDeviceStreamingRequest)
                    {
                        try
                        {
                            await _deviceClient.AcceptDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);

                            using (ClientWebSocket webSocket = await DeviceStreamingCommon.GetStreamingClientAsync(streamRequest.Url, streamRequest.AuthorizationToken, cancellationTokenSource.Token).ConfigureAwait(false))
                            {
                                using (TcpClient tcpClient = new TcpClient())
                                {
                                    await tcpClient.ConnectAsync(_host, _port).ConfigureAwait(false);

                                    using (NetworkStream localStream = tcpClient.GetStream())
                                    {
                                        Console.WriteLine("Starting streaming");
                                        counter = 0;
                                        await Task.WhenAny(
                                            HandleIncomingDataAsync(localStream, webSocket, cancellationTokenSource.Token),
                                            HandleOutgoingDataAsync(localStream, webSocket, cancellationTokenSource.Token)).ConfigureAwait(false);

                                        localStream.Close();
                                    }
                                }

                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSource.Token).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Device got an inner exception. Exiting connection."); //: {0}", ex);
                        }
                    }
                    else
                    {
                        await _deviceClient.RejectDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Device got an outer exception: {0}", ex);
            }
        }
    }
}
