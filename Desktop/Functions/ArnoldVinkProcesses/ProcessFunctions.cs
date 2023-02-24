using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get process parent id
        public static int Process_GetParentId(Process targetProcess)
        {
            try
            {
                PROCESS_BASIC_INFORMATION32 basicInformation = new PROCESS_BASIC_INFORMATION32();
                int readResult = NtQueryInformationProcess32(targetProcess.Handle, PROCESS_INFO_CLASS.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    Debug.WriteLine("Failed to get parent processid: " + targetProcess.Id + "/Query failed");
                    return -1;
                }
                return (int)basicInformation.InheritedFromUniqueProcessId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get parent processid: " + targetProcess.Id + "/" + ex.Message);
                return -1;
            }
        }

        //Get the window title from process
        public static string GetWindowTitleFromProcess(Process targetProcess)
        {
            string ProcessTitle = "Unknown";
            try
            {
                ProcessTitle = targetProcess.MainWindowTitle;
                if (string.IsNullOrWhiteSpace(ProcessTitle))
                {
                    ProcessTitle = GetWindowTitleFromWindowHandle(targetProcess.MainWindowHandle);
                }
                if (string.IsNullOrWhiteSpace(ProcessTitle) || ProcessTitle == "Unknown")
                {
                    ProcessTitle = targetProcess.ProcessName;
                }
                if (!string.IsNullOrWhiteSpace(ProcessTitle))
                {
                    ProcessTitle = AVFunctions.StringRemoveStart(ProcessTitle, " ");
                    ProcessTitle = AVFunctions.StringRemoveEnd(ProcessTitle, " ");
                }
                else
                {
                    ProcessTitle = "Unknown";
                }
            }
            catch { }
            return ProcessTitle;
        }

        //Get the window title from window handle
        public static string GetWindowTitleFromWindowHandle(IntPtr targetWindowHandle)
        {
            string ProcessTitle = "Unknown";
            try
            {
                int WindowTextBuilderLength = GetWindowTextLength(targetWindowHandle);
                if (WindowTextBuilderLength <= 0)
                {
                    return ProcessTitle;
                }

                WindowTextBuilderLength += 1;
                StringBuilder WindowTextBuilder = new StringBuilder(WindowTextBuilderLength);
                GetWindowText(targetWindowHandle, WindowTextBuilder, WindowTextBuilder.Capacity);
                string BuilderString = WindowTextBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(BuilderString))
                {
                    ProcessTitle = BuilderString;
                    ProcessTitle = AVFunctions.StringRemoveStart(ProcessTitle, " ");
                    ProcessTitle = AVFunctions.StringRemoveEnd(ProcessTitle, " ");
                }
                else
                {
                    ProcessTitle = "Unknown";
                }
            }
            catch { }
            return ProcessTitle;
        }

        //Get the class name from window handle
        public static string GetClassNameFromWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                StringBuilder classNameBuilder = new StringBuilder(1024);
                GetClassName(targetWindowHandle, classNameBuilder, classNameBuilder.Capacity);
                return classNameBuilder.ToString();
            }
            catch { }
            return string.Empty;
        }

        //Get process id from window handle
        public static int GetProcessIdFromWindowHandle(IntPtr targetWindowHandle)
        {
            int processId = -1;
            try
            {
                GetWindowThreadProcessId(targetWindowHandle, out processId);
            }
            catch { }
            try
            {
                if (processId <= 0)
                {
                    //Debug.WriteLine("Process id 0, using GetProcessHandleFromHwnd as backup.");
                    processId = GetProcessId(GetProcessHandleFromHwnd(targetWindowHandle));
                }
            }
            catch { }
            return processId;
        }

        /// <summary>
        /// Get process by identifier
        /// </summary>
        /// <param name="processId">Process identifier</param>
        public static Process GetProcessById(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get process by id: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get processes by name
        /// </summary>
        /// <param name="processName">Process name without extension</param>
        /// <param name="exactName">Search for exact process name</param>
        public static Process[] GetProcessesByName(string processName, bool exactName)
        {
            try
            {
                if (exactName)
                {
                    return Process.GetProcesses().Where(x => x.ProcessName.ToLower() == processName.ToLower()).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
                else
                {
                    return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(processName.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get processes by name: " + ex.Message);
                return new Process[0];
            }
        }

        /// <summary>
        /// Get processes by window title
        /// </summary>
        /// <param name="windowTitle">Search for window title</param>
        /// <param name="exactName">Search for exact window title</param>
        public static Process[] GetProcessesByWindowTitle(string windowTitle, bool exactName)
        {
            try
            {
                if (exactName)
                {
                    return Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower() == windowTitle.ToLower()).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
                else
                {
                    return Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(windowTitle.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get processes by window title: " + ex.Message);
                return new Process[0];
            }
        }

        //Get the full exe path from process
        public static string GetExecutablePathFromProcess(Process targetProcess)
        {
            try
            {
                return targetProcess.MainModule.FileName;
            }
            catch { }
            try
            {
                int stringLength = 1024;
                StringBuilder stringBuilder = new StringBuilder(stringLength);
                bool Succes = QueryFullProcessImageName(targetProcess.Handle, 0, stringBuilder, ref stringLength);
                if (Succes)
                {
                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        //Get the full package name from process
        public static string GetPackageFullNameFromProcess(Process targetProcess)
        {
            try
            {
                int stringLength = 1024;
                StringBuilder stringBuilder = new StringBuilder(stringLength);
                int Succes = GetPackageFullName(targetProcess.Handle, ref stringLength, stringBuilder);
                if (Succes == 0)
                {
                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        //Get the AppUserModelId from process
        public static string GetAppUserModelIdFromProcess(Process targetProcess)
        {
            try
            {
                int stringLength = 1024;
                StringBuilder stringBuilder = new StringBuilder(stringLength);
                int Succes = GetApplicationUserModelId(targetProcess.Handle, ref stringLength, stringBuilder);
                if (Succes == 0)
                {
                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        //Get the AppUserModelId from window handle
        public static string GetAppUserModelIdFromWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                PropertyVariant propertyVariant = new PropertyVariant();
                Guid propertyStoreGuid = typeof(IPropertyStore).GUID;

                SHGetPropertyStoreForWindow(targetWindowHandle, ref propertyStoreGuid, out IPropertyStore propertyStore);
                propertyStore.GetValue(ref PKEY_AppUserModel_ID, out propertyVariant);

                return Marshal.PtrToStringUni(propertyVariant.pwszVal);
            }
            catch { }
            return string.Empty;
        }
    }
}