using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode.AVDevices
{
    public partial class Interop
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public DiFunction installFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER classInstallHeader;
            public DiChangeState stateChange;
            public DiChangeStateFlag changeStateFlag;
            public int hwProfile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVPROPKEY
        {
            public Guid fmtId;
            public uint pId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_REMOVEDEVICE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER classInstallHeader;
            public DiRemoveDevice removeDevice;
            public int hwProfile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CM_POWER_DATA
        {
            public int PD_Size;
            public DEVICE_POWER_STATE PD_MostRecentPowerState;
            public PDCAP PD_Capabilities;
            public int PD_D1Latency;
            public int PD_D2Latency;
            public int PD_D3Latency;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)SYSTEM_POWER_STATE.PowerSystemMaximum)]
            public DEVICE_POWER_STATE[] PD_PowerStateMapping;
            public SYSTEM_POWER_STATE PD_DeepestSystemWake;
        }
    }
}