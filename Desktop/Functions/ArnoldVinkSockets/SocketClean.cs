using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Clean disconnected tcp clients
        private async void LoopTcpCleaner()
        {
            try
            {
                Debug.WriteLine("Tcp client cleaner is now running (L)");

                //Tcp cleaner loop
                while (vTask_SocketClean.Status == AVTaskStatus.Running)
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
                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed tcp client cleaner (L): " + ex.Message);
            }
            finally
            {
                vTask_SocketClean.Status = AVTaskStatus.Stopped;
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