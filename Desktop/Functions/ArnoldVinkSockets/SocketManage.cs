using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Create and enable socket server
        public ArnoldVinkSockets(string serverIp, int serverPort)
        {
            try
            {
                vSocketServerIp = serverIp;
                vSocketServerPort = serverPort;
                SocketServerEnable();
            }
            catch { }
        }

        //Enable the socket server
        public void SocketServerEnable()
        {
            try
            {
                Debug.WriteLine("Enabling the socket server (S)");
                TcpServerStart();
                UdpServerStart();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to enable the socket server (S): " + ex.Message);
            }
        }

        //Disable the socket server
        public async Task SocketServerDisable()
        {
            try
            {
                Debug.WriteLine("Disabling the socket server (S)");

                //Disconnect all the clients
                TcpClientDisconnectAll();

                //Stop the socket servers
                TcpServerStop(vTcpServer);
                UdpServerStop(vUdpServer);

                //Stop the server loops
                await AVActions.TaskStopLoop(vTask_TcpReceiveLoop);
                await AVActions.TaskStopLoop(vTask_UdpReceiveLoop);

                //Stop the clean loop
                await AVActions.TaskStopLoop(vTask_TcpCleanLoop);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to disable the socket server (S): " + ex.Message);
            }
        }

        //Restart the socket server
        public async Task SocketServerRestart()
        {
            try
            {
                Debug.WriteLine("Restarting the socket server (S)");
                await SocketServerDisable();
                SocketServerEnable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to restart the socket server (S): " + ex.Message);
            }
        }

        //Socket server exception
        void SocketServerException(Exception ex)
        {
            try
            {
                if (ex.GetType() == typeof(SocketException))
                {
                    SocketException socketException = (SocketException)ex;
                    if (socketException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        Debug.WriteLine("Socket server port is in use (S): " + ex.Message);
                        MessageBox.Show("Failed to start socket server, please make sure that the used server port is not already in use.", "Socket Server");
                        //await SocketServerDisable();
                    }
                    else
                    {
                        Debug.WriteLine("Socket server has crashed (S): " + ex.Message);
                        MessageBox.Show("Socket server has crashed, please make sure that the used server port is not already in use.", "Socket Server");
                        //await SocketServerRestart();
                    }
                }
            }
            catch { }
        }
    }
}