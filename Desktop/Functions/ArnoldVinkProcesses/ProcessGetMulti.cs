using System;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get multi process by UWP AppUserModelId
        public static ProcessMulti Get_ProcessMultiByUwpAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                Package appPackage = GetUwpAppPackageByAppUserModelId(targetAppUserModelId);
                AppxDetails appxDetails = GetUwpAppxDetailsByAppPackage(appPackage);
                return Get_ProcessMultiByUwpPackageAndAppxDetails(appPackage, appxDetails);
            }
            catch { }
            return null;
        }

        //Get multi process by UWP Package and AppxDetails
        public static ProcessMulti Get_ProcessMultiByUwpPackageAndAppxDetails(Package targetAppPackage, AppxDetails targetAppxDetails)
        {
            try
            {
                string targetAppUserModelId = targetAppxDetails.AppUserModelId;
                string targetProcessName = Path.GetFileNameWithoutExtension(targetAppxDetails.ExecutableAliasName);

                Process[] uwpProcesses = Get_ProcessesByName(targetProcessName, true);
                foreach (Process uwpProcess in uwpProcesses)
                {
                    try
                    {
                        string processAppUserModelId = Detail_ApplicationUserModelIdByProcess(uwpProcess);
                        if (processAppUserModelId == targetAppUserModelId)
                        {
                            //AVDebug.WriteLine(targetProcessName + "/Id" + uwpProcess.Id + "/App" + processAppUserModelId + "vs" + targetAppUserModelId);
                            return Get_ProcessMultiByProcess(uwpProcess, targetAppPackage, targetAppxDetails);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        //Get multi process by window handle
        public static ProcessMulti Get_ProcessMultiByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if window handle is uwp application
                if (Check_WindowHandleIsUwpApp(targetWindowHandle))
                {
                    IntPtr threadWindowHandleEx = FindWindowEx(targetWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                    if (threadWindowHandleEx != IntPtr.Zero)
                    {
                        //Get process from the window handle
                        int processId = Detail_ProcessIdByWindowHandle(threadWindowHandleEx);
                        if (processId > 0)
                        {
                            Process uwpProcess = Process.GetProcessById(processId);
                            return Get_ProcessMultiByProcess(uwpProcess, null, null);
                        }
                    }

                    //Get process from the appx package
                    string appUserModelId = Detail_ApplicationUserModelIdByWindowHandle(targetWindowHandle);
                    return Get_ProcessMultiByUwpAppUserModelId(appUserModelId);
                }
                else
                {
                    int processId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
                    Process process = Process.GetProcessById(processId);
                    return Get_ProcessMultiByProcess(process, null, null);
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process by window handle: " + ex.Message);
                return null;
            }
        }

        //Get multi process by process id
        public static ProcessMulti Get_ProcessMultiByProcessId(int targetProcessId)
        {
            try
            {
                Process targetProcess = Process.GetProcessById(targetProcessId);
                return Get_ProcessMultiByProcess(targetProcess, null, null);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process by id: " + ex.Message);
                return null;
            }
        }

        //Get multi process by process
        public static ProcessMulti Get_ProcessMultiByProcess(Process targetProcess, Package uwpAppPackage, AppxDetails uwpAppxDetails)
        {
            try
            {
                //Create process multi
                ProcessMulti convertedProcess = new ProcessMulti();

                //Set process identifier
                convertedProcess.Identifier = targetProcess.Id;

                //Set process handle
                convertedProcess.Handle = targetProcess.Handle;

                //Set process name
                convertedProcess.Name = targetProcess.ProcessName;

                //Set process starttime
                convertedProcess.StartTime = targetProcess.StartTime;

                //Set window handle
                convertedProcess.WindowHandle = targetProcess.MainWindowHandle;

                //Get window title
                convertedProcess.WindowTitle = Detail_WindowTitleByProcess(targetProcess);

                //Get window class name
                convertedProcess.WindowClassName = Detail_ClassNameByWindowHandle(targetProcess.MainWindowHandle);

                //Get executable path
                convertedProcess.ExecutablePath = Detail_ExecutablePathByProcess(targetProcess);

                //Set executable name
                convertedProcess.ExecutableName = Path.GetFileName(convertedProcess.ExecutablePath);

                //Set workpath argument
                convertedProcess.WorkPath = Detail_ApplicationParameterByProcessHandle(targetProcess.Handle, PROCESS_PARAMETER_OPTIONS.CurrentDirectoryPath);

                //Set launch argument
                convertedProcess.Argument = Detail_ApplicationParameterByProcessHandle(targetProcess.Handle, PROCESS_PARAMETER_OPTIONS.CommandLine);

                //Set application type and path
                string processAppUserModelId = Detail_ApplicationUserModelIdByProcess(targetProcess);
                if (!string.IsNullOrWhiteSpace(processAppUserModelId))
                {
                    convertedProcess.Type = ProcessType.UWP;
                    convertedProcess.AppUserModelId = processAppUserModelId;
                }
                else
                {
                    convertedProcess.Type = ProcessType.Win32;
                }

                //Check if application is UWP or Win32Store
                if (convertedProcess.Type == ProcessType.UWP)
                {
                    //Check if AppPackage is provided
                    if (uwpAppPackage == null)
                    {
                        convertedProcess.AppPackage = GetUwpAppPackageByAppUserModelId(processAppUserModelId);
                    }
                    else
                    {
                        convertedProcess.AppPackage = uwpAppPackage;
                    }

                    //Check if AppxDetails is provided
                    if (uwpAppxDetails == null)
                    {
                        convertedProcess.AppxDetails = GetUwpAppxDetailsByAppPackage(convertedProcess.AppPackage);
                    }
                    else
                    {
                        convertedProcess.AppxDetails = uwpAppxDetails;
                    }

                    //Check if application is Win32Store
                    if (Check_ClassNameIsUwpApp(convertedProcess.WindowClassName))
                    {
                        convertedProcess.WindowTitle = convertedProcess.AppxDetails.DisplayName;
                        convertedProcess.WindowHandle = Detail_UwpWindowHandleByAppUserModelId(processAppUserModelId);
                    }
                    else
                    {
                        convertedProcess.Type = ProcessType.Win32Store;
                    }
                }

                //AVDebug.WriteLine("Identifier: " + convertedProcess.Identifier);
                //AVDebug.WriteLine("ProcessType: " + convertedProcess.ProcessType);
                //AVDebug.WriteLine("ExecutableName: " + convertedProcess.ExecutableName);
                //AVDebug.WriteLine("ExecutablePath: " + convertedProcess.ExecutablePath);
                //AVDebug.WriteLine("Argument: " + convertedProcess.Argument);
                //AVDebug.WriteLine("WindowClassName: " + convertedProcess.WindowClassName);
                //AVDebug.WriteLine("WindowTitle: " + convertedProcess.WindowTitle);
                //AVDebug.WriteLine("WindowHandle: " + convertedProcess.WindowHandle);
                return convertedProcess;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to convert Process to ProcessMulti: " + ex.Message);
                return null;
            }
        }
    }
}