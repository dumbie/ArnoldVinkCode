using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get uwp ProcessMulti by window handle
        public static ProcessMulti GetUwpProcessMultiByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if window handle is in corewindow
                IntPtr threadWindowHandleEx = FindWindowEx(targetWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                if (threadWindowHandleEx != IntPtr.Zero)
                {
                    //Get process from the window handle
                    int processId = GetProcessIdFromWindowHandle(threadWindowHandleEx);
                    if (processId > 0)
                    {
                        Process uwpProcess = Process.GetProcessById(processId);
                        return ProcessMulti_GetFromProcess(uwpProcess, null, null);
                    }
                }
                else
                {
                    //Get process from the appx package
                    string appUserModelId = GetAppUserModelIdFromWindowHandle(targetWindowHandle);
                    return GetUwpProcessMultiByAppUserModelId(appUserModelId);
                }
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
                            return ProcessMulti_GetFromProcess(uwpProcess, appPackage, appxDetails);
                        }
                    }
                    catch { }
                }
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

                            foreach (IntPtr threadWindowHandle in Thread_GetWindowHandles(threadProcess.Id))
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
                            foreach (IntPtr threadWindowHandle in Thread_GetWindowHandles(threadProcess.Id))
                            {
                                try
                                {
                                    if (CheckProcessIsUwp(threadWindowHandle))
                                    {
                                        string targetAppUserModelIdLower = targetAppUserModelId.ToLower();
                                        string foundAppUserModelIdLower = GetAppUserModelIdFromWindowHandle(threadWindowHandle).ToLower();
                                        if (targetAppUserModelIdLower == foundAppUserModelIdLower)
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