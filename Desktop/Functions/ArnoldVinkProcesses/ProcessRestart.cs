﻿using System;
using System.Threading.Tasks;

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
                ProcessMulti restartProcess = Get_ProcessMultiByProcessId(processId, 0);
                if (restartProcess == null)
                {
                    AVDebug.WriteLine("Failed to get restart process by id: " + processId);
                    return 0;
                }

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
                Close_ProcessTreeById(processId);

                //Wait for process to have closed
                await Task.Delay(500);

                //Launch process
                if (restartProcess.Type == ProcessType.UWP || restartProcess.Type == ProcessType.Win32Store)
                {
                    return Launch_UwpApplication(restartProcess.AppUserModelId, launchArgument);
                }
                else
                {
                    //Get process access status
                    ProcessAccess currentProcessAccess = Get_ProcessAccessStatus(processId, false);

                    //Prepare process launch
                    return Launch_Prepare(restartProcess.ExePath, restartProcess.WorkPath, launchArgument, false, currentProcessAccess.AdminAccess, currentProcessAccess.UiAccess);
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