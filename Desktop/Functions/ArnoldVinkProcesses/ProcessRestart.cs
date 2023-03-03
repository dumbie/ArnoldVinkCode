using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Restart process by id
        public static async Task<int> Restart_ProcessById(int processId, string newArgs, bool withoutArgs)
        {
            try
            {
                AVDebug.WriteLine("Restarting process by id: " + processId);

                //Get restart process
                Process restartProcess = Process.GetProcessById(processId);
                if (restartProcess == null)
                {
                    AVDebug.WriteLine("Failed to get restart process by id: " + processId);
                    return 0;
                }

                //Get multi process
                ProcessMulti processDetails = Get_ProcessMultiByProcessId(restartProcess.Id);

                //Check launch argument
                string launchArgument = string.Empty;
                if (!withoutArgs)
                {
                    launchArgument = newArgs;
                    if (string.IsNullOrWhiteSpace(launchArgument))
                    {
                        launchArgument = processDetails.Argument;
                    }
                }

                //Close current process
                Close_ProcessTreeById(processId);

                //Wait for process to have closed
                await Task.Delay(500);

                //Launch process
                if (processDetails.Type == ProcessType.UWP || processDetails.Type == ProcessType.Win32Store)
                {
                    return Launch_UwpApplication(processDetails.AppUserModelId, launchArgument);
                }
                else
                {
                    //Get process access status
                    ProcessAccess currentProcessAccess = Get_ProcessAccessStatus(processId, false);

                    //Prepare process launch
                    return Launch_Prepare(processDetails.ExePath, processDetails.WorkPath, launchArgument, false, currentProcessAccess.AdminAccess, currentProcessAccess.UiAccess);
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to restart process id: " + processId + "/" + ex.Message);
                return 0;
            }
        }
    }
}