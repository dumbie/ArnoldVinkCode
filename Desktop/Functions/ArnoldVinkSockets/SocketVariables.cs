using System;
using System.Collections.Generic;
using System.Net.Sockets;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Tasks
        public AVTaskDetails vTask_TcpCleanLoop = new AVTaskDetails("vTask_TcpCleanLoop");
        public AVTaskDetails vTask_TcpReceiveLoop = new AVTaskDetails("vTask_TcpReceiveLoop");
        public AVTaskDetails vTask_UdpCleanLoop = new AVTaskDetails("vTask_UdpCleanLoop");
        public AVTaskDetails vTask_UdpReceiveLoop = new AVTaskDetails("vTask_UdpReceiveLoop");

        //Events
        public event Action<TcpClient, UdpEndPointDetails, byte[]> EventBytesReceived;

        //Tcp server and clients
        private TcpListener vTcpServer = null;
        private List<TcpClient> vTcpClients = new List<TcpClient>();
        private bool vCreatingClient = false;

        //Udp server and clients
        private UdpClient vUdpServer = null;
        private List<UdpEndPointDetails> vUdpClients = new List<UdpEndPointDetails>();

        //Variables
        public string vSocketServerIp = "127.0.0.1";
        public int vSocketServerPort = 1000;
        public int vSocketTimeout = 3000;
        public int vSocketCleanup = 6000;
        private bool vTcpServerEnabled = false;
        private bool vUdpServerEnabled = false;
    }
}