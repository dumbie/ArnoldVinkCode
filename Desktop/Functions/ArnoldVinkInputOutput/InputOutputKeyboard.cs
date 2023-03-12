﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputInterop;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputKeyboard
    {
        //Send single key press
        public static void KeySendSingle(KeysVirtual virtualKey, IntPtr windowHandle)
        {
            try
            {
                PostMessage(windowHandle, (int)WindowMessages.WM_KEYDOWN, (byte)virtualKey, 0); //Key Press
                AVActions.TaskDelayHighRes(50);
                PostMessage(windowHandle, (int)WindowMessages.WM_KEYUP, (byte)virtualKey, 0); //Key Release
            }
            catch { }
        }

        //Keyboard type string SendInput (UTF-32)
        public static void KeyTypeStringSend(string typeString)
        {
            try
            {
                INPUT[] input = new INPUT[typeString.Length];
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = new INPUT();
                    input[i].type = InputTypes.INPUT_KEYBOARD;
                    input[i].keyboard.wVk = 0;
                    input[i].keyboard.wScan = (short)typeString[i];
                    input[i].keyboard.time = 0;
                    input[i].keyboard.dwFlags = KeyboardEventFlags.KEYEVENTF_UNICODE;
                    input[i].keyboard.dwExtraInfo = IntPtr.Zero;
                }
                SendInput(input.Length, input, Marshal.SizeOf(typeof(INPUT)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send type string: " + ex.Message);
            }
        }

        //Keyboard type string Event
        public static void KeyTypeStringEvent(string typeString)
        {
            try
            {
                foreach (char charString in typeString)
                {
                    short scanVirtualKey = VkKeyScanEx(charString, IntPtr.Zero);
                    KeysVirtual usedVirtualKey = (KeysVirtual)scanVirtualKey;
                    bool shiftPressed = (scanVirtualKey & (short)VkKeyScanModifiers.SHIFT) > 0;
                    if (shiftPressed)
                    {
                        KeyPressReleaseCombo(KeysVirtual.Shift, usedVirtualKey);
                    }
                    else
                    {
                        KeyPressReleaseSingle(usedVirtualKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to event type string: " + ex.Message);
            }
        }

        //Simulate single key press and release
        public static bool KeyPressReleaseSingle(KeysVirtual virtualKey)
        {
            try
            {
                uint scanByte = 0;
                KeyboardEventFlags KeyFlagsDown = 0;
                KeyboardEventFlags KeyFlagsUp = 0;
                if (KeysVirtualExtended.Contains(virtualKey))
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                keybd_event(virtualKey, scanByte, KeyFlagsDown, 0); //Key Press
                AVActions.TaskDelayHighRes(50);
                keybd_event(virtualKey, scanByte, KeyFlagsUp, 0); //Key Release
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press and release single key.");
                return false;
            }
        }

        //Simulate single key press or release
        public static bool KeyToggleSingle(KeysVirtual virtualKey, bool pressKey)
        {
            try
            {
                uint scanByte = 0;
                KeyboardEventFlags KeyFlagsDown = 0;
                KeyboardEventFlags KeyFlagsUp = 0;
                if (KeysVirtualExtended.Contains(virtualKey))
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByte = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDown = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUp = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                if (pressKey)
                {
                    keybd_event(virtualKey, scanByte, KeyFlagsDown, 0); //Key Press
                }
                else
                {
                    keybd_event(virtualKey, scanByte, KeyFlagsUp, 0); //Key Release
                }
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to toggle single key.");
                return false;
            }
        }

        //Simulate combo key press and release
        public static bool KeyPressReleaseCombo(KeysVirtual modifierKey, KeysVirtual virtualKey)
        {
            try
            {
                uint scanByteMod = 0;
                KeyboardEventFlags KeyFlagsDownMod = 0;
                KeyboardEventFlags KeyFlagsUpMod = 0;
                if (KeysVirtualExtended.Contains(modifierKey))
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                uint scanByteVk = 0;
                KeyboardEventFlags KeyFlagsDownVk = 0;
                KeyboardEventFlags KeyFlagsUpVk = 0;
                if (KeysVirtualExtended.Contains(virtualKey))
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                keybd_event(modifierKey, scanByteMod, KeyFlagsDownMod, 0); //Modifier Press
                keybd_event(virtualKey, scanByteVk, KeyFlagsDownVk, 0); //Key Press
                AVActions.TaskDelayHighRes(50);
                keybd_event(virtualKey, scanByteVk, KeyFlagsUpVk, 0); //Key Release
                keybd_event(modifierKey, scanByteMod, KeyFlagsUpMod, 0); //Modifier Release
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to press and release combo keys.");
                return false;
            }
        }

        //Simulate combo key press or release
        public static bool KeyToggleCombo(KeysVirtual modifierKey, KeysVirtual virtualKey, bool pressKey)
        {
            try
            {
                uint scanByteMod = 0;
                KeyboardEventFlags KeyFlagsDownMod = 0;
                KeyboardEventFlags KeyFlagsUpMod = 0;
                if (KeysVirtualExtended.Contains(modifierKey))
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteMod = MapVirtualKey(modifierKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownMod = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpMod = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                uint scanByteVk = 0;
                KeyboardEventFlags KeyFlagsDownVk = 0;
                KeyboardEventFlags KeyFlagsUpVk = 0;
                if (KeysVirtualExtended.Contains(virtualKey))
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC_EX);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_EXTENDEDKEY | KeyboardEventFlags.KEYEVENTF_KEYUP;
                }
                else
                {
                    scanByteVk = MapVirtualKey(virtualKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
                    KeyFlagsDownVk = KeyboardEventFlags.KEYEVENTF_NONE;
                    KeyFlagsUpVk = KeyboardEventFlags.KEYEVENTF_KEYUP;
                }

                if (pressKey)
                {
                    keybd_event(modifierKey, scanByteMod, KeyFlagsDownMod, 0); //Modifier Press
                    keybd_event(virtualKey, scanByteVk, KeyFlagsDownVk, 0); //Key Press
                }
                else
                {
                    keybd_event(virtualKey, scanByteVk, KeyFlagsUpVk, 0); //Key Release
                    keybd_event(modifierKey, scanByteMod, KeyFlagsUpMod, 0); //Modifier Release
                }
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to toggle combo keys.");
                return false;
            }
        }
    }
}