using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Tasks
        public AVTaskDetails vTask_SocketServer = new AVTaskDetails();
        public AVTaskDetails vTask_SocketClean = new AVTaskDetails();

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
        public int vTcpClientTimeout = 3000;
    }
}