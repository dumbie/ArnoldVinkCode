using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVWindowFunctions
    {
        //Enumerators
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

        //Get application form window
        public static T GetWindowOfType<T>() where T : Window
        {
            try
            {
                return System.Windows.Application.Current.Windows.OfType<T>().FirstOrDefault();
            }
            catch { return null; }
        }

        //Application window move
        public static void WindowMove(IntPtr windowHandle, int horLeft, int verTop)
        {
            try
            {
                SetWindowPos(windowHandle, IntPtr.Zero, horLeft, verTop, 0, 0, (int)WindowPosFlags.NOSIZE);
            }
            catch { }
        }

        //Application window resize
        public static void WindowResize(IntPtr windowHandle, int width, int height)
        {
            try
            {
                SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, width, height, (int)WindowPosFlags.NOMOVE);
            }
            catch { }
        }

        //Update window visibility
        public static void WindowUpdateVisibility(IntPtr windowHandle, bool visible)
        {
            try
            {
                if (visible)
                {
                    //Set window style
                    IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                    SetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);
                }
                else
                {
                    //Set window style
                    IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_NONE);
                    SetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                    //Redraw the window
                    SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, (int)(WindowPosFlags.NOMOVE | WindowPosFlags.NOSIZE | WindowPosFlags.NOCOPYBITS | WindowPosFlags.FRAMECHANGED));
                }
            }
            catch { }
        }

        //Update window style
        public static void WindowUpdateStyle(IntPtr windowHandle, bool topMost, bool noActivate, bool clickThrough)
        {
            try
            {
                //Set window style ex
                WindowStylesEx updatedExStyle = WindowStylesEx.WS_EX_NONE;
                if (topMost)
                {
                    //Show window as top most
                    updatedExStyle |= WindowStylesEx.WS_EX_TOPMOST;
                }
                if (noActivate)
                {
                    //Block window activation
                    updatedExStyle |= WindowStylesEx.WS_EX_NOACTIVATE;
                }
                //if (noSwitch)
                //{
                //    //Hide from alt+tab and taskbar
                //    //Note: Window needs to be hidden
                //    updatedExStyle |= WindowStylesEx.WS_EX_TOOLWINDOW;
                //}
                if (clickThrough)
                {
                    //Mouse click through window
                    updatedExStyle |= WindowStylesEx.WS_EX_TRANSPARENT;
                }

                //Set window long
                IntPtr updatedExStyleIntPtr = new IntPtr((uint)updatedExStyle);
                SetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyleIntPtr);

                //Redraw the window
                if (topMost)
                {
                    SetWindowPos(windowHandle, (IntPtr)SWP_WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowPosFlags.NOMOVE | WindowPosFlags.NOSIZE | WindowPosFlags.NOCOPYBITS | WindowPosFlags.FRAMECHANGED));
                }
                else
                {
                    SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, (int)(WindowPosFlags.NOMOVE | WindowPosFlags.NOSIZE | WindowPosFlags.NOCOPYBITS | WindowPosFlags.FRAMECHANGED));
                }
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
                    int resizeTarget = windowRectangle.Width - (windowRectangle.Right - displayMonitorSettings.BoundsRight) - boundsMargin;
                    WindowResize(windowHandle, resizeTarget, windowRectangle.Height);
                    GetWindowRect(windowHandle, out windowRectangle);
                }

                //Check if window still leaves right side and move left
                if (windowRectangle.Right > (displayMonitorSettings.BoundsRight - boundsMargin))
                {
                    int difference = (windowRectangle.Right - displayMonitorSettings.BoundsRight) + boundsMargin;
                    int moveTarget = windowRectangle.Left - difference;
                    WindowMove(windowHandle, moveTarget, windowRectangle.Top);
                    GetWindowRect(windowHandle, out windowRectangle);
                }

                //Check if window leaves bottom side
                if (windowRectangle.Bottom > (displayMonitorSettings.BoundsBottom - boundsMargin))
                {
                    int resizeTarget = windowRectangle.Height - (windowRectangle.Bottom - displayMonitorSettings.BoundsBottom) - boundsMargin;
                    WindowResize(windowHandle, windowRectangle.Width, resizeTarget);
                    GetWindowRect(windowHandle, out windowRectangle);
                }

                //Check if window still leaves bottom side and move up
                if (windowRectangle.Bottom > (displayMonitorSettings.BoundsBottom - boundsMargin))
                {
                    int difference = (windowRectangle.Bottom - displayMonitorSettings.BoundsBottom) + boundsMargin;
                    int moveTarget = windowRectangle.Top - difference;
                    WindowMove(windowHandle, windowRectangle.Left, moveTarget);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check if window is out of screen: " + ex.Message);
            }
        }
    }
}