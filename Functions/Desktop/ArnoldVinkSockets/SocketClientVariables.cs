using System.Collections.Generic;
using System.Net.Sockets;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSocketClient
    {
        //Connection
        public List<TcpClient> vTcpClients = new List<TcpClient>();
    }
}