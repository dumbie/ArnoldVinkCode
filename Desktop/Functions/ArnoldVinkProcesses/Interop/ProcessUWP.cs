using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        //Enumerators
        private enum UWP_ACTIVATEOPTIONS
        {
            AO_NONE = 0,
            AO_DESIGNMODE = 0x1,
            AO_NOERRORUI = 0x2,
            AO_NOSPLASHSCREEN = 0x4,
            AO_PRELAUNCH = 0x2000000
        }

        //Interfaces
        [ComImport, Guid("2E941141-7F97-4756-BA1D-9DECDE894A3D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IApplicationActivationManager
        {
            IntPtr ActivateApplication([In] string AppId, [In] string Arguments, [In] UWP_ACTIVATEOPTIONS Options, out int ProcessId);
        }

        //Classes
        [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
        private class UWPActivationManager : IApplicationActivationManager
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern IntPtr ActivateApplication([In] string AppId, [In] string Arguments, [In] UWP_ACTIVATEOPTIONS Options, out int ProcessId);
        }
    }
}