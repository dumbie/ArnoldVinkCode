using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get multi process by UWP AppUserModelId
        public static ProcessMulti Get_ProcessMultiByAppUserModelId(string targetAppUserModelId)
        {
            try
            {
                Process uwpProcess = Get_ProcessesByAppUserModelId(targetAppUserModelId).FirstOrDefault();
                return Get_ProcessMultiByProcess(uwpProcess);
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
                            return Get_ProcessMultiByProcess(uwpProcess);
                        }
                    }

                    //Get process from the appx package
                    string appUserModelId = Detail_AppUserModelIdByWindowHandle(targetWindowHandle);
                    return Get_ProcessMultiByAppUserModelId(appUserModelId);
                }
                else
                {
                    int processId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
                    Process process = Process.GetProcessById(processId);
                    return Get_ProcessMultiByProcess(process);
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
                return Get_ProcessMultiByProcess(targetProcess);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process by id: " + ex.Message);
                return null;
            }
        }

        //Get multi process by process
        public static ProcessMulti Get_ProcessMultiByProcess(Process targetProcess)
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
                string processAppUserModelId = Detail_AppUserModelIdByProcess(targetProcess);
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
                    //Get AppPackage and AppxDetails
                    convertedProcess.AppPackage = GetUwpAppPackageByAppUserModelId(processAppUserModelId);
                    convertedProcess.AppxDetails = GetUwpAppxDetailsByAppPackage(convertedProcess.AppPackage);

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

                //AVDebug.WriteLine("----------------");
                //AVDebug.WriteLine("Identifier: " + convertedProcess.Identifier);
                //AVDebug.WriteLine("Type: " + convertedProcess.Type);
                //AVDebug.WriteLine("Handle: " + convertedProcess.Handle);
                //AVDebug.WriteLine("Name: " + convertedProcess.Name);
                //AVDebug.WriteLine("AppUserModelId: " + convertedProcess.AppUserModelId);
                //AVDebug.WriteLine("ExecutableName: " + convertedProcess.ExecutableName);
                //AVDebug.WriteLine("ExecutablePath: " + convertedProcess.ExecutablePath);
                //AVDebug.WriteLine("WorkPath: " + convertedProcess.WorkPath);
                //AVDebug.WriteLine("Argument: " + convertedProcess.Argument);
                //AVDebug.WriteLine("WindowClassName: " + convertedProcess.WindowClassName);
                //AVDebug.WriteLine("WindowTitle: " + convertedProcess.WindowTitle);
                //AVDebug.WriteLine("WindowHandle: " + convertedProcess.WindowHandle);
                //AVDebug.WriteLine("StartTime: " + convertedProcess.StartTime);
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