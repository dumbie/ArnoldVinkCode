using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Stop the udp server
        private void UdpServerStop(UdpClient udpClient)
        {
            try
            {
                if (udpClient != null)
                {
                    Debug.WriteLine("Stopping the udp server (S)");
                    udpClient.Close();
                    udpClient = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping udp server (S): " + ex.Message);
                udpClient = null;
            }
        }

        //Start the udp server
        private async Task UdpServerStart()
        {
            try
            {
                //Set server endpoint
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, vSocketServerPort);

                //Start udp server
                vUdpServer = new UdpClient();
                vUdpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                vUdpServer.Client.Bind(serverEndPoint);

                Debug.WriteLine("Udp server is running on (S): " + serverEndPoint.Address + ":" + serverEndPoint.Port);

                //Start receive loop
                AVActions.TaskStartLoop(UdpReceiveLoop, vTask_UdpReceiveLoop);

                //Start clean loop
                AVActions.TaskStartLoop(UdpCleanLoop, vTask_UdpCleanLoop);
            }
            catch (Exception ex)
            {
                await SocketServerException(ex);
            }
        }
    }
}