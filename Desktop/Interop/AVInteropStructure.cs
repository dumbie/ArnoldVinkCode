using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ArnoldVinkCode
{
    [SuppressUnmanagedCodeSecurity]
    public partial class AVInteropDll
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WindowClassEx
        {
            public uint cbSize;
            public ClassStyles style;
            public WindowProcedureDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public WindowBackgroundBrushes hbrBackground;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowMessage
        {
            public IntPtr hWnd;
            public IntPtr message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int pt_x;
            public int pt_y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowMinMaxInfo
        {
            public WindowPoint ptReserved;
            public WindowPoint ptMaxSize;
            public WindowPoint ptMaxPosition;
            public WindowPoint ptMinTrackSize;
            public WindowPoint ptMaxTrackSize;
        }

        public struct WindowPoint
        {
            public int X;
            public int Y;
        }

        public struct WindowSize
        {
            public int cx;
            public int cy;
        }

        public struct WindowRectangle
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int Width { get { return this.Right - this.Left; } }
            public int Height { get { return this.Bottom - this.Top; } }
        }

        public struct WindowPlacement
        {
            public int length;
            public WindowPlacementFlags windowFlags;
            public ShowWindowFlags windowShowCommand;
            public WindowPoint ptMinPosition;
            public WindowPoint ptMaxPosition;
            public WindowRectangle rcNormalPosition;
            public WindowRectangle rcDevice;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public KBDLLHOOKSTRUCTFLAGS flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHANGEFILTERSTRUCT
        {
            public uint cbSize;
            public MessageFilterStatus ExtStatus;
        }
    }
}