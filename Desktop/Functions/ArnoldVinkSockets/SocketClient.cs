using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Tcp client check, create and connect
        public async Task<TcpClient> TcpClientCheckCreateConnect(string targetIp, int targetPort, int timeOut)
        {
            try
            {
                //Check existing tcp client
                if (vTcpClients.Any())
                {
                    //Clean disconnected tcp clients
                    TcpClientCleanConnections();

                    //Look for target tcp client
                    foreach (TcpClient tcpClient in vTcpClients)
                    {
                        try
                        {
                            IPEndPoint endPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                            if (endPoint.Address.ToString() == targetIp && endPoint.Port == targetPort)
                            {
                                //Debug.WriteLine("Reusing tcp client (C): " + endPoint.Address + ":" + endPoint.Port);
                                return tcpClient;
                            }
                        }
                        catch { }
                    }
                }

                //Create new tcp client
                if (!vCreatingClient)
                {
                    vCreatingClient = true;

                    Debug.WriteLine("Creating new tcp client (C): " + targetIp + ":" + targetPort);
                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    tcpClient.Client.LingerState = new LingerOption(true, 0);

                    //Connect the tcp client
                    bool connectAsync = await TcpClientConnectAsyncTimeout(tcpClient, targetIp, targetPort, timeOut);
                    if (!connectAsync)
                    {
                        Debug.WriteLine("Failed connecting to the tcp listener (C): " + targetIp + ":" + targetPort);
                        vCreatingClient = false;
                        return null;
                    }
                    else
                    {
                        Debug.WriteLine("Connected to the tcp listener (C): " + targetIp + ":" + targetPort);
                        vTcpClients.Insert(0, tcpClient);
                        vCreatingClient = false;
                        return tcpClient;
                    }
                }
                else
                {
                    Debug.WriteLine("Busy creating new tcp client (C)");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check create and connect tcp client (C): " + ex.Message);
                return null;
            }
        }

        //Clean disconnected tcp clients
        public void TcpClientCleanConnections()
        {
            try
            {
                //Debug.WriteLine("Cleaning disconnected tcp clients (C)");
                foreach (TcpClient tcpClient in vTcpClients.ToList())
                {
                    try
                    {
                        //Check if the tcp client is connected
                        if (tcpClient == null || !tcpClient.Connected || !tcpClient.Client.Connected)
                        {
                            Debug.WriteLine("Cleaned disconnected tcp client (C)");
                            vTcpClients.Remove(tcpClient);
                            continue;
                        }

                        //Check if the tcp client has timed out
                        if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            if (tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0)
                            {
                                Debug.WriteLine("Cleaned timed out tcp client (C)");
                                vTcpClients.Remove(tcpClient);
                                continue;
                            }
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Cleaned failed tcp client (C)");
                        vTcpClients.Remove(tcpClient);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed cleaning tcp clients (C): " + ex.Message);
            }
        }

        //Tcp client disconnect all
        public void TcpClientDisconnectAll()
        {
            try
            {
                Debug.WriteLine("Disconnecting all tcp clients (C)");
                foreach (TcpClient tcpClient in vTcpClients)
                {
                    try
                    {
                        TcpClientDisconnect(tcpClient);
                        vTcpClients.Remove(tcpClient);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting all tcp clients (C): " + ex.Message);
            }
        }

        //Tcp client disconnect
        public void TcpClientDisconnect(TcpClient tcpClient)
        {
            try
            {
                //Check if the tcp client is connected
                if (tcpClient == null || !tcpClient.Connected || !tcpClient.Client.Connected)
                {
                    Debug.WriteLine("Tcp client is not connected, disposing the client (C)");
                    tcpClient = null;
                    return;
                }

                Debug.WriteLine("Disconnecting and disposing tcp client (C)");
                tcpClient.GetStream().Close();
                tcpClient.Client.Close();
                tcpClient.Close();
                tcpClient = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting tcp client (C): " + ex.Message);
                tcpClient = null;
            }
        }

        //Send sockets bytes
        public async Task<bool> TcpClientSendBytes(TcpClient tcpClient, byte[] targetBytes, int timeOut, bool disconnectClient)
        {
            try
            {
                //Check if the tcp client is connected
                if (tcpClient == null || !tcpClient.Connected || !tcpClient.Client.Connected)
                {
                    Debug.WriteLine("The sending tcp client is not connected (C)");
                    return false;
                }

                //Get the tcp client information
                //IPEndPoint endPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;

                //Get the stream and write the bytes
                //Debug.WriteLine("Sending bytes (C): " + endPoint.Address.ToString() + ":" + endPoint.Port + " / " + targetBytes.Length);

                //Check if the network stream can read and write
                NetworkStream networkStream = tcpClient.GetStream();
                if (networkStream == null || !networkStream.CanRead || !networkStream.CanWrite)
                {
                    Debug.WriteLine("Network stream cannot read or write (C)");
                    return false;
                }

                bool writeAsync = await NetworkStreamWriteAsyncTimeout(networkStream, targetBytes, 0, targetBytes.Length, timeOut);
                if (!writeAsync)
                {
                    //Debug.WriteLine("Failed to write to the tcp listener (C): " + endPoint.Address.ToString() + ":" + endPoint.Port + " " + targetBytes.Length + "b");
                    TcpClientDisconnect(tcpClient);
                    return false;
                }

                //Disconnect tcp client after writing
                if (disconnectClient)
                {
                    TcpClientDisconnect(tcpClient);
                }

                //Debug.WriteLine("Sended bytes to the tcp listener (C): " + endTargetIp + ":" + endPoint.Port + " " + targetBytes.Length + "b");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending bytes (C): " + ex.Message);
                TcpClientDisconnect(tcpClient);
                return false;
            }
        }

        //Tcp client connect with timeout
        private async Task<bool> TcpClientConnectAsyncTimeout(TcpClient tcpClient, string targetIp, int targetPort, int timeOut)
        {
            try
            {
                Task timeTask = Task.Run(async delegate
                {
                    await tcpClient.ConnectAsync(targetIp, targetPort);
                });

                Task delayTask = Task.Delay(timeOut);
                Task timeoutTask = await Task.WhenAny(timeTask, delayTask);
                if (timeoutTask == timeTask)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Write network stream with timeout
        private async Task<bool> NetworkStreamWriteAsyncTimeout(NetworkStream stream, byte[] buffer, int offset, int count, int timeOut)
        {
            try
            {
                Task timeTask = Task.Run(async delegate
                {
                    await stream.WriteAsync(buffer, offset, count);
                });

                Task delayTask = Task.Delay(timeOut);
                Task timeoutTask = await Task.WhenAny(timeTask, delayTask);
                if (timeoutTask == timeTask)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}