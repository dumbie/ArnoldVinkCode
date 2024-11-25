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
    public partial class AVInputOutputHotkeyHook
    {
        //Variables
        private static IntPtr vWindowHookPointer = IntPtr.Zero;

        //Settings
        public static bool BlockGlobalKeyboardPresses = false;

        //Lists
        private static bool[] vListKeysPressed = new bool[255];

        //Events
        public static Action<KeyboardMessage> EventHotkeyPressedMessage;
        public static Action<bool[]> EventHotkeyPressedList;

        //Tasks
        private static AVTaskDetails vTask_RestartKeyboardHook = new AVTaskDetails("vTask_RestartKeyboardHook");

        //Start receiving key input
        public static bool Start()
        {
            bool hooked = false;
            try
            {
                //Reset pressed keys
                vListKeysPressed = new bool[255];

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
                //Reset pressed keys
                vListKeysPressed = new bool[255];

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
                //Reset pressed keys
                vListKeysPressed = new bool[255];

                //Remove window keyboard hook
                WindowKeyboardUnhook();

                //Set window keyboard hook
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
                    vWindowHookPointer = SetWindowsHookEx(WindowHookTypes.WH_KEYBOARD_LL, LowLevelKeyboardDelegate, IntPtr.Zero, 0);
                    Debug.WriteLine("Hooked window keyboard: " + vWindowHookPointer);
                });
            }
            catch { }
            return vWindowHookPointer != IntPtr.Zero;
        }

        //Remove window keyboard hook
        private static bool WindowKeyboardUnhook()
        {
            bool unhooked = false;
            try
            {
                if (vWindowHookPointer != IntPtr.Zero)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        unhooked = UnhookWindowsHookEx(vWindowHookPointer);
                        //Debug.WriteLine("Unhooked window keyboard: " + unhooked);
                    });
                    vWindowHookPointer = IntPtr.Zero;
                }
            }
            catch { }
            return unhooked;
        }

        //Task detecting window change
        private static async Task vTaskLoop_RestartKeyboardHook()
        {
            try
            {
                //Loop variables
                IntPtr previousForegroundWindow = GetForegroundWindow();

                while (await TaskCheckLoop(vTask_RestartKeyboardHook, 1000))
                {
                    try
                    {
                        //Get foreground window
                        IntPtr currentForegroundWindow = GetForegroundWindow();

                        //Check foreground window change
                        if (previousForegroundWindow != currentForegroundWindow)
                        {
                            //Restart window keyboard hook
                            bool restarted = Restart();

                            //Update previous window
                            if (restarted)
                            {
                                previousForegroundWindow = currentForegroundWindow;
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Check if hotkey is pressed
        public static bool CheckHotkeyPressed(bool[] keysPressed, List<KeysVirtual> keysHotkey)
        {
            try
            {
                return keysHotkey.Where(x => x != KeysVirtual.None).All(x => keysPressed[(int)x]);
            }
            catch { }
            return false;
        }

        public static bool CheckHotkeyPressed(bool[] keysPressed, KeysVirtual[] keysHotkey)
        {
            try
            {
                return keysHotkey.Where(x => x != KeysVirtual.None).All(x => keysPressed[(int)x]);
            }
            catch { }
            return false;
        }

        //Check received keyboard input
        private static IntPtr LowLevelKeyboardDelegate(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                if (nCode >= 0)
                {
                    //Update keys pressed list
                    if (wParam == (IntPtr)WindowMessages.WM_KEYDOWN || wParam == (IntPtr)WindowMessages.WM_SYSKEYDOWN)
                    {
                        if (EventHotkeyPressedList != null)
                        {
                            //Update key press
                            vListKeysPressed[lParam.vkCode] = true;

                            //Trigger hotkey event
                            EventHotkeyPressedList(vListKeysPressed);
                        }

                        if (EventHotkeyPressedMessage != null)
                        {
                            //Set keyboard message
                            KeyboardMessage keyMessage = new KeyboardMessage();
                            keyMessage.windowMessage = (WindowMessages)wParam;
                            keyMessage.keyVirtual = (KeysVirtual)lParam.vkCode;

                            //Trigger hotkey event
                            EventHotkeyPressedMessage(keyMessage);
                        }

                        //Debug.WriteLine("Keyboard down: " + (KeysVirtual)lParam.vkCode);
                    }
                    else if (wParam == (IntPtr)WindowMessages.WM_KEYUP || wParam == (IntPtr)WindowMessages.WM_SYSKEYUP)
                    {
                        if (EventHotkeyPressedList != null)
                        {
                            //Update key press
                            vListKeysPressed[lParam.vkCode] = false;
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
                Debug.WriteLine("LowLevelKeyboardDelegate error: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}