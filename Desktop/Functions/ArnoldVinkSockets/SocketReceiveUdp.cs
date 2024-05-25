using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Udp receive loop
        async Task UdpReceiveLoop()
        {
            try
            {
                while (TaskCheckLoop(vTask_UdpReceiveLoop))
                {
                    try
                    {
                        UdpReceiveResult receiveResult = await vUdpServer.ReceiveAsync();
                        byte[] receivedBytes = receiveResult.Buffer;
                        if (receivedBytes.Length > 0)
                        {
                            //Debug.WriteLine("Received bytes from udp client (S): " + receivedBytes.Length);

                            //Add udp endpoint to client list
                            UdpEndPointDetails updDetailsExisting = vUdpClients.FirstOrDefault(x => x.IPEndPoint.Address.ToString() == receiveResult.RemoteEndPoint.Address.ToString() && x.IPEndPoint.Port == receiveResult.RemoteEndPoint.Port);
                            if (updDetailsExisting == null)
                            {
                                Debug.WriteLine("Added new udp client endpoint: " + receiveResult.RemoteEndPoint.Address + ":" + receiveResult.RemoteEndPoint.Port);
                                UdpEndPointDetails updDetailsNew = new UdpEndPointDetails();
                                updDetailsNew.Active = true;
                                updDetailsNew.IPEndPoint = receiveResult.RemoteEndPoint;
                                updDetailsNew.LastConnection = GetSystemTicksMs();
                                vUdpClients.Add(updDetailsNew);

                                //Signal that bytes have arrived
                                if (EventBytesReceived != null)
                                {
                                    EventBytesReceived(null, updDetailsNew, receivedBytes);
                                }
                            }
                            else
                            {
                                //Debug.WriteLine("Updated existing udp client endpoint: " + receiveResult.RemoteEndPoint.Address + ":" + receiveResult.RemoteEndPoint.Port);
                                updDetailsExisting.LastConnection = GetSystemTicksMs();

                                //Signal that bytes have arrived
                                if (EventBytesReceived != null)
                                {
                                    EventBytesReceived(null, updDetailsExisting, receivedBytes);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}