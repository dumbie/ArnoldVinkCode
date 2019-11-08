using System.Collections.Generic;
using System.Net.Sockets;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSocketClient
    {
        //Connections
        public List<TcpClient> vTcpClients = new List<TcpClient>();

        //Variables
        public int vTcpClientTimeout = 1000;
    }
}