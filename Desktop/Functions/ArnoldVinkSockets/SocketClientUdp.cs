using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Send sockets bytes
        public async Task<bool> UdpClientSendBytes(IPEndPoint endPoint, byte[] targetBytes, int timeOut)
        {
            try
            {
                Task timeTask = Task.Run(async delegate
                {
                    await vUdpServer.SendAsync(targetBytes, targetBytes.Length, endPoint);
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
                Debug.WriteLine("Failed sending udp bytes (C): " + ex.Message);
                return false;
            }
        }
    }
}