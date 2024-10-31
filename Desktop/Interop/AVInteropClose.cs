using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVInteropDll
    {
        public static bool SafeCloseMarshal(ref IntPtr hGlobal)
        {
            try
            {
                if (hGlobal != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hGlobal);
                    //Debug.WriteLine("Closed marshal: " + hGlobal);
                    hGlobal = IntPtr.Zero;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close marshal: " + ex.Message);
                return false;
            }
        }

        public static bool SafeCloseHandle(ref IntPtr hHandle)
        {
            try
            {
                if (hHandle != IntPtr.Zero)
                {
                    bool result = CloseHandle(hHandle);
                    //Debug.WriteLine("Closed handle: " + hHandle + "/" + result);
                    hHandle = IntPtr.Zero;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close handle: " + ex.Message);
                return false;
            }
        }

        public static bool SafeCloseObject(ref IntPtr hObject)
        {
            try
            {
                if (hObject != IntPtr.Zero)
                {
                    bool result = DeleteObject(hObject);
                    //Debug.WriteLine("Closed object: " + hObject + "/" + result);
                    hObject = IntPtr.Zero;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close object: " + ex.Message);
                return false;
            }
        }

        public static bool SafeCloseIcon(ref IntPtr hIcon)
        {
            try
            {
                if (hIcon != IntPtr.Zero)
                {
                    bool result = DestroyIcon(hIcon);
                    //Debug.WriteLine("Closed icon: " + hIcon + "/" + result);
                    hIcon = IntPtr.Zero;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close icon: " + ex.Message);
                return false;
            }
        }

        public static bool SafeCloseLibrary(ref IntPtr hModule)
        {
            try
            {
                if (hModule != IntPtr.Zero)
                {
                    bool result = FreeLibrary(hModule);
                    //Debug.WriteLine("Closed library: " + hModule + "/" + result);
                    hModule = IntPtr.Zero;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to close library: " + ex.Message);
                return false;
            }
        }
    }
}