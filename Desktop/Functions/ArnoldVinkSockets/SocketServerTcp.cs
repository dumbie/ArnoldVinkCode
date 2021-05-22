using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Stop the tcp server
        private void TcpServerStop(TcpListener tcpServer)
        {
            try
            {
                if (tcpServer != null)
                {
                    Debug.WriteLine("Stopping the tcp server (S)");
                    tcpServer.Server.Close();
                    tcpServer.Stop();
                    tcpServer = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping tcp server (S): " + ex.Message);
                tcpServer = null;
            }
        }

        //Start the tcp server
        private async Task TcpServerStart()
        {
            try
            {
                //Set the server endpoint
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, vSocketServerPort);

                //Start tcp server
                vTcpServer = new TcpListener(serverEndPoint);
                vTcpServer.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                vTcpServer.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                vTcpServer.Server.LingerState = new LingerOption(true, 0);
                vTcpServer.Start();

                Debug.WriteLine("Tcp server is running on (S): " + serverEndPoint.Address + ":" + serverEndPoint.Port);

                //Start receive loop
                AVActions.TaskStartLoop(TcpReceiveLoop, vTask_TcpReceiveLoop);

                //Start clean loop
                AVActions.TaskStartLoop(TcpCleanLoop, vTask_TcpCleanLoop);
            }
            catch (Exception ex)
            {
                await SocketServerException(ex);
            }
        }
    }
}