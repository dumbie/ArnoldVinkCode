using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVUwpAppx;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public class ProcessMulti
        {
            public string Action { get; set; } = string.Empty;

            public int Identifier { get; set; } = 0;

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
                            CachedHandle = Get_ProcessHandleById(Identifier);
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
                            //Check if application is UWP or Win32Store or Win32
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
                        if (CachedAppUserModelId == string.Empty)
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
                        if (CachedExeName == string.Empty)
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
                        if (CachedExeNameNoExt == string.Empty)
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
                        if (CachedExePath == string.Empty)
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
                        if (CachedWorkPath == string.Empty)
                        {
                            CachedWorkPath = Detail_ApplicationParameterByProcessHandle(Handle, PROCESS_PARAMETER_OPTIONS.CurrentDirectoryPath);
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
                        if (CachedArgument == string.Empty)
                        {
                            CachedArgument = Detail_ApplicationParameterByProcessHandle(Handle, PROCESS_PARAMETER_OPTIONS.CommandLine);
                        }
                    }
                    catch { }
                    return CachedArgument;
                }
            }

            private IntPtr CachedWindowHandle = IntPtr.Zero;
            public IntPtr WindowHandle
            {
                get
                {
                    try
                    {
                        if (CachedWindowHandle == IntPtr.Zero)
                        {
                            CachedWindowHandle = Detail_MainWindowHandleByProcessId(Identifier);
                            //Fix check uwp
                        }
                    }
                    catch { }
                    return CachedWindowHandle;
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
                            CachedWindowClassName = Detail_ClassNameByWindowHandle(WindowHandle);
                            //Fix check uwp
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
                    if (CustomWindowTitle == string.Empty)
                    {
                        return Detail_WindowTitleByWindowHandle(WindowHandle);
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
                        return Check_ProcessSuspendedByProcessId(Identifier);
                    }
                    catch { }
                    return false;
                }
            }

            public List<int> ThreadIdentifiers
            {
                get
                {
                    try
                    {
                        return Get_ThreadIdsByProcessId(Identifier);
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

            public void Debug()
            {
                try
                {
                    AVDebug.WriteLine("Identifier: " + Identifier);
                    AVDebug.WriteLine("IdentifierParent: " + IdentifierParent);
                    AVDebug.WriteLine("Handle: " + Handle);
                    AVDebug.WriteLine("Type: " + Type);
                    AVDebug.WriteLine("Priority: " + Priority);
                    AVDebug.WriteLine("AppUserModelId: " + AppUserModelId);
                    AVDebug.WriteLine("ExeName: " + ExeName);
                    AVDebug.WriteLine("ExeNameNoExt: " + ExeNameNoExt);
                    AVDebug.WriteLine("ExePath: " + ExePath);
                    AVDebug.WriteLine("WorkPath: " + WorkPath);
                    AVDebug.WriteLine("Argument: " + Argument);
                    AVDebug.WriteLine("WindowHandle: " + WindowHandle);
                    AVDebug.WriteLine("WindowClassName: " + WindowClassName);
                    AVDebug.WriteLine("WindowTitle: " + WindowTitle);
                    AVDebug.WriteLine("StartTime: " + StartTime);
                    AVDebug.WriteLine("RunTime: " + RunTime);
                    AVDebug.WriteLine("Suspended: " + Suspended);
                    AVDebug.WriteLine("Threads: " + ThreadIdentifiers.Count);
                    AVDebug.WriteLine("AppPackageName: " + AppPackage.DisplayName);
                    AVDebug.WriteLine("AppxDetailsName: " + AppxDetails.DisplayName);
                }
                catch { }
            }
        }
    }
}