using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Clean disconnected tcp clients
        public async void CleanLoop()
        {
            try
            {
                Debug.WriteLine("Tcp client cleaner is now running (L)");

                //Tcp clean loop
                while (vIsCleanRunning())
                {
                    try
                    {
                        //Debug.WriteLine("Cleaning disconnected tcp clients (L)");
                        foreach (TcpClient tcpClient in vTcpClients.ToList())
                        {
                            try
                            {
                                //Check if the tcp client is connected
                                if (tcpClient == null || !tcpClient.Connected || !tcpClient.Client.Connected)
                                {
                                    vTcpClients.Remove(tcpClient);
                                    Debug.WriteLine("Cleaned disconnected tcp client (L)");
                                    continue;
                                }

                                //Check if the tcp client has timed out
                                if (tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0)
                                {
                                    vTcpClients.Remove(tcpClient);
                                    Debug.WriteLine("Cleaned timed out tcp client (L)");
                                    continue;
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed tcp client cleaner (L): " + ex.Message);
            }
        }
    }
}