using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Tcp receive loop
        async Task TcpReceiveLoop()
        {
            try
            {
                while (TaskCheckLoop(vTask_TcpReceiveLoop))
                {
                    try
                    {
                        TcpClient tcpClient = await vTcpServer.AcceptTcpClientAsync();
                        if (tcpClient != null && tcpClient.Connected)
                        {
                            Debug.WriteLine("New tcp client connected from (S): " + tcpClient.Client.RemoteEndPoint);
                            async void TaskAction()
                            {
                                try
                                {
                                    await TcpClientHandler(tcpClient);
                                }
                                catch { }
                            }
                            await AVActions.TaskStart(TaskAction);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Handle incoming tcp client
        private async Task<bool> TcpClientHandler(TcpClient tcpClient)
        {
            try
            {
                //Check if the tcp client is connected
                while (tcpClient != null && tcpClient.Connected)
                {
                    try
                    {
                        //Check if the network stream can read and write
                        NetworkStream networkStream = tcpClient.GetStream();
                        if (networkStream == null || !networkStream.CanRead || !networkStream.CanWrite)
                        {
                            Debug.WriteLine("Network stream cannot read or write (S)");
                            break;
                        }

                        //Receive the data from the network stream
                        byte[] receivedBytes = new byte[tcpClient.ReceiveBufferSize];
                        int bytesReceivedLength = await networkStream.ReadAsync(receivedBytes, 0, receivedBytes.Length);
                        if (bytesReceivedLength > 0)
                        {
                            //Debug.WriteLine("Received bytes from tcp client (S): " + bytesReceivedLength);
                            //Signal that bytes have arrived
                            await EventBytesReceived(tcpClient, null, receivedBytes);
                        }
                        else
                        {
                            Debug.WriteLine("No more bytes received from tcp client (S)");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed reading bytes from tcp client (S) " + ex.Message);
                        break;
                    }
                }

                //Disconnect client
                TcpClientDisconnect(tcpClient);
                Debug.WriteLine("Finished tcp client session (S)");
                return true;
            }
            catch
            {
                TcpClientDisconnect(tcpClient);
                Debug.WriteLine("Failed tcp client session (S)");
                return true;
            }
        }
    }
}