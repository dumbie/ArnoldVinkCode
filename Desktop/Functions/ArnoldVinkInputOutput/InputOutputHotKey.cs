using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputHotKey
    {
        //Variables
        private static IntPtr vWindowHookPointer = IntPtr.Zero;
        private static LowLevelKeyboardCallBack vLowLevelKeyboardCallback = LowLevelKeyboardCallbackCode;

        //Lists
        private static List<KeysVirtual> vListKeysPressed = new List<KeysVirtual>();

        //Events
        public delegate void HotKeyPressed(List<KeysVirtual> keysPressed);
        public static event HotKeyPressed EventHotKeyPressed;

        //Start receiving key input
        public static void Start()
        {
            try
            {
                vWindowHookPointer = SetWindowsHookEx(WindowHookTypes.WH_KEYBOARD_LL, vLowLevelKeyboardCallback, IntPtr.Zero, 0);
            }
            catch { }
        }

        //Stop receiving key input
        public static void Stop()
        {
            try
            {
                UnhookWindowsHookEx(vWindowHookPointer);
                vWindowHookPointer = IntPtr.Zero;
            }
            catch { }
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
                        //Add key press
                        vListKeysPressed.Add((KeysVirtual)lParam.vkCode);

                        //Trigger hotkey event
                        EventHotKeyPressed(vListKeysPressed);

                        //Debug.WriteLine("Keyboard down: " + (KeysVirtual)lParam.vkCode);
                    }
                    else if (wParam == (IntPtr)WindowMessages.WM_KEYUP || wParam == (IntPtr)WindowMessages.WM_SYSKEYUP)
                    {
                        //Remove key press
                        vListKeysPressed.RemoveAll(x => x == (KeysVirtual)lParam.vkCode);

                        //Trigger hotkey event
                        EventHotKeyPressed(vListKeysPressed);

                        //Debug.WriteLine("Keyboard up: " + (KeysVirtual)lParam.vkCode);
                    }
                }
                return CallNextHookEx(vWindowHookPointer, nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LowLevelKeyboardCallback error: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}