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
                return Get_ProcessMultiByProcessId(GetCurrentProcessId());
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
                    return Get_ProcessesByAppUserModelId(appUserModelId).FirstOrDefault();
                }
                else
                {
                    int processId = Detail_ProcessIdByWindowHandle(targetWindowHandle);
                    return Get_ProcessMultiByProcessId(processId);
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed to get multi process by window handle: " + ex.Message);
                return null;
            }
        }

        //Get multi process by process id
        public static ProcessMulti Get_ProcessMultiByProcessId(int targetProcessId, int parentProcessId = -1)
        {
            try
            {
                //Create process multi
                ProcessMulti convertedProcess = new ProcessMulti();

                //Set process identifier
                convertedProcess.Identifier = targetProcessId;

                //Open process and set handle
                convertedProcess.Handle = Get_ProcessHandleById(convertedProcess.Identifier);

                //Check open process handle
                if (convertedProcess.Handle == IntPtr.Zero)
                {
                    return null;
                }

                //Set parent process identifier
                convertedProcess.ParentIdentifier = parentProcessId;
                if (convertedProcess.ParentIdentifier == -1)
                {
                    convertedProcess.ParentIdentifier = Detail_ProcessParentIdByProcessHandle(convertedProcess.Handle);
                }

                //Set process starttime
                convertedProcess.StartTime = Detail_StartTimeByProcessHandle(convertedProcess.Handle);

                //Set window handle
                convertedProcess.WindowHandle = Detail_MainWindowHandleByProcessThreads(convertedProcess.GetProcessThreads());

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


                return convertedProcess;


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
                        //IntPtr uwpWindowHandle = Detail_UwpWindowHandleByAppUserModelId(convertedProcess.AppUserModelId);
                        //if (Check_ValidWindowHandle(uwpWindowHandle))
                        //{
                        //    convertedProcess.WindowHandle = uwpWindowHandle;
                        //}
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
                //AVDebug.WriteLine("ExecutableName: " + convertedProcess.ExecutableName);
                //AVDebug.WriteLine("ExecutableNameNoExt: " + convertedProcess.ExecutableNameNoExt);
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