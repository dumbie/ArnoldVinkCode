using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public partial class AVStartup
    {
        //Setup application defaults
        public static bool SetupDefaults(ProcessPriority priorityLevel)
        {
            try
            {
                Debug.WriteLine("Setting application defaults.");

                //Get current process information
                ProcessMulti currentProcess = Get_ProcessMultiCurrent();
                List<ProcessMulti> activeProcesses = Get_ProcessesMultiByName(currentProcess.ExeNameNoExt, true);

                //Check if application is already running
                if (activeProcesses.Count > 1)
                {
                    Debug.WriteLine("Application " + currentProcess.ExeNameNoExt + " is already running, closing the process");
                    Environment.Exit(0);
                    return false;
                }

                //Set working directory to executable directory
                AVFunctions.ApplicationUpdateWorkingPath();

                //Set application shutdown mode
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                //Set application priority level
                currentProcess.Priority = priorityLevel;

                Debug.WriteLine("Application defaults setup complete.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Application defaults setup failed: " + ex.Message);
                return false;
            }
        }
    }
}