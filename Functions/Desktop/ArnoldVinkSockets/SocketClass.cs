using System;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        //Socket send container class
        [Serializable]
        public class SocketSendContainer
        {
            public string SourceIp = "127.0.0.1";
            public int SourcePort = 1000;
            public object Object = null;
        }
    }
}