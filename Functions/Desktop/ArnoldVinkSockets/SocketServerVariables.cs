using System.Threading;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkSocketServer
    {
        //Tasks
        private Task vTask_SocketServer;
        private CancellationTokenSource vTaskToken_SocketServer;
        private bool vIsServerRunning() { return vTaskToken_SocketServer != null && !vTaskToken_SocketServer.IsCancellationRequested; }

        //Events
        public delegate Task DelegateBytesReceived(byte[] bytesReceived);
        public DelegateBytesReceived EventBytesReceived = null;

        //Variables
        private bool vTcpListenerBusy = false;
        public string vTcpListenerIp = "127.0.0.1";
        public int vTcpListenerPort = 1010;
        public int vTcpListenerTimeout = 1000;
    }
}