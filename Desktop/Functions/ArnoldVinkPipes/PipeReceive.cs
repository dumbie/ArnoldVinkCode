﻿using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace ArnoldVinkCode
{
    public partial class ArnoldVinkPipes
    {
        //Pipe receive loop
        async Task PipeReceiveLoop()
        {
            try
            {
                while (TaskCheckLoop(vTask_PipeReceiveLoop))
                {
                    try
                    {
                        //Allow access from every user
                        PipeSecurity pipeSecurity = new PipeSecurity();
                        SecurityIdentifier pipeSecurityIdentity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                        PipeAccessRule pipeAccessRule = new PipeAccessRule(pipeSecurityIdentity, PipeAccessRights.ReadWrite, AccessControlType.Allow);
                        pipeSecurity.AddAccessRule(pipeAccessRule);

                        //Start pipe server and wait for connection
                        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(vPipeServerName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, pipeSecurity))
                        {
                            //Wait for connection from client
                            await pipeServer.WaitForConnectionAsync(vTask_PipeReceiveLoop.TokenSource.Token);

                            //Check if pipe can read and write
                            if (pipeServer == null || !pipeServer.CanRead || !pipeServer.CanWrite)
                            {
                                Debug.WriteLine("Pipe stream cannot read or write (S)");
                                continue;
                            }

                            //Get string from pipe client
                            using (StreamReader streamReader = new StreamReader(pipeServer))
                            {
                                string stringReceived = await streamReader.ReadLineAsync();
                                if (!string.IsNullOrWhiteSpace(stringReceived))
                                {
                                    //Debug.WriteLine("Received string '" + stringReceived + "' from pipe client on server '" + vPipeServerName + "' (S)");

                                    //Signal that string has arrived
                                    if (EventStringReceived != null)
                                    {
                                        EventStringReceived(stringReceived);
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}