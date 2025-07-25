using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using static ArnoldVinkCode.AVAssembly;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public partial class AVStartup
    {
        //Setup application defaults
        public static bool SetupDefaults(ProcessPriorityClasses priorityLevel, bool checkDoubleProcess)
        {
            try
            {
                Debug.WriteLine("Setting application defaults.");

                //Resolve missing assembly dll files
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveFile;

                //Get current process information
                ProcessMulti currentProcess = Get_ProcessMultiCurrent();

                //Check if application is already running
                if (checkDoubleProcess)
                {
                    List<ProcessMulti> activeProcesses = Get_ProcessesMultiByName(currentProcess.ExeNameNoExt, true);
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