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

        //Send sockets bytes to server
        public async Task<bool> UdpClientSendBytesServer(IPEndPoint endPoint, byte[] targetBytes, int timeOut)
        {
            try
            {
                Task timeTask = Task.Run(async delegate
                {
                    try
                    {
                        await vUdpServer.SendAsync(targetBytes, targetBytes.Length, endPoint);
                    }
                    catch { }
                });

                Task delayTask = Task.Delay(timeOut);
                Task timeoutTask = await Task.WhenAny(timeTask, delayTask);
                if (timeoutTask == timeTask)
                {
                    //Debug.WriteLine("Sended bytes to the udp server (C): " + endPoint.Address + ":" + endPoint.Port + " / " + targetBytes.Length);
                    return true;
                }
                else
                {
                    //Debug.WriteLine("Failed sending bytes to the udp server (C): " + endPoint.Address + ":" + endPoint.Port + " / " + targetBytes.Length);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending udp bytes server (C): " + ex.Message);
                return false;
            }
        }

        //Send sockets bytes to other
        public async Task<bool> UdpClientSendBytesOther(string targetIp, int targetPort, byte[] targetBytes, int timeOut)
        {
            try
            {
                Task timeTask = Task.Run(async delegate
                {
                    try
                    {
                        using (UdpClient udpClient = new UdpClient(targetIp, targetPort))
                        {
                            await udpClient.SendAsync(targetBytes, targetBytes.Length);
                        }
                    }
                    catch { }
                });

                Task delayTask = Task.Delay(timeOut);
                Task timeoutTask = await Task.WhenAny(timeTask, delayTask);
                if (timeoutTask == timeTask)
                {
                    //Debug.WriteLine("Sended bytes to udp other (C): " + targetIp + ":" + targetPort + " / " + targetBytes.Length);
                    return true;
                }
                else
                {
                    //Debug.WriteLine("Failed sending bytes to udp other (C): " + targetIp + ":" + targetPort + " / " + targetBytes.Length);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending udp bytes other (C): " + ex.Message);
                return false;
            }
        }
    }
}