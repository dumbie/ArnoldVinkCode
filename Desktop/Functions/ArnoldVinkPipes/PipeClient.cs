using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkPipes
    {
        //Send string to pipe server
        public static async Task PipeClientSendString(string serverName, int sendTimeout, string sendString)
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(serverName))
                {
                    await pipeClient.ConnectAsync(sendTimeout);
                    using (StreamWriter streamWriter = new StreamWriter(pipeClient))
                    {
                        await streamWriter.WriteLineAsync(sendString);
                        await streamWriter.FlushAsync();
                    }
                }
                //Debug.WriteLine("Sended string '" + sendString + "' to pipe server '" + serverName + "' (C) ");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending string to pipe server (C): " + ex.Message);
            }
        }
    }
}