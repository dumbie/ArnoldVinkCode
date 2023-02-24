﻿using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVProcessTool
    {
        public static int Restart_ProcessId(int processId, string newArguments, bool withoutArguments)
        {
            try
            {
                Debug.WriteLine("Process tool restarting, process id: " + processId);

                //Set process tool arguments
                string toolArguments = "-restart -pid=" + processId;

                if (withoutArguments)
                {
                    toolArguments += " -withoutargs";
                }

                if (!string.IsNullOrWhiteSpace(newArguments))
                {
                    toolArguments += " -args=" + CommandLine_PrepareArgument(newArguments);
                }

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                int restartProcessId = processTool.ExitCode;

                Debug.WriteLine("Process tool restarted, process id: " + restartProcessId);
                return restartProcessId;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool restarting failed: " + ex.Message);
                return 0;
            }
        }
    }
}