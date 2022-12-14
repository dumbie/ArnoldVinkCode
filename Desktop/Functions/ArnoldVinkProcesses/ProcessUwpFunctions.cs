using System;
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

                        //Get detailed application information
                        Package appPackage = GetUwpAppPackageByAppUserModelId(appUserModelId);
                        AppxDetails appxDetails = GetUwpAppxDetailsFromAppPackage(appPackage);
                        appUserModelId = appxDetails.AppUserModelId;

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
                        return ConvertProcessToProcessMulti(uwpProcess);
                    }
                }

                //Get process from the appx package
                string appUserModelId = GetAppUserModelIdFromWindowHandle(targetWindowHandle);
                Package appPackage = GetUwpAppPackageByAppUserModelId(appUserModelId);
                AppxDetails appxDetails = GetUwpAppxDetailsFromAppPackage(appPackage);
                return GetUwpProcessMultiByProcessNameAndAppUserModelId(Path.GetFileNameWithoutExtension(appxDetails.ExecutableAliasName), appUserModelId);
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
                return GetUwpProcessMultiByProcessNameAndAppUserModelId(Path.GetFileNameWithoutExtension(appxDetails.ExecutableAliasName), targetAppUserModelId);
            }
            catch { }
            return null;
        }

        //Get uwp ProcessMulti by ProcessName and AppUserModelId
        public static ProcessMulti GetUwpProcessMultiByProcessNameAndAppUserModelId(string targetProcessName, string targetAppUserModelId)
        {
            try
            {
                Process[] uwpProcesses = GetProcessesByNameOrTitle(targetProcessName, false, true);
                foreach (Process uwpProcess in uwpProcesses)
                {
                    try
                    {
                        string processAppUserModelId = GetAppUserModelIdFromProcess(uwpProcess);
                        if (processAppUserModelId == targetAppUserModelId)
                        {
                            //Debug.WriteLine(targetProcessName + "/Id" + uwpProcess.Id + "/App" + processAppUserModelId + "vs" + targetAppUserModelId);
                            return ConvertProcessToProcessMulti(uwpProcess);
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
                if (string.IsNullOrWhiteSpace(classNamestring) || classNamestring == "ApplicationFrameWindow" || classNamestring == "Windows.UI.Core.CoreWindow")
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Check if window is an uwp application
        public static bool CheckProcessIsUwp(string classNamestring)
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
                }
            }
            catch { }
            return IntPtr.Zero;
        }
    }
}