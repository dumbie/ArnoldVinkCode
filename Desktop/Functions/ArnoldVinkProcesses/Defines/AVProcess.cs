using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess : IDisposable
    {
        private int CachedIdentifier = 0;
        private int CachedIdentifierParent = 0;
        private IntPtr CachedHandle = IntPtr.Zero;
        private ProcessType CachedType = ProcessType.Unknown;
        private ProcessAccessStatus CachedAccessStatus = null;
        private string CachedAppUserModelId = string.Empty;
        private string CachedExeName = string.Empty;
        private string CachedExeNameNoExt = string.Empty;
        private string CachedExePath = string.Empty;
        private string CachedWorkPath = string.Empty;
        private string CachedArgument = string.Empty;
        private string CustomWindowTitleMain = string.Empty;
        private DateTime CachedStartTime = DateTime.MinValue;
        private Package CachedAppPackage = null;
        private AppxDetails CachedAppxDetails = null;

        public AVProcess(int identifier, int identifierParent = 0, string exeName = "")
        {
            CachedIdentifier = identifier;
            CachedIdentifierParent = identifierParent;
            CachedExeName = exeName;
        }

        public int Identifier
        {
            get
            {
                return CachedIdentifier;
            }
        }

        public int IdentifierParent
        {
            get
            {
                try
                {
                    if (CachedIdentifierParent <= 0)
                    {
                        CachedIdentifierParent = Detail_ProcessParentIdByProcessHandle(Handle);
                    }
                }
                catch { }
                return CachedIdentifierParent;
            }
        }

        public IntPtr Handle
        {
            get
            {
                try
                {
                    if (CachedHandle == IntPtr.Zero)
                    {
                        CachedHandle = Detail_ProcessHandleByProcessId(Identifier);
                    }
                }
                catch { }
                return CachedHandle;
            }
        }

        public ProcessType Type
        {
            get
            {
                try
                {
                    if (CachedType == ProcessType.Unknown)
                    {
                        if (!string.IsNullOrWhiteSpace(AppUserModelId))
                        {
                            if (Check_WindowClassNameIsUwpApp(WindowClassNameMain))
                            {
                                CachedType = ProcessType.UWP;
                            }
                            else
                            {
                                CachedType = ProcessType.Win32Store;
                            }
                        }
                        else
                        {
                            CachedType = ProcessType.Win32;
                        }
                    }
                }
                catch { }
                return CachedType;
            }
        }

        public ProcessAccessStatus AccessStatus
        {
            get
            {
                try
                {
                    if (CachedAccessStatus == null)
                    {
                        CachedAccessStatus = Detail_ProcessAccessStatusByProcessId(Identifier, false);
                    }
                }
                catch { }
                return CachedAccessStatus;
            }
        }

        public bool Responding
        {
            get
            {
                try
                {
                    return Detail_ProcessRespondingByWindowHandle(WindowHandleMain());
                }
                catch { }
                return true;
            }
        }

        public ProcessPriorityClasses Priority
        {
            get
            {
                try
                {
                    return GetPriorityClass(Handle);
                }
                catch { }
                return ProcessPriorityClasses.Unknown;
            }
            set
            {
                try
                {
                    bool prioritySet = SetPriorityClass(Handle, value);
                    AVDebug.WriteLine("Set process priority class: " + value + "/" + prioritySet);
                }
                catch { }
            }
        }

        public string AppUserModelId
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CachedAppUserModelId))
                    {
                        CachedAppUserModelId = Detail_AppUserModelIdByProcessHandle(Handle);
                    }
                }
                catch { }
                return CachedAppUserModelId;
            }
        }

        public string ExeName
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CachedExeName))
                    {
                        CachedExeName = Path.GetFileName(ExePath);
                    }
                }
                catch { }
                return CachedExeName;
            }
        }

        public string ExeNameNoExt
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CachedExeNameNoExt))
                    {
                        CachedExeNameNoExt = Path.GetFileNameWithoutExtension(ExeName);
                    }
                }
                catch { }
                return CachedExeNameNoExt;
            }
        }

        public string ExePath
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CachedExePath))
                    {
                        CachedExePath = Detail_ExecutablePathByProcessHandle(Handle);
                    }
                }
                catch { }
                return CachedExePath;
            }
        }

        public string WorkPath
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CachedWorkPath))
                    {
                        CachedWorkPath = Detail_ParameterByProcessHandle(Handle, ProcessParameterOptions.CurrentDirectoryPath);
                    }
                }
                catch { }
                return CachedWorkPath;
            }
        }

        public string Argument
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CachedArgument))
                    {
                        CachedArgument = Detail_ParameterByProcessHandle(Handle, ProcessParameterOptions.CommandLine);
                    }
                }
                catch { }
                return CachedArgument;
            }
        }

        public IntPtr WindowHandleMain(bool checkVisibility = true)
        {
            IntPtr windowHandleMain = IntPtr.Zero;
            try
            {
                //Check process name
                if (!Check_WindowProcessNameIsValid(ExeName))
                {
                    return windowHandleMain;
                }

                //Get window handle
                if (!string.IsNullOrWhiteSpace(AppUserModelId))
                {
                    windowHandleMain = Detail_WindowHandleMainByAppUserModelId(AppUserModelId, checkVisibility);
                }
                if (windowHandleMain == IntPtr.Zero)
                {
                    windowHandleMain = Detail_WindowHandleMainByProcessId(Identifier, checkVisibility);
                }
            }
            catch { }
            return windowHandleMain;
        }

        public List<IntPtr> WindowHandles
        {
            get
            {
                try
                {
                    if (Type == ProcessType.UWP)
                    {
                        return Detail_WindowHandlesByAppUserModelId(AppUserModelId);
                    }
                    else
                    {
                        return Detail_WindowHandlesByProcessId(Identifier);
                    }
                }
                catch { }
                return null;
            }
        }

        public string WindowClassNameMain
        {
            get
            {
                try
                {
                    return Detail_ClassNameByWindowHandle(WindowHandleMain());
                }
                catch { }
                return string.Empty;
            }
        }

        public string WindowTitleMain
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CustomWindowTitleMain))
                {
                    string foundWindowTitle = Detail_WindowTitleByWindowHandle(WindowHandleMain());
                    if (foundWindowTitle == "Unknown")
                    {
                        return ExeNameNoExt + " window";
                    }
                    else
                    {
                        return foundWindowTitle;
                    }
                }
                else
                {
                    return CustomWindowTitleMain;
                }
            }
            set
            {
                CustomWindowTitleMain = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                try
                {
                    if (CachedStartTime == DateTime.MinValue)
                    {
                        CachedStartTime = Detail_ProcessStartTimeByProcessHandle(Handle);
                    }
                }
                catch { }
                return CachedStartTime;
            }
        }

        public TimeSpan RunTime
        {
            get
            {
                try
                {
                    //Get current and start time
                    DateTime current_time = DateTime.Now;
                    DateTime start_time = StartTime;

                    //Get time difference
                    return current_time.Subtract(start_time);
                }
                catch { }
                return TimeSpan.Zero;
            }
        }

        public bool Suspended
        {
            get
            {
                try
                {
                    return Detail_ProcessThreadsByProcessId(Identifier, true).FirstOrDefault().Suspended;
                }
                catch { }
                return false;
            }
        }

        public List<ProcessThreadInfo> Threads
        {
            get
            {
                try
                {
                    return Detail_ProcessThreadsByProcessId(Identifier, false);
                }
                catch { }
                return null;
            }
        }

        public Package AppPackage
        {
            get
            {
                try
                {
                    if (CachedAppPackage == null)
                    {
                        CachedAppPackage = GetUwpAppPackageByAppUserModelId(AppUserModelId);
                    }
                }
                catch { }
                return CachedAppPackage;
            }
        }

        public AppxDetails AppxDetails
        {
            get
            {
                try
                {
                    if (CachedAppxDetails == null)
                    {
                        CachedAppxDetails = GetUwpAppxDetailsByUwpAppPackage(AppPackage);
                    }
                }
                catch { }
                return CachedAppxDetails;
            }
        }

        public bool Validate()
        {
            try
            {
                return Identifier > 0 && !string.IsNullOrWhiteSpace(ExePath);
            }
            catch { }
            return false;
        }

        public void Debug()
        {
            try
            {
                AVDebug.WriteLine("Identifier: " + Identifier);
                AVDebug.WriteLine("IdentifierParent: " + IdentifierParent);
                AVDebug.WriteLine("Handle: " + Handle);
                AVDebug.WriteLine("Type: " + Type);
                AVDebug.WriteLine("AdminAccess: " + AccessStatus.AdminAccess);
                AVDebug.WriteLine("Responding: " + Responding);
                AVDebug.WriteLine("Priority: " + Priority);
                AVDebug.WriteLine("AppUserModelId: " + AppUserModelId);
                AVDebug.WriteLine("ExeName: " + ExeName);
                AVDebug.WriteLine("ExeNameNoExt: " + ExeNameNoExt);
                AVDebug.WriteLine("ExePath: " + ExePath);
                AVDebug.WriteLine("WorkPath: " + WorkPath);
                AVDebug.WriteLine("Argument: " + Argument);
                AVDebug.WriteLine("WindowHandleMain: " + WindowHandleMain());
                AVDebug.WriteLine("WindowHandles: " + WindowHandles.Count);
                AVDebug.WriteLine("WindowClassNameMain: " + WindowClassNameMain);
                AVDebug.WriteLine("WindowTitleMain: " + WindowTitleMain);
                AVDebug.WriteLine("StartTime: " + StartTime);
                AVDebug.WriteLine("RunTime: " + RunTime);
                AVDebug.WriteLine("Suspended: " + Suspended);
                AVDebug.WriteLine("Threads: " + Threads.Count);
                AVDebug.WriteLine("AppPackageName: " + AppPackage.DisplayName);
                AVDebug.WriteLine("AppxDetailsName: " + AppxDetails.DisplayName);
            }
            catch { }
        }

        public void Cache()
        {
            try
            {
                _ = Identifier;
                _ = IdentifierParent;
                _ = Handle;
                _ = Type;
                _ = AccessStatus;
                _ = Responding;
                _ = Priority;
                _ = AppUserModelId;
                _ = ExeName;
                _ = ExeNameNoExt;
                _ = ExePath;
                _ = WorkPath;
                _ = Argument;
                _ = WindowHandleMain();
                _ = WindowHandles;
                _ = WindowClassNameMain;
                _ = WindowTitleMain;
                _ = StartTime;
                _ = RunTime;
                _ = Suspended;
                _ = Threads;
                _ = AppPackage;
                _ = AppxDetails;
            }
            catch { }
        }

        ~AVProcess() { Dispose(); }
        public void Dispose()
        {
            try
            {
                if (CachedHandle != IntPtr.Zero)
                {
                    CloseHandle(CachedHandle);
                }
            }
            catch { }
        }
    }
}