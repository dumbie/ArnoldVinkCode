using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessNtQueryInformation;
using static ArnoldVinkCode.ProcessUwpFunctions;

namespace ArnoldVinkCode
{
    public partial class ProcessFunctions
    {
        //Close open start menu, cortana or search
        public static async Task CloseOpenWindowsStartMenu(ProcessMulti foregroundProcess)
        {
            try
            {
                if (foregroundProcess.Name == "SearchUI")
                {
                    Debug.WriteLine("Start menu is currently open, pressing escape to close it.");
                    KeyPressReleaseSingle(KeysVirtual.Escape);
                    await Task.Delay(10);
                }
            }
            catch { }
        }

        //Close open Windows system menu
        public static async Task CloseOpenWindowsSystemMenu(ProcessMulti foregroundProcess)
        {
            try
            {
                Debug.WriteLine("Closing system menu for window: " + foregroundProcess.WindowHandle);
                SendMessage(foregroundProcess.WindowHandle, (int)WindowMessages.WM_CANCELMODE, 0, 0);
                await Task.Delay(10);
            }
            catch { }
        }

        //Close open Windows prompts
        public static async Task CloseOpenWindowsPrompts()
        {
            try
            {
                //Windows administrator consent prompt
                if (GetProcessByNameOrTitle("consent", false, true) != null)
                {
                    Debug.WriteLine("Windows administrator consent prompt is open, killing the process.");
                    bool closedProcess = CloseProcessesByNameOrTitle("consent", false, true);
                    await Task.Delay(500);
                    if (closedProcess)
                    {
                        KeyPressReleaseSingle(KeysVirtual.Escape);
                        await Task.Delay(10);
                    }
                }

                //Windows feature installation prompt
                if (GetProcessByNameOrTitle("fondue", false, true) != null)
                {
                    Debug.WriteLine("Windows feature installation prompt is open, killing the process.");
                    CloseProcessesByNameOrTitle("fondue", false, true);
                }
            }
            catch { }
        }

        //Focus on a process window
        public static async Task<bool> FocusProcessWindow(string processTitle, int processId, IntPtr processWindowHandle, WindowShowCommand windowShowCommand, bool setWindowState, bool setTempTopMost)
        {
            try
            {
                //Prepare the process focus
                async Task<bool> TaskAction()
                {
                    try
                    {
                        //Close open Windows prompts
                        await CloseOpenWindowsPrompts();

                        //Get the current focused application
                        ProcessMulti foregroundProcess = GetProcessMultiFromWindowHandle(GetForegroundWindow());

                        //Close open start menu, cortana or search
                        await CloseOpenWindowsStartMenu(foregroundProcess);

                        //Close open Windows system menu
                        await CloseOpenWindowsSystemMenu(foregroundProcess);

                        //Detect the previous window state
                        if (windowShowCommand == WindowShowCommand.None && setWindowState)
                        {
                            WindowPlacement processWindowState = new WindowPlacement();
                            GetWindowPlacement(processWindowHandle, ref processWindowState);
                            Debug.WriteLine("Detected the previous window state: " + processWindowState.windowFlags);
                            if (processWindowState.windowFlags == WindowFlags.RestoreToMaximized)
                            {
                                windowShowCommand = WindowShowCommand.ShowMaximized;
                            }
                            else
                            {
                                windowShowCommand = WindowShowCommand.Restore;
                            }
                        }

                        //Change the window state command
                        if (setWindowState)
                        {
                            ShowWindowAsync(processWindowHandle, windowShowCommand);
                            await Task.Delay(10);

                            ShowWindow(processWindowHandle, windowShowCommand);
                            await Task.Delay(10);
                        }

                        //Set the window as top most
                        if (setTempTopMost)
                        {
                            SetWindowPos(processWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)WindowSWP.NOMOVE | (int)WindowSWP.NOSIZE);
                            await Task.Delay(10);
                        }

                        //Retry to show the window
                        for (int i = 0; i < 2; i++)
                        {
                            try
                            {
                                //Allow changing window
                                AllowSetForegroundWindow(processId);
                                await Task.Delay(10);

                                //Bring window to top
                                BringWindowToTop(processWindowHandle);
                                await Task.Delay(10);

                                //Switch to the window
                                SwitchToThisWindow(processWindowHandle, true);
                                await Task.Delay(10);

                                //Focus on the window
                                UiaFocusWindowHandle(processWindowHandle);
                                await Task.Delay(10);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Process focus error: " + ex.Message);
                            }
                        }

                        //Disable the window as top most
                        if (setTempTopMost)
                        {
                            SetWindowPos(processWindowHandle, (IntPtr)WindowPosition.NoTopMost, 0, 0, 0, 0, (int)WindowSWP.NOMOVE | (int)WindowSWP.NOSIZE);
                            await Task.Delay(10);
                        }

                        //Return bool
                        Debug.WriteLine("Focused process window: " + processTitle + " WindowHandle: " + processWindowHandle + " ShowCmd: " + windowShowCommand);
                        return true;
                    }
                    catch { }
                    Debug.WriteLine("Failed focusing process: " + processTitle);
                    return false;
                };

                //Focus the process
                return await AVActions.TaskStartReturn(TaskAction).Result;
            }
            catch { }
            Debug.WriteLine("Failed focusing process: " + processTitle);
            return false;
        }

