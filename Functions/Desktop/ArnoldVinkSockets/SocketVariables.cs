using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Tasks
        private Task vTask_SocketServer;
        private CancellationTokenSource vTaskToken_SocketServer;
        public bool vIsServerRunning() { return vTaskToken_SocketServer != null && !vTaskToken_SocketServer.IsCancellationRequested; }

        //Events
        public delegate Task DelegateBytesReceived(TcpClient tcpClient, byte[] bytesReceived);
        public DelegateBytesReceived EventBytesReceived = null;

        //Tcp clients
        private List<TcpClient> vTcpClients = new List<TcpClient>();
        private bool vCreatingClient = false;

        //Tcp listener
        private TcpListener vTcpListener = null;

        //Variables
        public string vTcpListenerIp = "127.0.0.1";
        public int vTcpListenerPort = 1000;
        public int vTcpListenerTimeout = 3000;
        public int vTcpClientTimeout = 2000;
    }
}