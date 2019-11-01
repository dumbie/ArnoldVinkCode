using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSocketClass
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