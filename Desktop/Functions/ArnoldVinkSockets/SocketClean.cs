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
        private async Task LoopTcpCleaner()
        {
            try
            {
                Debug.WriteLine("Tcp client cleaner is now running (L)");

                //Tcp cleaner loop
                while (!vTask_SocketClean.TaskStopRequest)
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
                                    CleanRemoveTcpClientFromList(tcpClient);
                                    Debug.WriteLine("Cleaned disconnected tcp client (L)");
                                    continue;
                                }

                                //Check if the tcp client has timed out
                                if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                                {
                                    if (tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0)
                                    {
                                        CleanRemoveTcpClientFromList(tcpClient);
                                        Debug.WriteLine("Cleaned timed out tcp client (L)");
                                        continue;
                                    }
                                }
                            }
                            catch
                            {
                                CleanRemoveTcpClientFromList(tcpClient);
                                Debug.WriteLine("Cleaned failed tcp client (L)");
                            }
                        }
                    }
                    catch { }

                    //Delay the loop task
                    await Task.Delay(3000, vTask_SocketClean.TokenCancel);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed tcp client cleaner (L): " + ex.Message);
            }
        }

        //Remove tcp client from the list
        void CleanRemoveTcpClientFromList(TcpClient tcpClient)
        {
            try
            {
                vTcpClients.Remove(tcpClient);
                tcpClient = null;
            }
            catch { }
        }
    }
}