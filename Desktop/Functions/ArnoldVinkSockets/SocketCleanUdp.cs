using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Clean disconnected udp clients
        private async Task UdpCleanLoop()
        {
            try
            {
                Debug.WriteLine("Udp client cleaner is now running (L)");

                //Udp cleaner loop
                while (TaskCheckLoop(vTask_UdpCleanLoop))
                {
                    try
                    {
                        //Debug.WriteLine("Cleaning disconnected udp clients (L)");
                        lock (vUdpClients)
                        {
                            foreach (UdpEndPointDetails udpClient in vUdpClients.ToList())
                            {
                                try
                                {
                                    long lastActivity = GetSystemTicksMs() - udpClient.LastConnection;
                                    if (lastActivity >= vSocketCleanup)
                                    {
                                        UdpClientRemoveFromList(udpClient);
                                        Debug.WriteLine("Cleaned no longer used udp client (L)");
                                    }
                                }
                                catch
                                {
                                    UdpClientRemoveFromList(udpClient);
                                    Debug.WriteLine("Cleaned failed udp client (L)");
                                }
                            }
                        }
                    }
                    catch { }

                    //Delay the loop task
                    await TaskDelayLoop(3000, vTask_UdpCleanLoop);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed udp client cleaner (L): " + ex.Message);
            }
        }

        //Remove udp client from the list
        void UdpClientRemoveFromList(UdpEndPointDetails udpClient)
        {
            try
            {
                vUdpClients.Remove(udpClient);
                UdpClientDisconnect(udpClient);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed removing udp client from list (L): " + ex.Message);
            }
        }
    }
}