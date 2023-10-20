using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Initialize sockets
        public ArnoldVinkSockets(string serverIp, int serverPort, bool tcpServer, bool udpServer)
        {
            try
            {
                vSocketServerIp = serverIp;
                vSocketServerPort = serverPort;
                vTcpServerEnabled = tcpServer;
                vUdpServerEnabled = udpServer;
            }
            catch { }
        }

        //Enable the socket server
        public async Task SocketServerEnable()
        {
            try
            {
                Debug.WriteLine("Enabling the socket server (S)");
                if (vTcpServerEnabled) { await TcpServerStart(); }
                if (vUdpServerEnabled) { await UdpServerStart(); }
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

                //Stop the socket servers
                TcpServerStop(vTcpServer);
                UdpServerStop(vUdpServer);

                //Disconnect all the clients
                TcpClientDisconnectAll();
                UdpClientDisconnectAll();

                //Stop the server loops
                await AVActions.TaskStopLoop(vTask_TcpReceiveLoop, 5000);
                await AVActions.TaskStopLoop(vTask_UdpReceiveLoop, 5000);

                //Stop the clean loops
                await AVActions.TaskStopLoop(vTask_TcpCleanLoop, 5000);
                await AVActions.TaskStopLoop(vTask_UdpCleanLoop, 5000);
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
                await SocketServerEnable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to restart the socket server (S): " + ex.Message);
            }
        }

        //Socket server exception
        async Task SocketServerException(Exception ex)
        {
            try
            {
                Debug.WriteLine("Socket server error (S): " + ex.Message);

                List<string> messageAnswers = new List<string>();
                messageAnswers.Add("Restart server");
                messageAnswers.Add("Cancel");

                string messageResult = await new AVMessageBox().Popup(null, "Socket server error", ex.Message, messageAnswers);
                if (messageResult == "Restart server")
                {
                    await SocketServerRestart();
                }
            }
            catch { }
        }
    }
}