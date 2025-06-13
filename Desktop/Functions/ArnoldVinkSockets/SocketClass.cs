using System;
using System.Net;
using static ArnoldVinkCode.AVClassConverters;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSockets
    {
        [Serializable]
        public class SocketSendContainer
        {
            public string SourceIp = "127.0.0.1";
            public int SourcePort = 1000;
            public string SendType = string.Empty;
            public object SendObject = null;

            public void SetObject(object obj)
            {
                SendObject = obj;
                SendType = obj.GetType().ToString();
            }

            public T GetObjectAsType<T>()
            {
                return ConvertObjectToType<T>(SendObject);
            }
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