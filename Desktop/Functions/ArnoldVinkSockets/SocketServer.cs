using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Create and enable tcp listener
        public ArnoldVinkSockets(string serverIp, int serverPort)
        {
            try
            {
                vTcpListenerIp = serverIp;
                vTcpListenerPort = serverPort;
                SocketServerEnable();
            }
            catch { }
        }

        //Enable the tcp listener
        public void SocketServerEnable()
        {
            try
            {
                Debug.WriteLine("Enabling the tcp listener (S)");
                AVActions.TaskStartLoop(LoopTcpListener, vTask_SocketServer);
                AVActions.TaskStartLoop(LoopTcpCleaner, vTask_SocketClean);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to enable the tcp listener (S): " + ex.Message);
            }
        }

        //Disable the tcp listener
        public async Task SocketServerDisable()
        {
            try
            {
                Debug.WriteLine("Disabling the tcp listener (S)");

                //Disconnect all the clients
                TcpClientDisconnectAll();

                //Stop the tcp listener
                TcpListenerStop(vTcpListener);

                //Stop the listener loop
                await AVActions.TaskStopLoop(vTask_SocketServer);

                //Stop the clean loop
                await AVActions.TaskStopLoop(vTask_SocketClean);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to disable the tcp listener (S): " + ex.Message);
            }
        }

        //Disable the tcp listener
        public async Task SocketServerRestart()
        {
            try
            {
                Debug.WriteLine("Restarting the tcp listener (S)");
                await SocketServerDisable();
                SocketServerEnable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to restart the tcp listener (S): " + ex.Message);
            }
        }

        //Stop the tcp listener
        private void TcpListenerStop(TcpListener tcpListener)
        {
            try
            {
                Debug.WriteLine("Stopping the tcp listener (S)");
                tcpListener.Server.Close();
                tcpListener.Stop();
                tcpListener = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping tcp listener (S): " + ex.Message);
                tcpListener = null;
            }
        }

        //Handle incoming tcp client
        private async Task<bool> ListenerClientHandle(TcpClient tcpClient)
        {
            try
            {
                //Read client stream from listener
                while (tcpClient != null && tcpClient.Connected)
                {
                    try
                    {
                        //Check if the network stream can read and write
                        NetworkStream networkStream = tcpClient.GetStream();
                        if (networkStream == null || !networkStream.CanRead || !networkStream.CanWrite)
                        {
                            Debug.WriteLine("Network stream cannot read or write (S)");
                            break;
                        }

                        //Receive the data from the network stream
                        byte[] receivedBytes = new byte[tcpClient.ReceiveBufferSize];
                        int bytesReceivedLength = await networkStream.ReadAsync(receivedBytes, 0, receivedBytes.Length);
                        if (bytesReceivedLength > 0)
                        {
                            //Debug.WriteLine("Received bytes from tcp client (S): " + bytesReceivedLength);
                            await EventBytesReceived(tcpClient, receivedBytes);
                        }
                        else
                        {
                            Debug.WriteLine("No more bytes received from tcp client (S)");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed reading bytes from tcp client (S) " + ex.Message);
                        break;
                    }
                }

                TcpClientDisconnect(tcpClient);
                Debug.WriteLine("Finished tcp client session (S)");
                return true;
            }
            catch
            {
                TcpClientDisconnect(tcpClient);
                Debug.WriteLine("Failed tcp client session (S)");
                return true;
            }
        }

        //Receive incoming tcp clients
        private async void LoopTcpListener()
        {
            try
            {
                //Start tcp listener
                vTcpListener = new TcpListener(IPAddress.Any, vTcpListenerPort);
                vTcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                vTcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                vTcpListener.Server.LingerState = new LingerOption(true, 0);
                vTcpListener.Start();

                Debug.WriteLine("Tcp listener is running on (S): " + vTcpListener.LocalEndpoint);

                //Tcp listener loop
                while (vTask_SocketServer.Status == AVTaskStatus.Running)
                {
                    try
                    {
                        TcpClient tcpClient = await vTcpListener.AcceptTcpClientAsync();
                        if (tcpClient != null && tcpClient.Connected)
                        {
                            Debug.WriteLine("New tcp client connected from (S): " + tcpClient.Client.RemoteEndPoint);
                            async void TaskAction()
                            {
                                try
                                {
                                    await ListenerClientHandle(tcpClient);
                                }
                                catch { }
                            }
                            await AVActions.TaskStart(TaskAction);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                await ListenerLoop_Exception(ex);
            }
            finally
            {
                vTask_SocketServer.Status = AVTaskStatus.Stopped;
            }
        }

        //Receive incoming tcp clients exception
        async Task ListenerLoop_Exception(Exception ex)
        {
            try
            {
                //Check the exception type
                if (ex.GetType() == typeof(SocketException))
                {
                    SocketException socketException = (SocketException)ex;
                    if (socketException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        //Disable the tcp listener
                        MessageBox.Show("Failed to launch the socket server, please make sure that the used server port is not already in use.", "Socket Server");
                        Debug.WriteLine("The tcp listener port is in use (S): " + ex.Message);
                        await SocketServerDisable();
                    }
                    else
                    {
                        //Restart the tcp listener
                        Debug.WriteLine("The tcp listener has crashed (S): " + ex.Message);
                        await SocketServerRestart();
                    }
                }
            }
            catch { }
        }
    }
}