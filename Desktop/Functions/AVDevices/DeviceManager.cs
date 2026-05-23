using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVDevices.Interop;

namespace ArnoldVinkCode.AVDevices
{
    public partial class Enumerate
    {
        public static bool DriverInstallInf(string driverPackageInfPath, DIIRFLAG flag, ref bool rebootRequired)
        {
            try
            {
                return DiInstallDriver(IntPtr.Zero, driverPackageInfPath, flag, ref rebootRequired);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to install inf driver: " + ex.Message);
                return false;
            }
        }

        public static bool DriverUninstallInf(string driverPackageInfPath, DIIRFLAG flag, ref bool rebootRequired)
        {
            try
            {
                return DiUninstallDriver(IntPtr.Zero, driverPackageInfPath, flag, ref rebootRequired);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to uninstall inf driver: " + ex.Message);
                return false;
            }
        }

        public static bool DeviceCreateNode(string className, Guid classGuid, string propertyNode)
        {
            try
            {
                SP_DEVICE_INFO_DATA deviceInfoData = new SP_DEVICE_INFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
                using AVFin deviceInfoList = new AVFin(AVFinMethod.Custom, SetupDiCreateDeviceInfoList(ref classGuid, IntPtr.Zero));
                deviceInfoList.SetReleaser(delegate (IntPtr releaseObject) { SetupDiDestroyDeviceInfoList(releaseObject); });

                if (deviceInfoList.Get() == IntPtr.Zero)
                {
                    return false;
                }

                if (!SetupDiCreateDeviceInfo(deviceInfoList.Get(), className, ref classGuid, null, IntPtr.Zero, DiCreateDevice.DICD_GENERATE_ID, ref deviceInfoData))
                {
                    return false;
                }

                if (!SetupDiSetDeviceRegistryProperty(deviceInfoList.Get(), ref deviceInfoData, DiDeviceRegistryProperty.SPDRP_HARDWAREID, propertyNode, propertyNode.Length * 2))
                {
                    return false;
                }

                if (!SetupDiCallClassInstaller(DiFunction.DIF_REGISTERDEVICE, deviceInfoList.Get(), ref deviceInfoData))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create device node: " + ex.Message);
                return false;
            }
        }

        public static bool DeviceRemove(Guid classGuid, string instanceId)
        {
            try
            {
                SP_DEVICE_INFO_DATA deviceInfoData = new SP_DEVICE_INFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                //Get device information
                using AVFin deviceInfoList = new AVFin(AVFinMethod.Custom, SetupDiGetClassDevs(classGuid, null, IntPtr.Zero, DiGetClassFlag.DIGCF_DEVICEINTERFACE));
                deviceInfoList.SetReleaser(delegate (IntPtr releaseObject) { SetupDiDestroyDeviceInfoList(releaseObject); });

                if (!SetupDiOpenDeviceInfo(deviceInfoList.Get(), instanceId, IntPtr.Zero, 0, ref deviceInfoData))
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    return false;
                }

                SP_REMOVEDEVICE_PARAMS removeParams = new SP_REMOVEDEVICE_PARAMS();
                removeParams.classInstallHeader = new SP_CLASSINSTALL_HEADER();
                removeParams.classInstallHeader.cbSize = Marshal.SizeOf(removeParams.classInstallHeader);
                removeParams.classInstallHeader.installFunction = DiFunction.DIF_REMOVE;
                removeParams.removeDevice = DiRemoveDevice.DI_REMOVEDEVICE_GLOBAL;

                if (SetupDiSetClassInstallParams(deviceInfoList.Get(), ref deviceInfoData, ref removeParams, Marshal.SizeOf(removeParams)))
                {
                    return SetupDiCallClassInstaller(DiFunction.DIF_REMOVE, deviceInfoList.Get(), ref deviceInfoData);
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove device: " + ex.Message);
                return false;
            }
        }

        public static bool ChangePropertyDevice(Guid guidClass, string deviceInstanceId, DiChangeState changeState)
        {
            try
            {
                SP_DEVICE_INFO_DATA deviceInfoData = new SP_DEVICE_INFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                //Get device information
                using AVFin deviceInfoList = new AVFin(AVFinMethod.Custom, SetupDiGetClassDevs(guidClass, deviceInstanceId, IntPtr.Zero, DiGetClassFlag.DIGCF_DEVICEINTERFACE));
                deviceInfoList.SetReleaser(delegate (IntPtr releaseObject) { SetupDiDestroyDeviceInfoList(releaseObject); });

                if (!SetupDiEnumDeviceInfo(deviceInfoList.Get(), 0, ref deviceInfoData))
                {
                    Debug.WriteLine("SetupDi: Failed getting device info.");
                    return false;
                }

                //Set property change param
                SP_PROPCHANGE_PARAMS propertyParams = new SP_PROPCHANGE_PARAMS();
                propertyParams.classInstallHeader.cbSize = Marshal.SizeOf(propertyParams.classInstallHeader);
                propertyParams.classInstallHeader.installFunction = DiFunction.DIF_PROPERTYCHANGE;
                propertyParams.changeStateFlag = DiChangeStateFlag.DICS_FLAG_GLOBAL;
                propertyParams.stateChange = changeState;

                //Prepare the device
                if (!SetupDiSetClassInstallParams(deviceInfoList.Get(), ref deviceInfoData, ref propertyParams, Marshal.SizeOf(propertyParams)))
                {
                    Debug.WriteLine("SetupDi: Failed to set install params.");
                    return false;
                }

                //Change the property
                if (!SetupDiCallClassInstaller(DiFunction.DIF_PROPERTYCHANGE, deviceInfoList.Get(), ref deviceInfoData))
                {
                    Debug.WriteLine("SetupDi: Failed to change property.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to change property: " + ex.Message);
                return false;
            }
        }
    }
}