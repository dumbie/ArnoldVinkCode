using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVCustomWindow
    {
        //Window variables
        public int windowHeight;
        private int windowWidth;
        private IntPtr windowChild;
        private IntPtr windowParent;
        private static WindowProc windowProc;
        private IntPtr windowProcPointer;

        //Window create
        public AVCustomWindow(string windowTitle, int windowWidth, int windowHeight, bool windowVisible)
        {
            try
            {
                //Set window variables
                this.windowWidth = windowWidth;
                this.windowHeight = windowHeight;

                //Create window procedure
                windowProc = WindowProcessMessage;
                windowProcPointer = Marshal.GetFunctionPointerForDelegate(windowProc);

                //Create window class
                WNDCLASSEX windowClassEx = new WNDCLASSEX
                {
                    cbSize = WNDCLASSEX.classSize,
                    style = 0,
                    lpfnWndProc = windowProcPointer,
                    cbClsExtra = 0,
                    cbWndExtra = 0,
                    hInstance = IntPtr.Zero,
                    hIcon = IntPtr.Zero,
                    hCursor = IntPtr.Zero,
                    hbrBackground = IntPtr.Zero,
                    lpszMenuName = windowTitle,
                    lpszClassName = Environment.TickCount.ToString(),
                    hIconSm = IntPtr.Zero
                };

                //Register window class
                RegisterClassEx(ref windowClassEx);

                //Create window
                WindowStyles windowStyles = WindowStyles.WS_OVERLAPPEDWINDOW;
                WindowStylesEx windowStylesEx = WindowStylesEx.WS_EX_LEFT;
                windowParent = CreateWindowEx(windowStylesEx, windowClassEx.lpszClassName, windowClassEx.lpszMenuName, windowStyles, 0, 0, this.windowHeight, this.windowWidth, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                //Show window
                if (windowVisible)
                {
                    ShowWindow(windowParent, WindowShowCommand.Show);
                }

                Debug.WriteLine("Created custom window: " + windowParent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create custom window: " + ex.Message);
            }
        }

        //Window process message
        internal delegate IntPtr WindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        private IntPtr WindowProcessMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                return DefWindowProc(hWnd, uMsg, wParam, lParam);
            }
            catch { }
            return IntPtr.Zero;
        }

        //Window host
        public void HostWindow(IntPtr targetWindow)
        {
            try
            {
                //Set host child variable
                windowChild = targetWindow;
                Debug.WriteLine("Hosting window in parent: " + windowChild);

                //Maximize child window
                ShowWindow(windowChild, WindowShowCommand.Maximized);

                //Update child style
                WindowStyles windowStyle = WindowStyles.WS_NONE;
                IntPtr updatedStyle = new IntPtr((uint)windowStyle);
                SetWindowLongAuto(windowChild, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                //Set the parent window
                SetParent(windowChild, windowParent);
            }
            catch { }
        }

        //Window show
        public void Show()
        {
            try
            {
                ////Get current window styles
                //WindowStyles windowStyle = (WindowStyles)GetWindowLongAuto(windowParent, (int)WindowLongFlags.GWL_STYLE).ToInt64();
                //windowStyle |= WindowStyles.WS_VISIBLE;

                ////Update window style
                //IntPtr updatedStyle = new IntPtr((uint)windowStyle);
                //SetWindowLongAuto(windowParent, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                ShowWindow(windowParent, WindowShowCommand.Show);
                Debug.WriteLine("Showed custom window: " + windowParent);
            }
            catch { }
        }

        //Window hide
        public void Hide()
        {
            try
            {
                ////Get current window styles
                //WindowStyles windowStyle = (WindowStyles)GetWindowLongAuto(windowParent, (int)WindowLongFlags.GWL_STYLE).ToInt64();
                //windowStyle &= ~WindowStyles.WS_VISIBLE;

                ////Update window style
                //IntPtr updatedStyle = new IntPtr((uint)windowStyle);
                //SetWindowLongAuto(windowParent, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                ShowWindow(windowParent, WindowShowCommand.Hide);
                Debug.WriteLine("Hidden custom window: " + windowParent);
            }
            catch { }
        }

        //Window close
        public void Close()
        {
            try
            {
                DestroyWindow(windowParent);
                Debug.WriteLine("Closed custom window: " + windowParent);
            }
            catch { }
        }
    }
}