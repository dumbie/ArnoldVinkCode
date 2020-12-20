using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Tasks
        public AVTaskDetails vTask_TcpCleanLoop = new AVTaskDetails();
        public AVTaskDetails vTask_TcpReceiveLoop = new AVTaskDetails();
        public AVTaskDetails vTask_UdpReceiveLoop = new AVTaskDetails();

        //Events
        public delegate Task DelegateBytesReceived(TcpClient tcpClient, IPEndPoint endPoint, byte[] bytesReceived);
        public DelegateBytesReceived EventBytesReceived = null;

        //Tcp server and clients
        private TcpListener vTcpServer = null;
        private List<TcpClient> vTcpClients = new List<TcpClient>();
        private bool vCreatingClient = false;

        //Udp server and clients
        private UdpClient vUdpServer = null;

        //Variables
        public string vSocketServerIp = "127.0.0.1";
        public int vSocketServerPort = 1000;
        public int vSocketTimeout = 3000;
        private bool vTcpServerEnabled = false;
        private bool vUdpServerEnabled = false;
    }
}