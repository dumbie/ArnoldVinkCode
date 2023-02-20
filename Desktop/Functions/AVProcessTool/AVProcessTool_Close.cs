using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVProcessTool
    {
        public static bool Close_ProcessTreeId(int processId)
        {
            try
            {
                Debug.WriteLine("Process tool closing, process tree id: " + processId);

                //Set process tool arguments
                string toolArguments = "-close -pid=" + processId;

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                bool processResult = processTool.ExitCode == 0 ? false : true;

                Debug.WriteLine("Process tool closed, process tree id: " + processId + "/" + processResult);
                return processResult;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool closing failed: " + ex.Message);
                return false;
            }
        }

        public static bool Close_ProcessName(string processName)
        {
            try
            {
                Debug.WriteLine("Process tool closing, process name: " + processName);

                //Set process tool arguments
                string toolArguments = "-close";

                if (!string.IsNullOrWhiteSpace(processName))
                {
                    toolArguments += " -pname=" + "\"" + processName + "\"";
                }

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                bool processResult = processTool.ExitCode == 0 ? false : true;

                Debug.WriteLine("Process tool closed, process name: " + processName + "/" + processResult);
                return processResult;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool closing failed: " + ex.Message);
                return false;
            }
        }

        public static bool Close_ProcessMessageHwnd(IntPtr processesHwnd)
        {
            try
            {
                Debug.WriteLine("Process tool closing, process hwnd: " + processesHwnd);

                //Set process tool arguments
                string toolArguments = "-close -hwnd=" + processesHwnd;

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                bool processResult = processTool.ExitCode == 0 ? false : true;

                Debug.WriteLine("Process tool closed, process hwnd: " + processesHwnd + "/" + processResult);
                return processResult;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool closing failed: " + ex.Message);
                return false;
            }
        }
    }
}