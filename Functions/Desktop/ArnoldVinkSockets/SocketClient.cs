using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSocketClient
    {
        //Tcp client check
        public TcpClient SocketClientCheck(string targetIp, int targetPort, int timeOut)
        {
            try
            {
                //Check existing tcp client
                if (vTcpClients.Any())
                {
                    foreach (TcpClient tcpClientSearch in vTcpClients)
                    {
                        try
                        {
                            if (tcpClientSearch != null && tcpClientSearch.Connected)
                            {
                                IPEndPoint endPoint = (IPEndPoint)tcpClientSearch.Client.RemoteEndPoint;
                                string endTargetIp = endPoint.Address.ToString();
                                if (endTargetIp == targetIp && endPoint.Port == targetPort)
                                {
                                    //Debug.WriteLine("Reusing tcpclient: " + endTargetIp + ":" + endPoint.Port);
                                    return tcpClientSearch;
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Removing disconnected tcp client.");
                                vTcpClients.Remove(tcpClientSearch);
                            }
                        }
                        catch { }
                    }
                }

                //Create new tcp client
                return SocketClientCreateConnect(null, targetIp, targetPort, timeOut, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check tcp client: " + ex.Message);
                return null;
            }
        }

        //Tcp client create or connect
        public TcpClient SocketClientCreateConnect(TcpClient tcpClient, string targetIp, int targetPort, int timeOut, bool newClient)
        {
            try
            {
                if (newClient)
                {
                    Debug.WriteLine("Creating tcp client: " + targetIp + ":" + targetPort);
                    tcpClient = new TcpClient();
                }

                if (!tcpClient.ConnectAsync(targetIp, targetPort).Wait(timeOut))
                {
                    Debug.WriteLine("Failed to connect to the tcp listener: " + targetIp + ":" + targetPort);
                    SocketClientDisconnect(tcpClient);
                    return null;
                }
                else
                {
                    Debug.WriteLine("Connected to the tcp listener: " + targetIp + ":" + targetPort);
                    if (newClient) { vTcpClients.Insert(0, tcpClient); }
                    return tcpClient;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed connecting tcp client: " + ex.Message);
                SocketClientDisconnect(tcpClient);
                return null;
            }
        }

        //Tcp client disconnect
        public void SocketClientDisconnectAll()
        {
            try
            {
                foreach (TcpClient tcpClientSearch in vTcpClients)
                {
                    try
                    {
                        SocketClientDisconnect(tcpClientSearch);
                        vTcpClients.Remove(tcpClientSearch);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting all tcp clients: " + ex.Message);
            }
        }

        //Tcp client disconnect
        public void SocketClientDisconnect(TcpClient tcpClient)
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

        //Send sockets bytes
        public bool SocketClientSendBytes(TcpClient tcpClient, byte[] targetBytes, int timeOut, bool disconnectClient)
        {
            try
            {
                ////Get the tcp client information
                //IPEndPoint endPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                //string endTargetIp = endPoint.Address.ToString();

                //Get the stream and write the bytes
                //Debug.WriteLine("Sending bytes (C): " + endTargetIp + ":" + endPoint.Port + " / " + targetBytes.Length);
                if (!tcpClient.GetStream().WriteAsync(targetBytes, 0, targetBytes.Length).Wait(timeOut))
                {
                    //Debug.WriteLine("Failed to write to the tcp listener (C): " + endTargetIp + ":" + endPoint.Port);
                    SocketClientDisconnect(tcpClient);
                    return false;
                }

                //Disconnect tcp client after writing
                if (disconnectClient)
                {
                    SocketClientDisconnect(tcpClient);
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending bytes: " + ex.Message);
                SocketClientDisconnect(tcpClient);
                return false;
            }
        }
    }
}