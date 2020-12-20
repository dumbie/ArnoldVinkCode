using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Udp receive loop
        async Task UdpReceiveLoop()
        {
            try
            {
                while (!vTask_UdpReceiveLoop.TaskStopRequest)
                {
                    try
                    {
                        UdpReceiveResult receiveResult = await vUdpServer.ReceiveAsync();
                        byte[] receivedBytes = receiveResult.Buffer;
                        if (receivedBytes.Length > 0)
                        {
                            //Debug.WriteLine("Received bytes from udp client (S): " + receivedBytes.Length);
                            await EventBytesReceived(null, receiveResult.RemoteEndPoint, receivedBytes);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}