using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Clean disconnected tcp clients
        private async Task TcpCleanLoop()
        {
            try
            {
                Debug.WriteLine("Tcp client cleaner is now running (L)");

                //Tcp cleaner loop
                while (await TaskCheckLoop(vTask_TcpCleanLoop, 2000))
                {
                    try
                    {
                        //Debug.WriteLine("Cleaning disconnected tcp clients (L)");
                        lock (vTcpClients)
                        {
                            foreach (TcpClient tcpClient in vTcpClients)
                            {
                                try
                                {
                                    //Check if the tcp client is connected
                                    if (tcpClient == null || !tcpClient.Connected || !tcpClient.Client.Connected)
                                    {
                                        TcpClientRemoveFromList(tcpClient);
                                        Debug.WriteLine("Cleaned disconnected tcp client (L)");
                                        continue;
                                    }

                                    //Check if the tcp client has timed out
                                    if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                                    {
                                        if (tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0)
                                        {
                                            TcpClientRemoveFromList(tcpClient);
                                            Debug.WriteLine("Cleaned timed out tcp client (L)");
                                            continue;
                                        }
                                    }
                                }
                                catch
                                {
                                    TcpClientRemoveFromList(tcpClient);
                                    Debug.WriteLine("Cleaned failed tcp client (L)");
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed tcp client cleaner (L): " + ex.Message);
            }
        }

        //Remove tcp client from the list
        void TcpClientRemoveFromList(TcpClient tcpClient)
        {
            try
            {
                vTcpClients.Remove(tcpClient);
                TcpClientDisconnect(tcpClient);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed removing tcp client from list (L): " + ex.Message);
            }
        }
    }
}