        //Enumerate all thread windows including fullscreen
        public static List<IntPtr> EnumThreadWindows(int threadId)
        {
            IntPtr childWindow = IntPtr.Zero;
            List<IntPtr> listIntPtr = new List<IntPtr>();
            try
            {
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        if (GetWindowThreadProcessId(childWindow, out int processId) == threadId)
                        {
                            listIntPtr.Add(childWindow);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return listIntPtr;
        }

        //Check if a specific process is running by id
        public static bool CheckRunningProcessById(int processId)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.Id == processId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by id: " + ex.Message);
                return false;
            }
        }

        //Check if a specific process is running by window handle
        public static bool CheckRunningProcessByWindowHandle(IntPtr windowHandle)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.MainWindowHandle == windowHandle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by handle: " + ex.Message);
                return false;
            }
        }

        //Check if a specific process is running by name
        public static bool CheckRunningProcessByNameOrTitle(string processName, bool windowTitle, bool exactName)
        {
            try
            {
                if (windowTitle)
                {
                    return Process.GetProcesses().Any(x => x.MainWindowTitle.ToLower().Contains(processName.ToLower()));
                }
                else
                {
                    processName = Path.GetFileNameWithoutExtension(processName);
                    if (exactName)
                    {
                        return Process.GetProcessesByName(processName).Any();
                    }
                    else
                    {
                        return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(processName.ToLower())).Any();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check process by name: " + ex.Message);
                return false;
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
                int WindowTextBuilderLength = GetWindowTextLength(targetWindowHandle) + 1;
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
                StringBuilder classNameBuilder = new StringBuilder(256);
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
                if (processId == -1 || processId == 0)
                {
                    //Debug.WriteLine("Process id 0, using GetProcessHandleFromHwnd as backup.");
                    processId = GetProcessId(GetProcessHandleFromHwnd(targetWindowHandle));
                }
            }
            catch { }
            return processId;
        }

        //Get multi process from window handle
        public static ProcessMulti GetProcessMultiFromWindowHandle(IntPtr targetWindowHandle)
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
                    return ConvertProcessToProcessMulti(process);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get multi process: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get single process by name or window title
        /// </summary>
        /// <param name="processName">Process name without extension</param>
        /// <param name="windowTitle">Search for window title?</param>
        /// <param name="exactName">Search for exact process or contain?</param>
        public static Process GetProcessByNameOrTitle(string processName, bool windowTitle, bool exactName)
        {
            try
            {
                if (windowTitle)
                {
                    return Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(processName.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).FirstOrDefault();
                }
                else
                {
                    if (exactName)
                    {
                        return Process.GetProcessesByName(processName).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).FirstOrDefault();
                    }
                    else
                    {
                        return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(processName.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get process by name: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get multiple processes by name or window title
        /// </summary>
        /// <param name="processName">Process name without extension</param>
        /// <param name="windowTitle">Search for window title?</param>
        /// <param name="exactName">Search for exact process or contain?</param>
        public static Process[] GetProcessesByNameOrTitle(string processName, bool windowTitle, bool exactName)
        {
            try
            {
                if (windowTitle)
                {
                    return Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(processName.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                }
                else
                {
                    if (exactName)
                    {
                        return Process.GetProcessesByName(processName).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                    }
                    else
                    {
                        return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(processName.ToLower())).OrderByDescending(x => x.MainWindowHandle != IntPtr.Zero).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get processes by name: " + ex.Message);
                return null;
            }
        }

        //Close processes by name or window title
        public static bool CloseProcessesByNameOrTitle(string processName, bool windowTitle, bool exactName)
        {
            try
            {
                bool processClosed = false;
                if (windowTitle)
                {
                    foreach (Process AllProcess in Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(processName.ToLower())))
                    {
                        KillProcessTreeById(AllProcess.Id, true);
                        processClosed = true;
                    }
                }
                else
                {
                    processName = Path.GetFileNameWithoutExtension(processName);
                    if (exactName)
                    {
                        foreach (Process AllProcess in Process.GetProcessesByName(processName))
                        {
                            KillProcessTreeById(AllProcess.Id, true);
                            processClosed = true;
                        }
                    }
                    else
                    {
                        foreach (Process AllProcess in Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(processName.ToLower())))
                        {
                            KillProcessTreeById(AllProcess.Id, true);
                            processClosed = true;
                        }
                    }
                }
                return processClosed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close processes by name: " + ex.Message);
                return false;
            }
        }

        //Close process by window handle
        public static bool CloseProcessByWindowHandle(IntPtr windowHandle)
        {
            try
            {
                SendMessage(windowHandle, (int)WindowMessages.WM_CLOSE, 0, 0);
                SendMessage(windowHandle, (int)WindowMessages.WM_QUIT, 0, 0);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close process by handle: " + ex.Message);
                return false;
            }
        }

        //Get threads from ProcessMulti
        public static ProcessThreadCollection GetProcessThreads(ProcessMulti processMulti)
        {
            try
            {
                return processMulti.Process.Threads;
            }
            catch { }
            return null;
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
                int Succes = AVInteropDll.GetPackageFullName(targetProcess.Handle, ref stringLength, stringBuilder);
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

        //Get the launch arguments from process
        public static string GetLaunchArgumentsFromProcess(Process targetProcess, string executablePath)
        {
            string launchArguments = string.Empty;
            try
            {
                string removeFromArgument = '"' + executablePath + '"';
                USER_PROCESS_PARAMETERS processParameter = USER_PROCESS_PARAMETERS.CommandLine;
                launchArguments = GetProcessParameterString(targetProcess.Id, processParameter);
                launchArguments = AVFunctions.StringReplaceFirst(launchArguments, removeFromArgument, string.Empty, true);
                launchArguments = AVFunctions.StringRemoveStart(launchArguments, " ");
            }
            catch { }
            return launchArguments;
        }

        //Check if process is in suspended state
        public static bool CheckProcessSuspended(ProcessThreadCollection threadCollection)
        {
            try
            {
                //Debug.WriteLine("Checking suspend state for process: " + targetProcess.ProcessName + "/" + targetProcess.Id);
                ProcessThread processThread = threadCollection[0];
                if (processThread.ThreadState == ThreadState.Wait && processThread.WaitReason == ThreadWaitReason.Suspended)
                {
                    //Debug.WriteLine("The process main thread is currently suspended.");
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Check if process is active
        public static bool ValidateProcessState(Process targetProcess, bool checkSuspended, bool checkWin32)
        {
            try
            {
                //Check if the application is suspended
                if (checkSuspended)
                {
                    if (CheckProcessSuspended(targetProcess.Threads))
                    {
                        //Debug.WriteLine("Application is suspended and can't be shown or hidden.");
                        return false;
                    }
                }

                //Check if the application is win32
                if (checkWin32)
                {
                    if (CheckProcessIsUwp(targetProcess.MainWindowHandle))
                    {
                        //Debug.WriteLine("Application is an uwp application.");
                        return false;
                    }
                }

                return true;
            }
            catch { }
            return false;
        }

        //Check if window handle is a window
        public static bool ValidateWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Check if is a window
                if (!IsWindow(targetWindowHandle))
                {
                    //Debug.WriteLine("Window handle is not a Window.");
                    return false;
                }

                //Check if window is visible
                if (!IsWindowVisible(targetWindowHandle))
                {
                    //Debug.WriteLine("Window handle is not visible.");
                    return false;
                }

                //Check if application is hidden to the tray
                WindowPlacement ProcessWindowState = new WindowPlacement();
                GetWindowPlacement(targetWindowHandle, ref ProcessWindowState);
                if (ProcessWindowState.windowShowCommand <= 0)
                {
                    //Debug.WriteLine("Application is in the tray and can't be shown or hidden.");
                    return false;
                }

                ////Check if the window size is not zero
                //WindowRectangle PositionRect = new WindowRectangle();
                //GetWindowRect(TargetWindowHandle, ref PositionRect);
                //int WindowWidth = PositionRect.Right - PositionRect.Left;
                //int WindowHeight = PositionRect.Bottom - PositionRect.Top;
                //if (WindowWidth < 25 && WindowHeight < 25)
                //{
                //    Debug.WriteLine("Window is too small to be a proper window.");
                //    return false;
                //}

                ////Check the process window style
                //WindowStyles CurrentStyle = (WindowStyles)GetWindowLongAuto(TargetWindowHandle, (int)WindowLongFlags.GWL_STYLE).ToInt64();
                //if (!CurrentStyle.HasFlag(WindowStyles.WS_VISIBLE))
                //{
                //    Debug.WriteLine("Handle is missing WS_VISIBLE and can't be shown.");
                //    return false;
                //}

                return true;
            }
            catch { }
            return false;
        }

        //Check if a procress is running as administrator
        public static bool IsProcessRunningAsAdmin(Process targetProcess)
        {
            try
            {
                IntPtr tokenHandle = IntPtr.Zero;
                try
                {
                    OpenProcessToken(targetProcess.Handle, DesiredAccessFlags.TOKEN_ADJUST_DEFAULT, out tokenHandle);
                    CloseHandle(tokenHandle);
                    return false;
                }
                catch
                {
                    CloseHandle(tokenHandle);
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Convert Process to a ProcessMulti
        public static ProcessMulti ConvertProcessToProcessMulti(Process convertProcess)
        {
            ProcessMulti convertedProcess = new ProcessMulti();
            try
            {
                //Set process
                convertedProcess.Process = convertProcess;

                //Set identifier
                convertedProcess.Identifier = convertProcess.Id;

                //Set process name
                convertedProcess.Name = convertProcess.ProcessName;

                //Set window handle
                convertedProcess.WindowHandle = convertProcess.MainWindowHandle;

                //Get window title
                convertedProcess.WindowTitle = GetWindowTitleFromWindowHandle(convertProcess.MainWindowHandle);
                if (convertedProcess.WindowTitle == "Unknown")
                {
                    convertedProcess.WindowTitle = GetWindowTitleFromProcess(convertProcess);
                }

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

                //Check if application is Win32Store
                if (convertedProcess.Type == ProcessType.UWP)
                {
                    if (CheckProcessIsUwp(convertedProcess.ClassName))
                    {
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

        //Kill process and tree by id
        public static bool KillProcessTreeById(int processId, bool killTree)
        {
            try
            {
                //Kill tree processes
                if (killTree)
                {
                    using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + processId))
                    {
                        using (ManagementObjectCollection objCollection = objSearcher.Get())
                        {
                            foreach (ManagementObject objProcess in objCollection)
                            {
                                KillProcessTreeById(Convert.ToInt32(objProcess["ProcessID"]), killTree);
                            }
                        }
                    }
                }

                //Kill parent process
                Process.GetProcessById(processId).Kill();

                Debug.WriteLine("Killed process tree: " + processId);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to kill process tree: " + ex.Message);
                return false;
            }
        }
    }
}