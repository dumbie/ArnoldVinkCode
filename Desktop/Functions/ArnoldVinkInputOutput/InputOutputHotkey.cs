using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputHotkey
    {
        //Variables
        private static IntPtr vWindowHookPointer = IntPtr.Zero;
        private static LowLevelCallBackKeyboard vLowLevelKeyboardCallback = LowLevelKeyboardCallbackCode;

        //Settings
        public static bool BlockGlobalKeyboardPresses = false;

        //Lists
        private static List<KeysVirtual> vListKeysPressed = new List<KeysVirtual>();

        //Events
        public static event Action<KeyboardMessage> EventHotkeyPressedMessage;
        public static event Action<List<KeysVirtual>> EventHotkeyPressedList;

        //Tasks
        private static AVTaskDetails vTask_RestartKeyboardHook = new AVTaskDetails("vTask_RestartKeyboardHook");

        //Start receiving key input
        public static bool Start()
        {
            bool hooked = false;
            try
            {
                //Set window keyboard hook
                hooked = WindowKeyboardHook();

                //Start task detecting window change
                if (hooked)
                {
                    TaskStartLoop(vTaskLoop_RestartKeyboardHook, vTask_RestartKeyboardHook);
                }
            }
            catch { }
            return hooked;
        }

        //Stop receiving key input
        public static async Task Stop()
        {
            try
            {
                //Remove window keyboard hook
                WindowKeyboardUnhook();

                //Stop task detecting window change
                await TaskStopLoop(vTask_RestartKeyboardHook, 5000);
            }
            catch { }
        }

        //Restart receiving key input
        public static bool Restart()
        {
            bool hooked = false;
            try
            {
                WindowKeyboardUnhook();
                hooked = WindowKeyboardHook();
            }
            catch { }
            return hooked;
        }

        //Set window keyboard hook
        private static bool WindowKeyboardHook()
        {
            try
            {
                if (vWindowHookPointer != IntPtr.Zero)
                {
                    Debug.WriteLine("Window keyboard hook already set, unhook first.");
                    return false;
                }

                AVActions.DispatcherInvoke(delegate
                {
                    vWindowHookPointer = SetWindowsHookEx(WindowHookTypes.WH_KEYBOARD_LL, vLowLevelKeyboardCallback, IntPtr.Zero, 0);
                    Debug.WriteLine("Hooked window keyboard: " + vWindowHookPointer);
                });
            }
            catch { }
            return vWindowHookPointer != IntPtr.Zero;
        }

        //Remove window keyboard hook
        private static void WindowKeyboardUnhook()
        {
            try
            {
                if (vWindowHookPointer != IntPtr.Zero)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        bool unhooked = UnhookWindowsHookEx(vWindowHookPointer);
                        //Debug.WriteLine("Unhooked window keyboard: " + unhooked);
                    });
                    vWindowHookPointer = IntPtr.Zero;
                }
            }
            catch { }
        }

        //Task detecting window change
        private static async Task vTaskLoop_RestartKeyboardHook()
        {
            try
            {
                //Loop variables
                IntPtr previousForegroundWindow = GetForegroundWindow();

                while (TaskCheckLoop(vTask_RestartKeyboardHook))
                {
                    //Get foreground window
                    IntPtr currentForegroundWindow = GetForegroundWindow();

                    //Check foreground window change
                    if (previousForegroundWindow != currentForegroundWindow)
                    {
                        Restart();
                    }

                    //Update previous variables
                    previousForegroundWindow = currentForegroundWindow;

                    //Delay the loop task
                    await TaskDelay(1000, vTask_RestartKeyboardHook);
                }
            }
            catch { }
        }

        //Check if hotkey is pressed
        public static bool CheckHotkeyPress(List<KeysVirtual> keysPressed, List<KeysVirtual> keysHotkey)
        {
            try
            {
                return keysHotkey.Any(x => x != KeysVirtual.None) && !keysHotkey.Where(x => x != KeysVirtual.None).Except(keysPressed).Any();
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckHotkeyPress(List<KeysVirtual> keysPressed, KeysVirtual[] keysHotkey)
        {
            try
            {
                return keysHotkey.Any(x => x != KeysVirtual.None) && !keysHotkey.Where(x => x != KeysVirtual.None).Except(keysPressed).Any();
            }
            catch
            {
                return false;
            }
        }

        //Check received keyboard input
        private static IntPtr LowLevelKeyboardCallbackCode(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                if (nCode >= 0)
                {
                    //Update keys pressed list
                    if (wParam == (IntPtr)WindowMessages.WM_KEYDOWN || wParam == (IntPtr)WindowMessages.WM_SYSKEYDOWN)
                    {
                        //Trigger hotkey event
                        if (EventHotkeyPressedList != null)
                        {
                            //Add key press
                            vListKeysPressed.Add((KeysVirtual)lParam.vkCode);
                            EventHotkeyPressedList(vListKeysPressed);
                        }

                        //Trigger hotkey event
                        if (EventHotkeyPressedMessage != null)
                        {
                            KeyboardMessage keyMessage = new KeyboardMessage();
                            keyMessage.windowMessage = (WindowMessages)wParam;
                            keyMessage.keyVirtual = (KeysVirtual)lParam.vkCode;
                            EventHotkeyPressedMessage(keyMessage);
                        }

                        //Debug.WriteLine("Keyboard down: " + (KeysVirtual)lParam.vkCode);
                    }
                    else if (wParam == (IntPtr)WindowMessages.WM_KEYUP || wParam == (IntPtr)WindowMessages.WM_SYSKEYUP)
                    {
                        if (EventHotkeyPressedList != null)
                        {
                            //Remove key press
                            vListKeysPressed.RemoveAll(x => x == (KeysVirtual)lParam.vkCode);
                        }

                        //Debug.WriteLine("Keyboard up: " + (KeysVirtual)lParam.vkCode);
                    }
                }

                if (BlockGlobalKeyboardPresses)
                {
                    return new IntPtr(1);
                }
                else
                {
                    return CallNextHookEx(vWindowHookPointer, nCode, wParam, lParam);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LowLevelKeyboardCallback error: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}