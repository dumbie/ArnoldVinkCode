using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputInterop;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputMouse
    {
        //Move the mouse cursor
        public static void MouseMoveCursor(int mouseHorizontal, int mouseVertical)
        {
            try
            {
                //Check the mouse movement position
                if (mouseHorizontal == 0 && mouseVertical == 0) { return; }

                //Move the mouse cursor to position
                mouse_event(MouseEventFlags.MOUSEEVENTF_MOVE, mouseHorizontal, mouseVertical, 0, IntPtr.Zero);
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

        //Simulate mouse press
        public static async Task MousePress(bool rightClick)
        {
            try
            {
                if (!rightClick)
                {
                    mouse_event(MouseEventFlags.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                    await Task.Delay(10);
                    mouse_event(MouseEventFlags.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
                }
                else
                {
                    mouse_event(MouseEventFlags.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                    await Task.Delay(10);
                    mouse_event(MouseEventFlags.MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
                }
            }
            catch { }
        }

        //Simulate mouse up or down
        public static void MouseToggle(bool rightClick, bool toggleDown)
        {
            try
            {
                if (!rightClick)
                {
                    if (toggleDown)
                    {
                        mouse_event(MouseEventFlags.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                    }
                    else
                    {
                        mouse_event(MouseEventFlags.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
                    }
                }
                else
                {
                    if (toggleDown)
                    {
                        mouse_event(MouseEventFlags.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                    }
                    else
                    {
                        mouse_event(MouseEventFlags.MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
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
                mouse_event(MouseEventFlags.MOUSEEVENTF_VWHEEL, 0, 0, scrollAmount, IntPtr.Zero);
            }
            catch { }
        }

        //Mouse scroll wheel left and right
        public static void MouseScrollWheelHorizontal(int scrollAmount)
        {
            try
            {
                mouse_event(MouseEventFlags.MOUSEEVENTF_HWHEEL, 0, 0, scrollAmount, IntPtr.Zero);
            }
            catch { }
        }
    }
}