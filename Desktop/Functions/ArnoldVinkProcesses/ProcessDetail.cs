using System;
using System.Runtime.InteropServices;
using System.Text;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Get process start time by process handle
        public static DateTime Detail_StartTimeByProcessHandle(IntPtr processHandle)
        {
            try
            {
                //Get process times
                GetProcessTimes(processHandle, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

                //Convert start time
                return DateTime.FromFileTime(lpCreationTime);
            }
            catch { }
            return DateTime.MinValue;
        }

        //Get process parent id by process handle
        public static int Detail_ProcessParentIdByProcessHandle(IntPtr targetProcessHandle)
        {
            try
            {
                PROCESS_BASIC_INFORMATION32 basicInformation = new PROCESS_BASIC_INFORMATION32();
                int readResult = NtQueryInformationProcess32(targetProcessHandle, PROCESS_INFO_CLASS.ProcessBasicInformation, ref basicInformation, (uint)Marshal.SizeOf(basicInformation), out _);
                if (readResult != 0)
                {
                    AVDebug.WriteLine("Failed to get parent processid: " + targetProcessHandle + "/Query failed.");
                    return -1;
                }
                return (int)basicInformation.InheritedFromUniqueProcessId;
            }
            catch
            {
                //AVDebug.WriteLine("Failed to get parent processid: " + targetProcessHandle + "/" + ex.Message);
                return -1;
            }
        }

        //Get window title by window handle
        public static string Detail_WindowTitleByWindowHandle(IntPtr targetWindowHandle)
        {
            string processTitle = "Unknown";
            try
            {
                if (targetWindowHandle == IntPtr.Zero)
                {
                    return processTitle;
                }

                int windowTextBuilderLength = GetWindowTextLength(targetWindowHandle);
                if (windowTextBuilderLength <= 0)
                {
                    return processTitle;
                }

                windowTextBuilderLength += 1;
                StringBuilder windowTextBuilder = new StringBuilder(windowTextBuilderLength);
                GetWindowText(targetWindowHandle, windowTextBuilder, windowTextBuilder.Capacity);
                string builderString = windowTextBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(builderString))
                {
                    processTitle = builderString.Trim();
                }
                else
                {
                    processTitle = "Unknown";
                }
            }
            catch { }
            return processTitle;
        }

        //Get window Z order by window handle
        public static int Detail_WindowZOrderByWindowHandle(IntPtr windowHandle)
        {
            int zOrder = -1;
            try
            {
                IntPtr zHandle = windowHandle;
                while (zHandle != IntPtr.Zero)
                {
                    zHandle = GetWindow(zHandle, GetWindowFlags.GW_HWNDPREV);
                    zOrder++;
                }
                //AVDebug.WriteLine("Window " + hWnd + " ZOrder: " + zOrder);
            }
            catch { }
            return zOrder;
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
                    //AVDebug.WriteLine("Process id 0, using GetProcessHandleFromHwnd as backup.");
                    processId = GetProcessId(GetProcessHandleFromHwnd(targetWindowHandle));
                }
            }
            catch { }
            return processId;
        }

        //Get full exe path by process handle
        public static string Detail_ExecutablePathByProcessHandle(IntPtr targetProcessHandle)
        {
            try
            {
                int stringLength = 1024;
                StringBuilder stringBuilder = new StringBuilder(stringLength);
                if (QueryFullProcessImageName(targetProcessHandle, 0, stringBuilder, ref stringLength))
                {
                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        //Get full package name by process handle
        public static string Detail_PackageFullNameByProcessHandle(IntPtr targetProcessHandle)
        {
            try
            {
                int stringLength = 1024;
                StringBuilder stringBuilder = new StringBuilder(stringLength);
                if (GetPackageFullName(targetProcessHandle, ref stringLength, stringBuilder) == 0)
                {
                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        //Get AppUserModelId by process handle
        public static string Detail_AppUserModelIdByProcessHandle(IntPtr targetProcessHandle)
        {
            try
            {
                int stringLength = 1024;
                StringBuilder stringBuilder = new StringBuilder(stringLength);
                if (GetApplicationUserModelId(targetProcessHandle, ref stringLength, stringBuilder) == 0)
                {
                    return stringBuilder.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        //Get AppUserModelId by window handle
        public static string Detail_AppUserModelIdByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                Guid propertyStoreGuid = typeof(IPropertyStore).GUID;
                SHGetPropertyStoreForWindow(targetWindowHandle, ref propertyStoreGuid, out IPropertyStore propertyStore);
                propertyStore.GetValue(ref PKEY_AppUserModel_ID, out PropertyVariant propertyVariant);
                return Marshal.PtrToStringUni(propertyVariant.pwszVal);
            }
            catch { }
            return string.Empty;
        }

        //Get main window handle by process id
        public static IntPtr Detail_MainWindowHandleByProcessId(int targetProcessId)
        {
            try
            {
                foreach (IntPtr windowHandle in Get_WindowHandlesByProcessId(targetProcessId))
                {
                    try
                    {
                        bool windowVisible = IsWindowVisible(windowHandle);
                        bool windowOwner = GetWindow(windowHandle, GetWindowFlags.GW_OWNER) == IntPtr.Zero;
                        if (windowVisible && windowOwner)
                        {
                            return windowHandle;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return IntPtr.Zero;
        }
    }
}