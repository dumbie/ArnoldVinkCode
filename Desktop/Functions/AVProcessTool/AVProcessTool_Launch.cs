using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVProcessTool
    {
        public static int Launch_Exe(string exePath, string workPath, string arguments, bool normalAccess, bool adminAccess, bool allowUiAccess)
        {
            try
            {
                Debug.WriteLine("Process tool launching, exe: " + exePath + " (" + arguments + ")");

                //Check executable path
                if (string.IsNullOrWhiteSpace(exePath))
                {
                    Debug.WriteLine("Launching process tool failed: No executable path.");
                    return 0;
                }

                //Set process tool arguments
                string toolArguments = "-exePath=" + CommandLine_PrepareArgument(exePath);

                if (!string.IsNullOrWhiteSpace(workPath))
                {
                    toolArguments += " -workPath=" + CommandLine_PrepareArgument(workPath);
                }

                if (!string.IsNullOrWhiteSpace(arguments))
                {
                    toolArguments += " -args=" + CommandLine_PrepareArgument(arguments);
                }

                if (normalAccess)
                {
                    toolArguments += " -normal";
                }
                else if (adminAccess)
                {
                    toolArguments += " -admin";
                }

                if (allowUiAccess)
                {
                    toolArguments += " -allowuiaccess";
                }

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                int processId = processTool.ExitCode;

                Debug.WriteLine("Process tool launched exe, process id: " + processId);
                return processId;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool launching exe failed: " + ex.Message);
                return 0;
            }
        }

        public static int Launch_Uwp(string appUserModelId, string arguments)
        {
            try
            {
                Debug.WriteLine("Process tool launching, uwp: " + appUserModelId + " (" + arguments + ")");

                //Check uwp application id
                if (string.IsNullOrWhiteSpace(appUserModelId))
                {
                    Debug.WriteLine("Launching process tool failed: No Windows application id.");
                    return 0;
                }

                //Set process tool arguments
                string toolArguments = "-uwp=" + CommandLine_PrepareArgument(appUserModelId);

                if (!string.IsNullOrWhiteSpace(arguments))
                {
                    toolArguments += " -args=" + CommandLine_PrepareArgument(arguments);
                }

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                int processId = processTool.ExitCode;

                Debug.WriteLine("Process tool launched uwp, process id: " + processId);
                return processId;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool launching uwp failed: " + ex.Message);
                return 0;
            }
        }
    }
}