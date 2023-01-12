using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVWindowFunctions
    {
        public enum AVWindowPosition : int
        {
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,
            MiddleLeft = 3,
            MiddleCenter = 4,
            MiddleRight = 5,
            BottomLeft = 6,
            BottomCenter = 7,
            BottomRight = 8,
            FullScreen = 9
        }

        //Update window style to visible
        public static void WindowUpdateStyleVisible(IntPtr windowHandle, bool topMost, bool noActivate, bool clickThrough)
        {
            try
            {
                //Set window style
                IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                //Set window style ex
                WindowStylesEx updatedExStyle = WindowStylesEx.WS_EX_NONE;
                if (topMost)
                {
                    updatedExStyle |= WindowStylesEx.WS_EX_TOPMOST;
                }
                if (noActivate)
                {
                    updatedExStyle |= WindowStylesEx.WS_EX_NOACTIVATE;
                }
                if (clickThrough)
                {
                    updatedExStyle |= WindowStylesEx.WS_EX_TRANSPARENT;
                }
                IntPtr updatedExStyleIntPtr = new IntPtr((uint)updatedExStyle);
                SetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyleIntPtr);

                //Redraw the window
                if (topMost)
                {
                    SetWindowPos(windowHandle, (IntPtr)SWP_WindowPosition.TopMost, 0, 0, 0, 0, (int)(SWP_WindowFlags.NOMOVE | SWP_WindowFlags.NOSIZE | SWP_WindowFlags.DRAWFRAME));
                }
                else
                {
                    SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, (int)(SWP_WindowFlags.NOMOVE | SWP_WindowFlags.NOSIZE | SWP_WindowFlags.DRAWFRAME));
                }
            }
            catch { }
        }

        //Update window style to hidden
        public static void WindowUpdateStyleHidden(IntPtr windowHandle)
        {
            try
            {
                //Set window style
                IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_NONE);
                SetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                //Set window style ex
                IntPtr updatedExStyleIntPtr = new IntPtr((uint)WindowStylesEx.WS_EX_NONE);
                SetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyleIntPtr);

                //Redraw the window
                SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, (int)(SWP_WindowFlags.NOMOVE | SWP_WindowFlags.NOSIZE | SWP_WindowFlags.DRAWFRAME));
            }
            catch { }
        }

        //Update window position
        public static void WindowUpdatePosition(int? monitorNumber, IntPtr windowHandle, AVWindowPosition windowPosition)
        {
            try
            {
                //Check the set monitor number
                if (monitorNumber == null || monitorNumber <= 0)
                {
                    monitorNumber = Convert.ToInt32(Regex.Replace(Screen.FromHandle(windowHandle).DeviceName, "[^0-9]", string.Empty));
                    Debug.WriteLine("Window monitor number: " + monitorNumber);
                }

                //Get monitor information
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay((int)monitorNumber);

                //Get the current window size
                GetWindowRect(windowHandle, out WindowRectangle windowRectangle);

                //Move or resize the window
                if (windowPosition == AVWindowPosition.TopLeft)
                {
                    int horizontalLeft = displayMonitorSettings.BoundsLeft;
                    int verticalTop = displayMonitorSettings.BoundsTop;
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.TopCenter)
                {
                    int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowRectangle.Width) / 2);
                    int verticalTop = displayMonitorSettings.BoundsTop;
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.TopRight)
                {
                    int horizontalLeft = displayMonitorSettings.BoundsRight - windowRectangle.Width;
                    int verticalTop = displayMonitorSettings.BoundsTop;
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.BottomLeft)
                {
                    int horizontalLeft = displayMonitorSettings.BoundsLeft;
                    int verticalTop = displayMonitorSettings.BoundsTop + displayMonitorSettings.HeightNative - windowRectangle.Height;
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.BottomCenter)
                {
                    int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowRectangle.Width) / 2);
                    int verticalTop = displayMonitorSettings.BoundsTop + displayMonitorSettings.HeightNative - windowRectangle.Height;
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.BottomRight)
                {
                    int horizontalLeft = displayMonitorSettings.BoundsRight - windowRectangle.Width;
                    int verticalTop = displayMonitorSettings.BoundsTop + displayMonitorSettings.HeightNative - windowRectangle.Height;
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.MiddleLeft)
                {
                    int horizontalLeft = displayMonitorSettings.BoundsLeft;
                    int verticalTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowRectangle.Height) / 2);
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.MiddleCenter)
                {
                    int horizontalLeft = (int)(displayMonitorSettings.BoundsLeft + (displayMonitorSettings.WidthNative - windowRectangle.Width) / 2);
                    int verticalTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowRectangle.Height) / 2);
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.MiddleRight)
                {
                    int horizontalLeft = displayMonitorSettings.BoundsRight - windowRectangle.Width;
                    int verticalTop = (int)(displayMonitorSettings.BoundsTop + (displayMonitorSettings.HeightNative - windowRectangle.Height) / 2);
                    WindowMove(windowHandle, horizontalLeft, verticalTop);
                }
                else if (windowPosition == AVWindowPosition.FullScreen)
                {
                    WindowMove(windowHandle, displayMonitorSettings.BoundsLeft, displayMonitorSettings.BoundsTop);
                    WindowResize(windowHandle, displayMonitorSettings.WidthNative, displayMonitorSettings.HeightNative);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update window position: " + ex.Message);
            }
        }

        //Check if window is out of screen
        public static void WindowCheckScreenBounds(int? monitorNumber, IntPtr windowHandle, int boundsMargin)
        {
            try
            {
                //Check the set bounds margin
                if (boundsMargin < 0)
                {
                    boundsMargin = 0;
                }

                //Check the set monitor number
                if (monitorNumber == null || monitorNumber <= 0)
                {
                    monitorNumber = Convert.ToInt32(Regex.Replace(Screen.FromHandle(windowHandle).DeviceName, "[^0-9]", string.Empty));
                    Debug.WriteLine("Window monitor number: " + monitorNumber);
                }

                //Get monitor information
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay((int)monitorNumber);

                //Get the current window size
                GetWindowRect(windowHandle, out WindowRectangle windowRectangle);

                //Check if window leaves top side
                if (windowRectangle.Top < (displayMonitorSettings.BoundsTop + boundsMargin))
                {
                    int moveTarget = displayMonitorSettings.BoundsTop + boundsMargin;
                    WindowMove(windowHandle, windowRectangle.Left, moveTarget);
                    GetWindowRect(windowHandle, out windowRectangle);
                }

                //Check if window leaves left side
                if (windowRectangle.Left < (displayMonitorSettings.BoundsLeft + boundsMargin))
                {
                    int moveTarget = displayMonitorSettings.BoundsLeft + boundsMargin;
                    WindowMove(windowHandle, moveTarget, windowRectangle.Top);
                    GetWindowRect(windowHandle, out windowRectangle);
                }

                //Check if window leaves right side
                if (windowRectangle.Right > (displayMonitorSettings.BoundsRight - boundsMargin))
                {
                    //This does not work properly when minimum window size is set
                    int resizeTarget = windowRectangle.Width - (windowRectangle.Right - displayMonitorSettings.BoundsRight) - boundsMargin;
                    WindowResize(windowHandle, resizeTarget, windowRectangle.Height);
                    GetWindowRect(windowHandle, out windowRectangle);
                }

                //Check if window leaves bottom side
                if (windowRectangle.Bottom > (displayMonitorSettings.BoundsBottom - boundsMargin))
                {
                    //This does not work properly when minimum window size is set
                    int resizeTarget = windowRectangle.Height - (windowRectangle.Bottom - displayMonitorSettings.BoundsBottom) - boundsMargin;
                    WindowResize(windowHandle, windowRectangle.Width, resizeTarget);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check if window is out of screen: " + ex.Message);
            }
        }
    }
}