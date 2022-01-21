using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputInterop
    {
        //Input events
        [DllImport("user32.dll")]
        public static extern uint SendInput(int nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public InputTypes type;
            [FieldOffset(8)]
            public MOUSEINPUT mouse;
            [FieldOffset(8)]
            public KEYBOARDINPUT keyboard;
            [FieldOffset(8)]
            public HARDWAREINPUT hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBOARDINPUT
        {
            public short wVk;
            public short wScan;
            public KeyboardEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        //Keyboard events
        [DllImport("user32.dll")]
        public static extern void keybd_event(KeysVirtual bVk, uint bScan, KeyboardEventFlags dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(KeysVirtual bVk, MapVirtualKeyMapTypes uMapType);
        public enum MapVirtualKeyMapTypes : int
        {
            MAPVK_VK_TO_VSC = 0x00,
            MAPVK_VSC_TO_VK = 0x01,
            MAPVK_VK_TO_CHAR = 0x02,
            MAPVK_VSC_TO_VK_EX = 0x03,
            MAPVK_VK_TO_VSC_EX = 0x04
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScanEx(char ch, IntPtr dwhkl);
        public enum VkKeyScanModifiers : short
        {
            SHIFT = 0x100,
            ALT = 0x200,
            CONTROL = 0x400,
            CMD = 0x800
        }

        //Mouse events
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out WindowPoint lpPoint);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(MouseEventFlags dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

        //Register Hotkey
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, KeysVirtual bVk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public static int HotKeyRegisterId = 0x9000;
    }
}