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
        //Enable the tcp listener
        public void SocketServerEnable()
        {
            try
            {
                Debug.WriteLine("Enabling the tcp listener.");
                vTaskToken_SocketServer = new CancellationTokenSource();
                vTask_SocketServer = AVActions.TaskStart(ListenerLoop, vTaskToken_SocketServer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to enable the tcp listener: " + ex.Message);
            }
        }

        //Disable the tcp listener
        public async Task SocketServerDisable()
        {
            try
            {
                Debug.WriteLine("Disabling the tcp listener.");
                await AVActions.TaskStop(vTask_SocketServer, vTaskToken_SocketServer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to disable the tcp listener: " + ex.Message);
            }
        }

        //Disable the tcp listener
        public async Task SocketServerRestart()
        {
            try
            {
                Debug.WriteLine("Restarting the tcp listener.");
                await SocketServerDisable();
                SocketServerEnable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to restart the tcp listener: " + ex.Message);
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

        //Tcp client disconnect
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

        //Read network stream with timeout
        private async Task<int> NetworkStreamReadAsyncTimeout(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            try
            {
                Task<int> readTask = stream.ReadAsync(buffer, offset, count);
                Task delayTask = Task.Delay(vTcpListenerTimeout);
                Task timeoutTask = await Task.WhenAny(readTask, delayTask);
                if (timeoutTask == readTask)
                {
                    return await readTask;
                }
            }
            catch { }
            return 0;
        }

        //Handle incoming tcp client
        private async Task ListenerClientHandle(TcpClient tcpClient)
        {
            try
            {
                //Read client stream from listener
                while (tcpClient != null)
                {
                    try
                    {
                        //Check if the network stream can be read
                        NetworkStream networkStream = tcpClient.GetStream();
                        if (networkStream == null || !networkStream.CanRead)
                        {
                            Debug.WriteLine("Stream cannot be read (S)");
                            break;
                        }

                        //Receive the data from the network stream
                        byte[] receivedBytes = new byte[tcpClient.ReceiveBufferSize];
                        int bytesReceivedLength = await NetworkStreamReadAsyncTimeout(networkStream, receivedBytes, 0, receivedBytes.Length);
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

                Debug.WriteLine("The tcp listener is running on: " + tcpListener.LocalEndpoint);

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