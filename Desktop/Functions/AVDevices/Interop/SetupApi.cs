using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ArnoldVinkCode.AVDevices
{
    [SuppressUnmanagedCodeSecurity]
    public partial class Interop
    {
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, ref SP_PROPCHANGE_PARAMS classInstallParams, int classInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiCallClassInstaller(DiFunction installFunction, IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(Guid classGuid, string enumerator, IntPtr hWndParent, DiGetClassFlag diFlags);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryProperty")]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, DiDeviceRegistryProperty propertyVal, ref int propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryProperty")]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, DiDeviceRegistryProperty propertyVal, ref int propertyRegDataType, ref CM_POWER_DATA propertyBuffer, int propertyBufferSize, ref int requiredSize);

        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDevicePropertyW")]
        public static extern bool SetupDiGetDeviceProperty(IntPtr deviceInfo, ref SP_DEVICE_INFO_DATA deviceInfoData, ref DEVPROPKEY propKey, ref int propertyDataType, byte[] propertyBuffer, int propertyBufferSize, ref int requiredSize, uint flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoList, int memberIndex, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoList);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoList, IntPtr deviceInfoData, Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoList, SP_DEVICE_INFO_DATA deviceInfoData, Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoList, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoList, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, byte[] deviceInstanceId, int deviceInstanceIdSize, ref int requiredSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiOpenDeviceInfo(IntPtr deviceInfoList, string DeviceInstanceId, IntPtr hWndParent, DiOpenDevice flags, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, ref SP_REMOVEDEVICE_PARAMS ClassInstallParams, int ClassInstallParamsSize);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hWndParent);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiCreateDeviceInfo(IntPtr deviceInfoList, string DeviceName, ref Guid ClassGuid, string DeviceDescription, IntPtr hWndParent, DiCreateDevice flags, ref SP_DEVICE_INFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern bool SetupDiSetDeviceRegistryProperty(IntPtr deviceInfoList, ref SP_DEVICE_INFO_DATA deviceInfoData, DiDeviceRegistryProperty Property, [MarshalAs(UnmanagedType.LPWStr)] string PropertyBuffer, int PropertyBufferSize);

        [DllImport("newdev.dll", CharSet = CharSet.Auto)]
        public static extern bool DiInstallDriver(IntPtr hWndParent, string DriverPackageInfPath, DIIRFLAG Flags, ref bool RebootRequired);

        [DllImport("newdev.dll", CharSet = CharSet.Auto)]
        public static extern bool DiUninstallDriver(IntPtr hWndParent, string DriverPackageInfPath, DIIRFLAG Flags, ref bool RebootRequired);
    }
}