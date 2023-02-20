using System;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get multi process from window handle
        public static ProcessMulti ProcessMulti_GetFromWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                if (CheckProcessIsUwp(targetWindowHandle))
                {
                    return GetUwpProcessMultiByWindowHandle(targetWindowHandle);
                }
                else
                {
                    int processId = GetProcessIdFromWindowHandle(targetWindowHandle);
                    Process process = Process.GetProcessById(processId);
                    return ProcessMulti_GetFromProcess(process, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get multi process: " + ex.Message);
                return null;
            }
        }

        //Get multi process from process
        public static ProcessMulti ProcessMulti_GetFromProcess(Process convertProcess, Package uwpAppPackage, AppxDetails uwpAppxDetails)
        {
            ProcessMulti convertedProcess = new ProcessMulti();
            try
            {
                //Set process identifier
                convertedProcess.Identifier = convertProcess.Id;

                //Set process name
                convertedProcess.Name = convertProcess.ProcessName;

                //Set process starttime
                convertedProcess.StartTime = convertProcess.StartTime;

                //Set window handle
                convertedProcess.WindowHandle = convertProcess.MainWindowHandle;

                //Get window title
                convertedProcess.WindowTitle = GetWindowTitleFromProcess(convertProcess);

                //Get class name
                convertedProcess.ClassName = GetClassNameFromWindowHandle(convertProcess.MainWindowHandle);

                //Get executable path
                string executablePath = GetExecutablePathFromProcess(convertProcess);

                //Set executable name
                convertedProcess.ExecutableName = Path.GetFileName(executablePath);

                //Set launch argument
                convertedProcess.Argument = GetLaunchArgumentsFromProcess(convertProcess, executablePath);

                //Set type and path
                string processAppUserModelId = GetAppUserModelIdFromProcess(convertProcess);
                if (!string.IsNullOrWhiteSpace(processAppUserModelId))
                {
                    convertedProcess.Type = ProcessType.UWP;
                    convertedProcess.Path = processAppUserModelId;
                }
                else
                {
                    convertedProcess.Type = ProcessType.Win32;
                    convertedProcess.Path = executablePath;
                }

                //Check if application is UWP
                if (convertedProcess.Type == ProcessType.UWP)
                {
                    //Check if AppPackage is provided
                    if (uwpAppPackage == null || uwpAppxDetails == null)
                    {
                        convertedProcess.AppPackage = GetUwpAppPackageByAppUserModelId(processAppUserModelId);
                        convertedProcess.AppxDetails = GetUwpAppxDetailsFromAppPackage(convertedProcess.AppPackage);
                    }
                    else
                    {
                        convertedProcess.AppPackage = uwpAppPackage;
                        convertedProcess.AppxDetails = uwpAppxDetails;
                    }

                    //Check if application is Win32Store
                    if (CheckClassNameIsUwp(convertedProcess.ClassName))
                    {
                        convertedProcess.WindowTitle = convertedProcess.AppxDetails.DisplayName;
                        convertedProcess.WindowHandle = GetUwpWindowFromAppUserModelId(processAppUserModelId);
                    }
                    else
                    {
                        convertedProcess.Type = ProcessType.Win32Store;
                    }
                }

                //Debug.WriteLine("Identifier: " + convertedProcess.Identifier);
                //Debug.WriteLine("Type: " + convertedProcess.Type);
                //Debug.WriteLine("Name: " + convertedProcess.Name);
                //Debug.WriteLine("ExecutableName: " + convertedProcess.ExecutableName);
                //Debug.WriteLine("Path: " + convertedProcess.Path);
                //Debug.WriteLine("Argument: " + convertedProcess.Argument);
                //Debug.WriteLine("ClassName: " + convertedProcess.ClassName);
                //Debug.WriteLine("WindowTitle: " + convertedProcess.WindowTitle);
                //Debug.WriteLine("WindowHandle: " + convertedProcess.WindowHandle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to convert Process to ProcessMulti: " + ex.Message);
            }
            return convertedProcess;
        }
    }
}