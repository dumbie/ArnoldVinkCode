using System;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkPipes
    {
        //Tasks
        public AVTaskDetails vTask_PipeReceiveLoop = new AVTaskDetails("vTask_PipeReceiveLoop");

        //Events
        public Action<string> EventStringReceived;

        //Variables
        private string vPipeServerName = string.Empty;
    }
}