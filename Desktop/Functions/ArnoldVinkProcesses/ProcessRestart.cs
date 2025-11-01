using System;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Restart process by process id
        public static async Task<bool> Restart_ProcessByProcessId(int processId, string newArgs, bool withoutArgs)
        {
            try
            {
                AVDebug.WriteLine("Restarting process by id: " + processId);

                //Get restart process
                ProcessMulti restartProcess = Get_ProcessMultiByProcessId(processId);
                if (restartProcess == null)
                {
                    AVDebug.WriteLine("Failed to get restart process by id: " + processId);
                    return false;
                }

                //Cache restart process
                restartProcess.Cache();

                //Check launch argument
                string launchArgument = string.Empty;
                if (!withoutArgs)
                {
                    launchArgument = newArgs;
                    if (string.IsNullOrWhiteSpace(launchArgument))
                    {
                        launchArgument = restartProcess.Argument;
                    }
                }

                //Close current process
                Close_ProcessTreeByProcessId(processId);

                //Wait for process to have closed
                await Task.Delay(500);

                //Launch process
                if (restartProcess.Type == ProcessType.UWP || restartProcess.Type == ProcessType.Win32Store)
                {
                    return Launch_UwpApplication(restartProcess.AppUserModelId, launchArgument);
                }
                else
                {
                    bool adminAccess = restartProcess.AccessStatus.AdminAccess;
                    return Launch_ShellExecute(restartProcess.ExePath, restartProcess.WorkPath, launchArgument, adminAccess);
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to restart process id: " + processId + "/" + ex.Message);
                return false;
            }
        }
    }
}