using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVProcessTool
    {
        public static bool Show_ProcessIdHwnd(int processId, IntPtr processHwnd)
        {
            try
            {
                Debug.WriteLine("Process tool showing, id and window handle: " + processId + "/" + processHwnd);

                //Set process tool arguments
                string toolArguments = "-show -pid=" + processId + " -hwnd=" + processHwnd;

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                bool processResult = processTool.ExitCode == 0 ? false : true;

                Debug.WriteLine("Process tool showed, window handle: " + processHwnd + "/" + processResult);
                return processResult;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool showing failed: " + ex.Message);
                return false;
            }
        }

        public static bool Show_ProcessHwnd(IntPtr processHwnd)
        {
            try
            {
                Debug.WriteLine("Process tool showing, window handle: " + processHwnd);

                //Set process tool arguments
                string toolArguments = "-show -hwnd=" + processHwnd;

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                bool processResult = processTool.ExitCode == 0 ? false : true;

                Debug.WriteLine("Process tool showed, window handle: " + processHwnd + "/" + processResult);
                return processResult;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool showing failed: " + ex.Message);
                return false;
            }
        }

        public static bool Show_ProcessId(int processId)
        {
            try
            {
                Debug.WriteLine("Process tool showing, process id: " + processId);

                //Set process tool arguments
                string toolArguments = "-show -pid=" + processId;

                //Start the process tool
                Process processTool = new Process();
                processTool.StartInfo.FileName = "ProcessTool.exe";
                processTool.StartInfo.Arguments = toolArguments;
                processTool.StartInfo.CreateNoWindow = true;
                processTool.StartInfo.UseShellExecute = false;
                processTool.Start();
                processTool.WaitForExit(2000);
                bool processResult = processTool.ExitCode == 0 ? false : true;

                Debug.WriteLine("Process tool showed, process id: " + processId + "/" + processResult);
                return processResult;
            }
            catch (Exception ex)
            {
                Debug.Write("Process tool showing failed: " + ex.Message);
                return false;
            }
        }
    }
}