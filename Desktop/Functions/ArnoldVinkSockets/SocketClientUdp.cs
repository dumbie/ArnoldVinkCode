using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Udp client disconnect all
        public void UdpClientDisconnectAll()
        {
            try
            {
                Debug.WriteLine("Disconnecting all udp clients (C)");
                lock (vUdpClients)
                {
                    foreach (UdpEndPointDetails udpClient in vUdpClients)
                    {
                        try
                        {
                            UdpClientRemoveFromList(udpClient);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting all udp clients (C): " + ex.Message);
            }
        }

        //Udp client disconnect
        public void UdpClientDisconnect(UdpEndPointDetails udpClient)
        {
            try
            {
                Debug.WriteLine("Disconnecting and disposing udp client (C)");
                udpClient.Active = false;
                udpClient.IPEndPoint = null;
                udpClient.LastConnection = 0;
                udpClient = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed disconnecting udp client (C): " + ex.Message);
                udpClient = null;
            }
        }

        //Send sockets bytes to server with timeout
        public async Task<bool> UdpClientSendBytesServer(IPEndPoint endPoint, byte[] targetBytes, int timeOutMs)
        {
            try
            {
                int sendBytes = await vUdpServer.SendAsync(targetBytes, targetBytes.Length, endPoint).WaitAsync(TimeSpan.FromMilliseconds(timeOutMs));
                //Debug.WriteLine("Sended bytes to the udp server (C): " + endPoint.Address + ":" + endPoint.Port + " (" + sendBytes + "/" + targetBytes.Length + ")");
                return sendBytes == targetBytes.Length;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending udp bytes server (C): " + ex.Message);
                return false;
            }
        }

        //Send sockets bytes to other with timeout
        public async Task<bool> UdpClientSendBytesOther(string targetIp, int targetPort, byte[] targetBytes, int timeOutMs)
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    int sendBytes = await udpClient.SendAsync(targetBytes, targetBytes.Length, targetIp, targetPort).WaitAsync(TimeSpan.FromMilliseconds(timeOutMs));
                    //Debug.WriteLine("Sended bytes to udp other (C): " + targetIp + ":" + targetPort + " (" + sendBytes + "/" + targetBytes.Length + ")");
                    return sendBytes == targetBytes.Length;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending udp bytes other (C): " + ex.Message);
                return false;
            }
        }

        //Send sockets bytes to broadcast with timeout
        public async Task<bool> UdpClientSendBytesBroadcast(bool broadcastLocal, int targetPort, byte[] targetBytes, int timeOutMs)
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    string targetIp = broadcastLocal ? "127.255.255.255" : "255.255.255.255";
                    int sendBytes = await udpClient.SendAsync(targetBytes, targetBytes.Length, targetIp, targetPort).WaitAsync(TimeSpan.FromMilliseconds(timeOutMs));
                    //Debug.WriteLine("Sended bytes to udp broadcast (C): " + targetIp + ":" + targetPort + " (" + sendBytes + "/" + targetBytes.Length + ")");
                    return sendBytes == targetBytes.Length;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending udp bytes broadcast (C): " + ex.Message);
                return false;
            }
        }
    }
}