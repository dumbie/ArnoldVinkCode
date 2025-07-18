using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkPipes
    {
        //Initialize pipes
        public ArnoldVinkPipes(string serverName)
        {
            try
            {
                vPipeServerName = serverName;
            }
            catch { }
        }

        //Enable the pipe server
        public void PipeServerEnable()
        {
            try
            {
                //Start receive loop
                AVActions.TaskStartLoop(PipeReceiveLoop, vTask_PipeReceiveLoop);

                Debug.WriteLine("Pipe server is running as '" + vPipeServerName + "' (S)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to enable the pipe server (S): " + ex.Message);
            }
        }

        //Pipe server exception
        void PipeServerException(Exception ex)
        {
            try
            {
                Debug.WriteLine("Pipe server error (S): " + ex.Message);

                List<string> messageAnswers = new List<string>();
                messageAnswers.Add("Restart server");
                messageAnswers.Add("Cancel");

                string messageResult = AVMessageBox.Popup(null, "Pipe server error", ex.Message, messageAnswers);
                if (messageResult == "Restart server")
                {
                    PipeServerEnable();
                }
            }
            catch { }
        }
    }
}