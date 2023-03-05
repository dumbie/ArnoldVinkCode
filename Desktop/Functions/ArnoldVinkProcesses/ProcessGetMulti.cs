using System;
using System.IO;
using System.Linq;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get multi process for current process
        public static ProcessMulti Get_ProcessMultiCurrent()
        {
            try
            {
                return Get_ProcessMultiByProcessId(GetCurrentProcessId(), 0, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process for current process: " + ex.Message);
                return null;
            }
        }

        //Get multi process by window handle
        public static ProcessMulti Get_ProcessMultiByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if window handle is uwp application
                if (Check_WindowHandleIsUwpApp(targetWindowHandle))
                {
                    string appUserModelId = Detail_AppUserModelIdByWindowHandle(targetWindowHandle);
                    ProcessConnect processConnect = Get_ProcessesByAppUserModelId(appUserModelId).FirstOrDefault();
                    return Get_ProcessMultiByProcessId(processConnect.Identifier, processConnect.ParentIdentifier, processConnect.Handle);
                }
                else
                {
                    int processId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
                    return Get_ProcessMultiByProcessId(processId, 0, IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process by window handle: " + ex.Message);
                return null;
            }
        }

        //Get multi process by process id
        public static ProcessMulti Get_ProcessMultiByProcessId(int targetProcessId, int parentProcessId, IntPtr processHandle)
        {
            try
            {
                //Create process multi
                ProcessMulti convertedProcess = new ProcessMulti();

                //Set process identifier
                convertedProcess.Identifier = targetProcessId;

                //Open process and set handle
                convertedProcess.Handle = processHandle;
                if (convertedProcess.Handle == IntPtr.Zero)
                {
                    convertedProcess.Handle = Get_ProcessHandleById(convertedProcess.Identifier);
                }
                if (convertedProcess.Handle == IntPtr.Zero)
                {
                    AVDebug.WriteLine("GetProcessMultiByProcessId process handle is empty.");
                    return null;
                }

                //Set parent process identifier
                convertedProcess.ParentIdentifier = parentProcessId;
                if (convertedProcess.ParentIdentifier <= 0)
                {
                    convertedProcess.ParentIdentifier = Detail_ProcessParentIdByProcessHandle(convertedProcess.Handle);
                }

                //Set process starttime
                convertedProcess.StartTime = Detail_StartTimeByProcessHandle(convertedProcess.Handle);

                //Set window handle
                convertedProcess.WindowHandle = Detail_MainWindowHandleByProcessId(convertedProcess.Identifier);

                //Get window title
                convertedProcess.WindowTitle = Detail_WindowTitleByWindowHandle(convertedProcess.WindowHandle);

                //Get window class name
                convertedProcess.WindowClassName = Detail_ClassNameByWindowHandle(convertedProcess.WindowHandle);

                //Get executable path
                convertedProcess.ExePath = Detail_ExecutablePathByProcessHandle(convertedProcess.Handle);

                //Set executable name (with extension)
                convertedProcess.ExeName = Path.GetFileName(convertedProcess.ExePath);

                //Set executable name (no extension)
                convertedProcess.ExeNameNoExt = Path.GetFileNameWithoutExtension(convertedProcess.ExePath);

                //Set workpath argument
                convertedProcess.WorkPath = Detail_ApplicationParameterByProcessHandle(convertedProcess.Handle, PROCESS_PARAMETER_OPTIONS.CurrentDirectoryPath);

                //Set launch argument
                convertedProcess.Argument = Detail_ApplicationParameterByProcessHandle(convertedProcess.Handle, PROCESS_PARAMETER_OPTIONS.CommandLine);

                //Set app user model id
                convertedProcess.AppUserModelId = Detail_AppUserModelIdByProcessHandle(convertedProcess.Handle);

                //Check if application is UWP or Win32Store
                if (!string.IsNullOrWhiteSpace(convertedProcess.AppUserModelId))
                {
                    //Get AppPackage and AppxDetails
                    convertedProcess.AppPackage = GetUwpAppPackageByAppUserModelId(convertedProcess.AppUserModelId);
                    convertedProcess.AppxDetails = GetUwpAppxDetailsByAppPackage(convertedProcess.AppPackage);

                    //Check if application is Win32Store
                    if (Check_ClassNameIsUwpApp(convertedProcess.WindowClassName))
                    {
                        convertedProcess.Type = ProcessType.UWP;
                        convertedProcess.WindowTitle = convertedProcess.AppxDetails.DisplayName;
                        convertedProcess.WindowHandle = Get_WindowHandlesByAppUserModelId(convertedProcess.AppUserModelId).FirstOrDefault();
                    }
                    else
                    {
                        convertedProcess.Type = ProcessType.Win32Store;
                    }
                }
                else
                {
                    convertedProcess.Type = ProcessType.Win32;
                }

                //AVDebug.WriteLine("----------------");
                //AVDebug.WriteLine("Identifier: " + convertedProcess.Identifier);
                //AVDebug.WriteLine("Type: " + convertedProcess.Type);
                //AVDebug.WriteLine("Handle: " + convertedProcess.Handle);
                //AVDebug.WriteLine("AppUserModelId: " + convertedProcess.AppUserModelId);
                //AVDebug.WriteLine("ExeName: " + convertedProcess.ExeName);
                //AVDebug.WriteLine("ExeNameNoExt: " + convertedProcess.ExeNameNoExt);
                //AVDebug.WriteLine("ExePath: " + convertedProcess.ExePath);
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