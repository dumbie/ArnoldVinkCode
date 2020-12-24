using System;
using System.Net;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        [Serializable]
        public class SocketSendContainer
        {
            public string SourceIp = "127.0.0.1";
            public int SourcePort = 1000;
            public object Object = null;
        }

        [Serializable]
        public class UdpEndPointDetails
        {
            public bool Active = false;
            public IPEndPoint IPEndPoint = null;
            public long LastConnection = 0;
        }
    }
}