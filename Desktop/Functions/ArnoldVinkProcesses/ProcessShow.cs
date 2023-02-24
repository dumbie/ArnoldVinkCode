using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
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
                //Fix
                ////Windows administrator consent prompt
                //if (GetProcessByNameOrTitle("consent", false, true) != null)
                //{
                //    Debug.WriteLine("Windows administrator consent prompt is open, killing the process.");
                //    bool closedProcess = CloseProcessesByNameOrTitle("consent", false, true);
                //    await Task.Delay(500);
                //    if (closedProcess)
                //    {
                //        KeyPressReleaseSingle(KeysVirtual.Escape);
                //        await Task.Delay(10);
                //    }
                //}

                ////Windows feature installation prompt
                //if (GetProcessByNameOrTitle("fondue", false, true) != null)
                //{
                //    Debug.WriteLine("Windows feature installation prompt is open, killing the process.");
                //    CloseProcessesByNameOrTitle("fondue", false, true);
                //}
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
                        ProcessMulti foregroundProcess = ProcessMulti_GetByWindowHandle(GetForegroundWindow());

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
                            SetWindowPos(processWindowHandle, (IntPtr)SWP_WindowPosition.TopMost, 0, 0, 0, 0, (int)SWP_WindowFlags.NOMOVE | (int)SWP_WindowFlags.NOSIZE);
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
                            SetWindowPos(processWindowHandle, (IntPtr)SWP_WindowPosition.NoTopMost, 0, 0, 0, 0, (int)SWP_WindowFlags.NOMOVE | (int)SWP_WindowFlags.NOSIZE);
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
    }
}