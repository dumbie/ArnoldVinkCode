using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public enum AVFinMethod
    {
        CloseHandle,
        FreeLibrary,
        DestroyIcon,
        DeleteObject,
        ReleaseDC,
        ComFree,
        FreeSid,
        FreeMarshal,
        Custom
    }

    ///Description: Automatically releases object after going out of scope or loop.
    ///Note: Error C0000374 usually means you are using the wrong release method.
    ///Usage example: using AVFin avFin = new AVFin(AVFinMethod.Custom, releaseObject);
    ///Usage example: avFin.SetReleaser(delegate(IntPtr releaseObject)
    ///{
    ///	for (int i = 0; i < releaseItemCount; i++)
    ///	{
    ///		Marshal.FreeHGlobal(releaseObject[i].WCHAR);
    ///	}
    ///	Marshal.FreeHGlobal(releaseObject);
    ///});
    public partial class AVFin : IDisposable
    {
        private IntPtr ReleaseObject = IntPtr.Zero;
        private Action<IntPtr> ReleaseFunction = null;
        private AVFinMethod ReleaseMethod = AVFinMethod.Custom;

        public AVFin(AVFinMethod setMethod)
        {
            ReleaseMethod = setMethod;
        }

        public AVFin(AVFinMethod setMethod, IntPtr setObject)
        {
            ReleaseMethod = setMethod;
            ReleaseObject = setObject;
        }

        public AVFin(AVFinMethod setMethod, ref IntPtr setObject)
        {
            ReleaseMethod = setMethod;
            ReleaseObject = setObject;
        }

        public void SetReleaser(Action<IntPtr> setFunction)
        {
            ReleaseFunction = setFunction;
        }

        public void Set(IntPtr setObject)
        {
            if (ReleaseObject == IntPtr.Zero)
            {
                ReleaseObject = setObject;
            }
            else
            {
                AVDebug.WriteLine("AVFin object is already set.");
            }
        }

        public void Set(ref IntPtr setObject)
        {
            if (ReleaseObject == IntPtr.Zero)
            {
                ReleaseObject = setObject;
            }
            else
            {
                AVDebug.WriteLine("AVFin object is already set.");
            }
        }

        public ref IntPtr Get()
        {
            return ref ReleaseObject;
        }

        ~AVFin() { Dispose(); }
        public void Dispose()
        {
            try
            {
                if (ReleaseObject != IntPtr.Zero)
                {
                    if (ReleaseMethod == AVFinMethod.CloseHandle)
                    {
                        CloseHandle(ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.FreeLibrary)
                    {
                        FreeLibrary(ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.DestroyIcon)
                    {
                        DestroyIcon(ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.DeleteObject)
                    {
                        DeleteObject(ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.ReleaseDC)
                    {
                        ReleaseDC(IntPtr.Zero, ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.FreeSid)
                    {
                        CoTaskMemFree(ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.ComFree)
                    {
                        FreeSid(ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.FreeMarshal)
                    {
                        Marshal.FreeHGlobal(ReleaseObject);
                    }
                    else if (ReleaseMethod == AVFinMethod.Custom)
                    {
                        if (ReleaseFunction != null)
                        {
                            ReleaseFunction(ReleaseObject);
                        }
                    }
                    ReleaseObject = IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to dispose AVFin object: " + ex.Message);
            }
        }
    }
}