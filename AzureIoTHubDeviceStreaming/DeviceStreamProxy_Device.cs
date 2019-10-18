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
        // Callback for received message
        private ActionReceivedText OnRecvdTextD = null;
        private ActionReceivedText OnDeviceStatusUpdateD = null;

        private static DeviceStreamSample sample = null;

        public DeviceStreamSample(DeviceClient deviceClient, String host, int port, ActionReceivedText onRecvdText,  ActionReceivedText onDeviceStatusUpdateD = null)
        {
            sample = this;
            OnRecvdTextD = onRecvdText;
            OnDeviceStatusUpdateD = onDeviceStatusUpdateD;

            _deviceClient = deviceClient;
            _host = host;
            _port = port;
        }

        private static void  ErrorMsg(string Context, Exception ex)
        {
            sample?.OnDeviceStatusUpdateD?.Invoke(string.Format("Device " + Context + " {0}", ex.Message));
        }

        private static void Update(string msg)
        {
            sample?.OnDeviceStatusUpdateD?.Invoke("Device " + msg);
        }

        private static void Info(string msg)
        {
            sample?.OnRecvdTextD?.Invoke(msg);
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
                Info(string.Format("Device count = {0}", counter));

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
                    ErrorMsg("Device got connection exception", ex);
                }
                Update("Device waiting again...");
            }
        }

        public async Task RunSampleAsync(bool acceptDeviceStreamingRequest, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                Update("Awaiting connection.");
                DeviceStreamRequest streamRequest = await _deviceClient.WaitForDeviceStreamRequestAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                Update("Got request.");
                if (streamRequest != null)
                {
                    if (acceptDeviceStreamingRequest)
                    {
                        try
                        {
                            await _deviceClient.AcceptDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
                            Update("Accepted request.");
                            using (ClientWebSocket webSocket = await DeviceStreamingCommon.GetStreamingClientAsync(streamRequest.Url, streamRequest.AuthorizationToken, cancellationTokenSource.Token).ConfigureAwait(false))
                            {
                                Update("Got client stream.");
                                using (TcpClient tcpClient = new TcpClient())
                                {
                                    Update("Connecting to stream.");
                                    await tcpClient.ConnectAsync(_host, _port).ConfigureAwait(false);

                                    using (NetworkStream localStream = tcpClient.GetStream())
                                    {
                                        Update("Device starting streaming");
                                        counter = 0;
                                        await Task.WhenAny(
                                            HandleIncomingDataAsync(localStream, webSocket, cancellationTokenSource.Token),
                                            HandleOutgoingDataAsync(localStream, webSocket, cancellationTokenSource.Token)).ConfigureAwait(false);
                                        Update("Closing stream");
                                        localStream.Close();
                                        Update("Closed stream");
                                    }
                                }

                                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSource.Token).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Update("Device got an inner exception. Exiting connection."); //: {0}", ex);
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
               ErrorMsg("Device got an outer exception", ex);
            }
        }
    }
}
