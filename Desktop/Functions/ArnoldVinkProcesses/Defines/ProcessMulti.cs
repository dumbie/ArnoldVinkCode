using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Enumerators
        public enum ProcessMultiActions
        {
            Launch,
            Close,
            CloseAll,
            Restart,
            RestartAll,
            Select,
            NoAction,
            Cancel
        }

        //Classes
        public class ProcessMultiAction
        {
            public ProcessMultiActions Action { get; set; } = ProcessMultiActions.NoAction;
            public ProcessMulti ProcessMulti { get; set; } = null;
        }

        public class ProcessMulti : IDisposable
        {
            public ProcessMulti(int identifier, int identifierParent)
            {
                CachedIdentifier = identifier;
                CachedIdentifierParent = identifierParent;
            }

            private int CachedIdentifier = 0;
            public int Identifier
            {
                get
                {
                    return CachedIdentifier;
                }
            }

            private int CachedIdentifierParent = 0;
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
                set
                {
                    CachedIdentifierParent = value;
                }
            }

            private IntPtr CachedHandle = IntPtr.Zero;
            public IntPtr Handle
            {
                get
                {
                    try
                    {
                        if (CachedHandle == IntPtr.Zero)
                        {
                            CachedHandle = Get_ProcessHandleByProcessId(Identifier);
                        }
                    }
                    catch { }
                    return CachedHandle;
                }
            }

            private ProcessType CachedType = ProcessType.Unknown;
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
                                if (Check_ClassNameIsUwpApp(WindowClassName))
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

            private ProcessAccessStatus CachedAccessStatus = null;
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
                        return Detail_ProcessRespondingByWindowHandle(WindowHandleMain);
                    }
                    catch { }
                    return true;
                }
            }

            public ProcessPriority Priority
            {
                get
                {
                    try
                    {
                        return GetPriorityClass(Handle);
                    }
                    catch { }
                    return ProcessPriority.Unknown;
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

            private string CachedAppUserModelId = string.Empty;
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

            private string CachedExeName = string.Empty;
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

            private string CachedExeNameNoExt = string.Empty;
            public string ExeNameNoExt
            {
                get
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(CachedExeNameNoExt))
                        {
                            CachedExeNameNoExt = Path.GetFileNameWithoutExtension(ExePath);
                        }
                    }
                    catch { }
                    return CachedExeNameNoExt;
                }
            }

            private string CachedExePath = string.Empty;
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

            private string CachedWorkPath = string.Empty;
            public string WorkPath
            {
                get
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(CachedWorkPath))
                        {
                            CachedWorkPath = Detail_ParameterByProcessHandle(Handle, PROCESS_PARAMETER_OPTIONS.CurrentDirectoryPath);
                        }
                    }
                    catch { }
                    return CachedWorkPath;
                }
            }

            private string CachedArgument = string.Empty;
            public string Argument
            {
                get
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(CachedArgument))
                        {
                            CachedArgument = Detail_ParameterByProcessHandle(Handle, PROCESS_PARAMETER_OPTIONS.CommandLine);
                        }
                    }
                    catch { }
                    return CachedArgument;
                }
            }

            private IntPtr CachedWindowHandleMain = IntPtr.Zero;
            public IntPtr WindowHandleMain
            {
                get
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(AppUserModelId))
                        {
                            if (CachedWindowHandleMain == IntPtr.Zero)
                            {
                                CachedWindowHandleMain = Detail_MainWindowHandleByAppUserModelId(AppUserModelId);
                            }
                        }
                        if (CachedWindowHandleMain == IntPtr.Zero)
                        {
                            CachedWindowHandleMain = Detail_MainWindowHandleByProcessId(Identifier);
                        }
                    }
                    catch { }
                    return CachedWindowHandleMain;
                }
            }

            public List<IntPtr> WindowHandles
            {
                get
                {
                    try
                    {
                        if (Type == ProcessType.UWP)
                        {
                            return Get_WindowHandlesByAppUserModelId(AppUserModelId);
                        }
                        else
                        {
                            return Get_WindowHandlesByProcessId(Identifier);
                        }
                    }
                    catch { }
                    return null;
                }
            }

            private string CachedWindowClassName = string.Empty;
            public string WindowClassName
            {
                get
                {
                    try
                    {
                        if (CachedWindowClassName == string.Empty)
                        {
                            CachedWindowClassName = Detail_ClassNameByWindowHandle(WindowHandleMain);
                        }
                    }
                    catch { }
                    return CachedWindowClassName;
                }
            }

            private string CustomWindowTitle = string.Empty;
            public string WindowTitle
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(CustomWindowTitle))
                    {
                        string foundWindowTitle = Detail_WindowTitleByWindowHandle(WindowHandleMain);
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
                        return CustomWindowTitle;
                    }
                }
                set
                {
                    CustomWindowTitle = value;
                }
            }

            private DateTime CachedStartTime { get; set; } = DateTime.MinValue;
            public DateTime StartTime
            {
                get
                {
                    try
                    {
                        if (CachedStartTime == DateTime.MinValue)
                        {
                            CachedStartTime = Detail_StartTimeByProcessHandle(Handle);
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
                        return DateTime.Now.Subtract(StartTime);
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
                        return Get_ProcessThreadsByProcessId(Identifier, true).FirstOrDefault().Suspended;
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
                        return Get_ProcessThreadsByProcessId(Identifier, false);
                    }
                    catch { }
                    return null;
                }
            }

            private Package CachedAppPackage = null;
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

            private AppxDetails CachedAppxDetails = null;
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
                    AVDebug.WriteLine("WindowHandleMain: " + WindowHandleMain);
                    AVDebug.WriteLine("WindowHandles: " + WindowHandles.Count);
                    AVDebug.WriteLine("WindowClassName: " + WindowClassName);
                    AVDebug.WriteLine("WindowTitle: " + WindowTitle);
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
                    _ = WindowHandleMain;
                    _ = WindowHandles;
                    _ = WindowClassName;
                    _ = WindowTitle;
                    _ = StartTime;
                    _ = RunTime;
                    _ = Suspended;
                    _ = Threads;
                    _ = AppPackage;
                    _ = AppxDetails;
                }
                catch { }
            }

            ~ProcessMulti() { Dispose(); }
            public void Dispose()
            {
                try
                {
                    CloseHandleAuto(CachedHandle);
                    CloseHandleAuto(CachedWindowHandleMain);
                }
                catch { }
            }
        }
    }
}