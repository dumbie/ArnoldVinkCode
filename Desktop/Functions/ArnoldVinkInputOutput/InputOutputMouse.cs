using System;
using System.Threading;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputMouse
    {
        //Move the mouse cursor
        public static void MouseMoveCursor(int mouseHorizontal, int mouseVertical)
        {
            try
            {
                if (mouseHorizontal == 0 || mouseHorizontal == 0) { return; }
                mouse_event((uint)MouseEvents.MOUSEEVENTF_MOVE, mouseHorizontal, mouseVertical, 0, IntPtr.Zero);
            }
            catch { }
        }

        //Scroll the mouse wheel
        public static void MouseScrollWheel(int scrollHorizontal, int scrollVertical)
        {
            try
            {
                if (scrollVertical != 0)
                {
                    MouseScrollWheelVertical(scrollVertical);
                }
                if (scrollHorizontal != 0)
                {
                    MouseScrollWheelHorizontal(scrollHorizontal);
                }
            }
            catch { }
        }

        //Simulate single key press
        public static void MousePressSingle(bool rightClick)
        {
            try
            {
                if (!rightClick)
                {
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                    Thread.Sleep(10);
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
                }
                else
                {
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                    Thread.Sleep(10);
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
                }
            }
            catch { }
        }

        //Simulate mouse up or down
        public static void MouseToggle(bool rightClick, bool downClick)
        {
            try
            {
                if (!rightClick)
                {
                    if (downClick)
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                    }
                    else
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
                    }
                }
                else
                {
                    if (downClick)
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                    }
                    else
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
                    }
                }
            }
            catch { }
        }

        //Mouse scroll wheel up or down
        public static void MouseScrollWheelVertical(int scrollAmount)
        {
            try
            {
                mouse_event((uint)MouseEvents.MOUSEEVENTF_VWHEEL, 0, 0, scrollAmount, IntPtr.Zero);
            }
            catch { }
        }

        //Mouse scroll wheel left and right
        public static void MouseScrollWheelHorizontal(int scrollAmount)
        {
            try
            {
                mouse_event((uint)MouseEvents.MOUSEEVENTF_HWHEEL, 0, 0, scrollAmount, IntPtr.Zero);
            }
            catch { }
        }
    }
}