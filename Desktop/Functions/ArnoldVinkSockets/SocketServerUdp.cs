using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Stop the udp server
        private void UdpServerStop(UdpClient udpClient)
        {
            try
            {
                Debug.WriteLine("Stopping the udp server (S)");
                udpClient.Close();
                udpClient.Dispose();
                udpClient = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping udp server (S): " + ex.Message);
                udpClient = null;
            }
        }

        //Start the udp server
        private void UdpServerStart()
        {
            try
            {
                //Set the server end point
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, vSocketServerPort);

                //Start udp server
                vUdpServer = new UdpClient(serverEndPoint);

                Debug.WriteLine("Udp server is running on (S): " + serverEndPoint.Address + ":" + serverEndPoint.Port);

                //Start receive loop
                AVActions.TaskStartLoop(UdpReceiveLoop, vTask_UdpReceiveLoop);
            }
            catch (Exception ex)
            {
                SocketServerException(ex);
            }
        }
    }
}