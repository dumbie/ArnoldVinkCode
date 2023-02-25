using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get process parent id by process
        public static int Detail_ProcessParentIdByProcess(Process targetProcess)
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

        //Get window title by process
        public static string Detail_WindowTitleByProcess(Process targetProcess)
        {
            string ProcessTitle = "Unknown";
            try
            {
                ProcessTitle = targetProcess.MainWindowTitle;
                if (string.IsNullOrWhiteSpace(ProcessTitle))
                {
                    ProcessTitle = Detail_WindowTitleByWindowHandle(targetProcess.MainWindowHandle);
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

        //Get window title by window handle
        public static string Detail_WindowTitleByWindowHandle(IntPtr targetWindowHandle)
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

        //Get class name by window handle
        public static string Detail_ClassNameByWindowHandle(IntPtr targetWindowHandle)
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

        //Get process id by window handle
        public static int Detail_ProcessIdByWindowHandle(IntPtr targetWindowHandle)
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

        //Get full exe path by process
        public static string Detail_ExecutablePathByProcess(Process targetProcess)
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
                if (QueryFullProcessImageName(targetProcess.Handle, 0, stringBuilder, ref stringLength))
                {
                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        //Get full package name by process
        public static string Detail_PackageFullNameByProcess(Process targetProcess)
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

        //Get AppUserModelId by process
        public static string Detail_ApplicationUserModelIdByProcess(Process targetProcess)
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

        //Get AppUserModelId by window handle
        public static string Detail_ApplicationUserModelIdByWindowHandle(IntPtr targetWindowHandle)
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