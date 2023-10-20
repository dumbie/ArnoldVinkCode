using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkPipes
    {
        //Tasks
        public AVTaskDetails vTask_PipeReceiveLoop = new AVTaskDetails("vTask_PipeReceiveLoop");

        //Events
        public delegate void DelegateStringReceived(string bytesReceived);
        public DelegateStringReceived EventStringReceived = null;

        //Variables
        private string vPipeServerName = string.Empty;
    }
}