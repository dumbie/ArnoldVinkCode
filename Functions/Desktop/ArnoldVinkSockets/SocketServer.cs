using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSocketServer
    {
        //Enable the socket server
        public void SocketServerEnable()
        {
            try
            {
                Debug.WriteLine("Enabling the socket server.");
                vTaskToken_SocketServer = new CancellationTokenSource();
                vTask_SocketServer = AVActions.TaskStart(ListenerLoop, vTaskToken_SocketServer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to enable the socket server: " + ex.Message);
            }
        }

        //Disable the socket server
        public async Task SocketServerDisable()
        {
            try
            {
                Debug.WriteLine("Disabling the socket server.");
                await AVActions.TaskStop(vTask_SocketServer, vTaskToken_SocketServer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to disable the socket server: " + ex.Message);
            }
        }

        //Disable the socket server
        public async Task SocketServerRestart()
        {
            try
            {
                Debug.WriteLine("Restarting the socket server.");
                await SocketServerDisable();
                SocketServerEnable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to restart the socket server: " + ex.Message);
            }
        }

        //Stop the tcp listener
        private void TcpListenerStop(TcpListener tcpListener)
        {
            try
            {
                Debug.WriteLine("Stopping the tcp listener.");
                tcpListener.Server.Close();
                tcpListener.Stop();
                tcpListener = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping tcp listener: " + ex.Message);
                tcpListener = null;
            }
        }

        //Disconnect tcp client
        private void TcpClientDisconnect(TcpClient tcpClient)
        {
            try
            {
                Debug.WriteLine("Disconnecting the tcp client.");
                tcpClient.GetStream().Close();
                tcpClient.Client.Close();
                tcpClient.Close();
                tcpClient = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting tcp client: " + ex.Message);
                tcpClient = null;
            }
        }

        //Handle tcp listener client
        private async Task ListenerClientHandle(TcpClient tcpClient)
        {
            try
            {
                //Read client stream from listener
                while (tcpClient != null)
                {
                    try
                    {
                        //Get the stream and receive the bytes
                        byte[] receivedBytes = new byte[tcpClient.ReceiveBufferSize];
                        int bytesReceivedLength = await tcpClient.GetStream().ReadAsync(receivedBytes, 0, receivedBytes.Length);
                        if (bytesReceivedLength > 0)
                        {
                            //Debug.WriteLine("Received bytes (S): " + bytesReceivedLength);
                            await EventBytesReceived(tcpClient, receivedBytes);
                        }
                        else
                        {
                            Debug.WriteLine("No more bytes received (S)");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed reading bytes (S) " + ex.Message);
                        break;
                    }
                }

                //Disconnect client from listener
                TcpClientDisconnect(tcpClient);
                Debug.WriteLine("Finished tcp client session.");
            }
            catch { }
        }

        //Receive incoming tcp clients
        private async void ListenerLoop()
        {
            try
            {
                //Start tcp listener
                TcpListener tcpListener = new TcpListener(IPAddress.Any, vTcpListenerPort);
                tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                tcpListener.Server.LingerState = new LingerOption(true, 0);
                tcpListener.Start();

                Debug.WriteLine("The socket server is running on: " + tcpListener.LocalEndpoint);

                //Tcp listener loop
                while (vIsServerRunning())
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    if (tcpClient != null && tcpClient.Connected)
                    {
                        Debug.WriteLine("New tcp client connected (S)");
                        async void TaskAction()
                        {
                            try
                            {
                                await ListenerClientHandle(tcpClient);
                            }
                            catch { }
                        }
                        await AVActions.TaskStart(TaskAction, null);
                    }
                }

                //Stop the tcp listener
                TcpListenerStop(tcpListener);
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147467259)
                {
                    MessageBox.Show("Failed to launch the socket server, please make sure the used server port is not already in use.", "Socket Server");
                    Debug.WriteLine("The tcp listener port is in use: " + ex.Message);
                    await SocketServerDisable();
                }
                else
                {
                    Debug.WriteLine("The tcp listener has crashed: " + ex.Message);
                    await SocketServerRestart();
                }
            }
        }
    }
}