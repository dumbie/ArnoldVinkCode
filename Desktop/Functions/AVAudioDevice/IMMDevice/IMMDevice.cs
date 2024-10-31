using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropCom;

namespace IMMDevice
{
    public enum STGM
    {
        STGM_READ = 0,
        STGM_WRITE = 1,
        STGM_READWRITE = 2
    }

    public enum DeviceState : uint
    {
        ACTIVE = 0x00000001,
        DISABLED = 0x00000002,
        NOTPRESENT = 0x00000004,
        UNPLUGGED = 0x00000008,
        MASK_ALL = 0x0000000F
    }

    public enum EDataFlow
    {
        eRender = 0,
        eCapture = 1,
        eAll = 2
    }

    public enum ERole
    {
        eConsole = 0,
        eMultimedia = 1,
        eCommunications = 2
    }

    [ComImport, Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDevice
    {
        void Activate([MarshalAs(UnmanagedType.LPStruct)] Guid id, uint dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.Interface)] out IAudioEndpointVolume ppInterface);
        [return: MarshalAs(UnmanagedType.Interface)]
        IPropertyStore OpenPropertyStore(STGM stgmAccess);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetId();
        DeviceState GetState();
    }

    [ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    public class MMDeviceEnumerator { }

    [ComImport, Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceEnumerator
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDeviceCollection EnumAudioEndpoints(EDataFlow dataFlow, DeviceState dwStateMask);
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDevice GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role);
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDevice GetDevice([MarshalAs(UnmanagedType.LPWStr)] string pwstrId);
    }

    [ComImport, Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceCollection
    {
        uint GetCount();
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDevice Item(uint nDevice);
    }

    [ComImport, Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEndpointVolume
    {
        int RegisterControlChangeNotify(object pNotify);
        int UnregisterControlChangeNotify(object pNotify);
        int GetChannelCount([MarshalAs(UnmanagedType.U4)] out uint channelCount);
        int SetMasterVolumeLevel([MarshalAs(UnmanagedType.R4)] float level, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        int SetMasterVolumeLevelScalar([MarshalAs(UnmanagedType.R4)] float level, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        int GetMasterVolumeLevel([MarshalAs(UnmanagedType.R4)] out float level);
        int GetMasterVolumeLevelScalar([MarshalAs(UnmanagedType.R4)] out float level);
        int SetChannelVolumeLevel([MarshalAs(UnmanagedType.U4)] uint channelNumber, [MarshalAs(UnmanagedType.R4)] float level, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        int SetChannelVolumeLevelScalar([MarshalAs(UnmanagedType.U4)] uint channelNumber, [MarshalAs(UnmanagedType.R4)] float level, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        int GetChannelVolumeLevel([MarshalAs(UnmanagedType.U4)] uint channelNumber, [MarshalAs(UnmanagedType.R4)] out float level);
        int GetChannelVolumeLevelScalar([MarshalAs(UnmanagedType.U4)] uint channelNumber, [MarshalAs(UnmanagedType.R4)] out float level);
        int SetMute([MarshalAs(UnmanagedType.Bool)] bool isMuted, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        int GetMute([MarshalAs(UnmanagedType.Bool)] out bool isMuted);
        int GetVolumeStepInfo([MarshalAs(UnmanagedType.U4)] out uint step, [MarshalAs(UnmanagedType.U4)] out uint stepCount);
        int VolumeStepUp([MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        int VolumeStepDown([MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
    }
}