using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;

namespace ArnoldVinkCode
{
    public partial class ProcessUwpFunctions
    {
        //Launch an uwp application manually
        public static async Task<Process> ProcessLauncherUwpAndWin32StoreAsync(string appUserModelId, string runArgument)
        {
            try
            {
                //Prepare the process launch
                Process TaskAction()
                {
                    try
                    {
                        //Show launching message
                        Debug.WriteLine("Launching UWP or Win32Store: " + appUserModelId + " / " + runArgument);

                        //Start the process
                        UWPActivationManager UWPActivationManager = new UWPActivationManager();
                        UWPActivationManager.ActivateApplication(appUserModelId, runArgument, UWPActivationManagerOptions.None, out int processId);

                        //Return process
                        Process returnProcess = Process.GetProcessById(processId);
                        Debug.WriteLine("Launched UWP or Win32Store process identifier: " + returnProcess.Id);
                        return returnProcess;
                    }
                    catch { }
                    Debug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + " / " + runArgument);
                    return null;
                };

                //Launch the process
                return await AVActions.TaskStartReturn(TaskAction);
            }
            catch { }
            Debug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + " / " + runArgument);
            return null;
        }

        //Get uwp ProcessMulti by window handle
        public static ProcessMulti GetUwpProcessMultiByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Get process from the window handle
                IntPtr threadWindowHandleEx = FindWindowEx(targetWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                if (threadWindowHandleEx != IntPtr.Zero)
                {
                    int processId = GetProcessIdFromWindowHandle(threadWindowHandleEx);
                    if (processId > 0)
                    {
                        Process uwpProcess = Process.GetProcessById(processId);
                        return ConvertProcessToProcessMulti(uwpProcess, null, null);
                    }
                }

                //Get process from the appx package
                string appUserModelId = GetAppUserModelIdFromWindowHandle(targetWindowHandle);
                return GetUwpProcessMultiByAppUserModelId(appUserModelId);
            }
            catch { }
            return null;
        }

        //Get uwp ProcessMulti by AppUserModelId
        public static ProcessMulti GetUwpProcessMultiByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                Package appPackage = GetUwpAppPackageByAppUserModelId(targetAppUserModelId);
                AppxDetails appxDetails = GetUwpAppxDetailsFromAppPackage(appPackage);
                return GetUwpProcessMultiByPackageAndAppxDetails(appPackage, appxDetails);
            }
            catch { }
            return null;
        }

        //Get uwp ProcessMulti by Package and AppxDetails
        public static ProcessMulti GetUwpProcessMultiByPackageAndAppxDetails(Package appPackage, AppxDetails appxDetails)
        {
            try
            {
                string targetAppUserModelId = appxDetails.AppUserModelId;
                string targetProcessName = Path.GetFileNameWithoutExtension(appxDetails.ExecutableAliasName);

                Process[] uwpProcesses = GetProcessesByNameOrTitle(targetProcessName, false, true);
                foreach (Process uwpProcess in uwpProcesses)
                {
                    try
                    {
                        string processAppUserModelId = GetAppUserModelIdFromProcess(uwpProcess);
                        if (processAppUserModelId == targetAppUserModelId)
                        {
                            //Debug.WriteLine(targetProcessName + "/Id" + uwpProcess.Id + "/App" + processAppUserModelId + "vs" + targetAppUserModelId);
                            return ConvertProcessToProcessMulti(uwpProcess, appPackage, appxDetails);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        //Close an uwp application by window handle
        public static async Task<bool> CloseProcessUwpByWindowHandleOrProcessId(string appName, int processId, IntPtr processWindowHandle)
        {
            try
            {
                if (processWindowHandle != IntPtr.Zero)
                {
                    //Show the process
                    await FocusProcessWindow(appName, processId, processWindowHandle, WindowShowCommand.None, false, false);
                    await Task.Delay(500);

                    //Close the process or app
                    return CloseProcessByWindowHandle(processWindowHandle);
                }
                else if (processId > 0)
                {
                    //Close the process or app
                    return KillProcessTreeById(processId, true);
                }
            }
            catch { }
            return false;
        }

        //Restart a uwp process or app
        public static async Task<Process> RestartProcessUwp(string processName, string processAppUserModelId, int processId, IntPtr processWindowHandle, string processArgument)
        {
            try
            {
                //Close the process or app
                await CloseProcessUwpByWindowHandleOrProcessId(processName, processId, processWindowHandle);
                await Task.Delay(1000);

                //Relaunch the process or app
                return await ProcessLauncherUwpAndWin32StoreAsync(processAppUserModelId, processArgument);
            }
            catch { }
            return null;
        }

        //Check if window is an uwp application
        public static bool CheckProcessIsUwp(IntPtr targetWindowHandle)
        {
            try
            {
                string classNamestring = GetClassNameFromWindowHandle(targetWindowHandle);
                return CheckClassNameIsUwp(classNamestring);
            }
            catch { }
            return false;
        }

        //Check if window is an uwp application
        public static bool CheckClassNameIsUwp(string classNamestring)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classNamestring) || classNamestring == "ApplicationFrameWindow" || classNamestring == "Windows.UI.Core.CoreWindow")
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Get all uwp application processes
        public static List<ProcessMulti> GetUwpAppProcesses()
        {
            List<ProcessMulti> processList = new List<ProcessMulti>();
            try
            {
                Process frameHostProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false, true);
                if (frameHostProcess != null)
                {
                    foreach (ProcessThread threadProcess in frameHostProcess.Threads)
                    {
                        try
                        {
                            //Process variables
                            bool processInterfaceChecked = false;
                            IntPtr processWindowHandle = IntPtr.Zero;

                            foreach (IntPtr threadWindowHandle in EnumThreadWindows(threadProcess.Id))
                            {
                                try
                                {
                                    //Get window class name
                                    string classNameString = GetClassNameFromWindowHandle(threadWindowHandle);

                                    //Get application process
                                    if (classNameString == "ApplicationFrameWindow")
                                    {
                                        processWindowHandle = threadWindowHandle;
                                    }

                                    //Check if process has interface
                                    if (classNameString == "MSCTFIME UI")
                                    {
                                        processInterfaceChecked = true;
                                    }
                                }
                                catch { }
                            }

                            //Add process
                            if (processInterfaceChecked && processWindowHandle != IntPtr.Zero)
                            {
                                ProcessMulti processMulti = GetUwpProcessMultiByWindowHandle(processWindowHandle);
                                if (processMulti != null)
                                {
                                    processList.Add(processMulti);
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return processList;
        }

        //Get uwp application window from AppUserModelId
        public static IntPtr GetUwpWindowFromAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                Process frameHostProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false, true);
                if (frameHostProcess != null)
                {
                    foreach (ProcessThread threadProcess in frameHostProcess.Threads)
                    {
                        try
                        {
                            foreach (IntPtr threadWindowHandle in EnumThreadWindows(threadProcess.Id))
                            {
                                try
                                {
                                    if (CheckProcessIsUwp(threadWindowHandle))
                                    {
                                        if (targetAppUserModelId == GetAppUserModelIdFromWindowHandle(threadWindowHandle))
                                        {
                                            return threadWindowHandle;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return IntPtr.Zero;
        }
    }
}