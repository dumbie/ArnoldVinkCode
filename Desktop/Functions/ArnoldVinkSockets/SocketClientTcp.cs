﻿using System;
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
                TcpClient tcpClientExisting = vTcpClients.Where(x => ((IPEndPoint)x.Client.RemoteEndPoint).Address.ToString() == targetIp && ((IPEndPoint)x.Client.RemoteEndPoint).Port == targetPort).FirstOrDefault();
                if (tcpClientExisting != null)
                {
                    //Debug.WriteLine("Reusing tcp client (C): " + targetIp + ":" + targetPort);
                    return tcpClientExisting;
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
                        Debug.WriteLine("Failed connecting to the tcp server (C): " + targetIp + ":" + targetPort);
                        vCreatingClient = false;
                        return null;
                    }
                    else
                    {
                        Debug.WriteLine("Connected to the tcp server (C): " + targetIp + ":" + targetPort);
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

        //Tcp client disconnect all
        public void TcpClientDisconnectAll()
        {
            try
            {
                Debug.WriteLine("Disconnecting all tcp clients (C)");
                lock (vTcpClients)
                {
                    foreach (TcpClient tcpClient in vTcpClients)
                    {
                        try
                        {
                            TcpClientRemoveFromList(tcpClient);
                        }
                        catch { }
                    }
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

        //Send sockets bytes to server
        public async Task<bool> TcpClientSendBytesServer(TcpClient tcpClient, byte[] targetBytes, int timeOut, bool disconnectClient)
        {
            try
            {
                //Check if the tcp client is connected
                if (tcpClient == null || !tcpClient.Connected || !tcpClient.Client.Connected)
                {
                    Debug.WriteLine("Sending tcp client is not connected (C)");
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
                    //Debug.WriteLine("Failed to write to the tcp server (C): " + endPoint.Address.ToString() + ":" + endPoint.Port + " " + targetBytes.Length + "b");
                    TcpClientDisconnect(tcpClient);
                    return false;
                }

                //Disconnect tcp client after writing
                if (disconnectClient)
                {
                    TcpClientDisconnect(tcpClient);
                }

                //Debug.WriteLine("Sended bytes to the tcp server (C): " + endTargetIp + ":" + endPoint.Port + " " + targetBytes.Length + "b");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending tcp bytes server (C): " + ex.Message);
                TcpClientDisconnect(tcpClient);
                return false;
            }
        }

        //Send sockets bytes to other
        public async Task<bool> TcpClientSendBytesOther(string targetIp, int targetPort, byte[] targetBytes, int timeOut)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        using (TcpClient tcpClient = new TcpClient(targetIp, targetPort))
                        {
                            using (NetworkStream networkStream = tcpClient.GetStream())
                            {
                                await networkStream.WriteAsync(targetBytes, 0, targetBytes.Length);
                            }
                        }
                    }
                    catch { }
                }

                bool taskRun = await AVActions.TaskStartTimeout(TaskAction, timeOut);
                if (taskRun)
                {
                    //Debug.WriteLine("Sended bytes to tcp other (C): " + targetIp + ":" + targetPort + " / " + targetBytes.Length);
                    return true;
                }
                else
                {
                    //Debug.WriteLine("Failed sending bytes to tcp other (C): " + targetIp + ":" + targetPort + " / " + targetBytes.Length);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending tcp bytes other (C): " + ex.Message);
                return false;
            }
        }

        //Tcp client connect with timeout
        private async Task<bool> TcpClientConnectAsyncTimeout(TcpClient tcpClient, string targetIp, int targetPort, int timeOut)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        await tcpClient.ConnectAsync(targetIp, targetPort);
                    }
                    catch { }
                }

                return await AVActions.TaskStartTimeout(TaskAction, timeOut);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to connect tcp client (C): " + ex.Message);
                return false;
            }
        }

        //Write network stream with timeout
        private async Task<bool> NetworkStreamWriteAsyncTimeout(NetworkStream stream, byte[] buffer, int offset, int count, int timeOut)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        await stream.WriteAsync(buffer, offset, count);
                    }
                    catch { }
                }

                return await AVActions.TaskStartTimeout(TaskAction, timeOut);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to write network stream (C): " + ex.Message);
                return false;
            }
        }
    }
}