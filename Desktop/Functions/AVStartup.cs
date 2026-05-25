using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using static ArnoldVinkCode.AVAssembly;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public partial class AVStartup
    {
        //Setup application defaults
        public static bool SetupDefaults(ProcessPriorityClasses priorityLevel, bool checkDoubleProcess, bool setDebugPrivileges)
        {
            try
            {
                Debug.WriteLine("Setting application defaults.");

                //Resolve missing assembly dll files
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveFile;

                //Get current process information
                AVProcess currentProcess = Get_ProcessCurrent();

                //Check if application is already running
                if (checkDoubleProcess)
                {
                    List<AVProcess> activeProcesses = Get_ProcessByName(currentProcess.ExeNameNoExt, true);
                    if (activeProcesses.Count > 1)
                    {
                        Debug.WriteLine("Application " + currentProcess.ExeNameNoExt + " is already running, closing the process.");
                        Environment.Exit(0);
                        return false;
                    }
                }

                //Set working directory to executable directory
                AVFunctions.ApplicationUpdateWorkingPath();

                //Set application shutdown mode
                try
                {
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                }
                catch { }

                //Set application priority level
                currentProcess.Priority = priorityLevel;

                //Set application debug privileges
                if (setDebugPrivileges)
                {
                    currentProcess.SetPrivilege(PrivilegeConstants.SeDebugPrivilege, true);
                }

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