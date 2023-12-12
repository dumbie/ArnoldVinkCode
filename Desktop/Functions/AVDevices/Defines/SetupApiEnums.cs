namespace ArnoldVinkCode.AVDevices
{
    public partial class Interop
    {
        public enum DiRemoveDevice : int
        {
            DI_REMOVEDEVICE_GLOBAL = 0x00000001,
            DI_REMOVEDEVICE_CONFIGSPECIFIC = 0x00000002
        }

        public enum DiFunction : int
        {
            DIF_SELECTDEVICE = 0x0001,
            DIF_INSTALLDEVICE = 0x0002,
            DIF_ASSIGNRESOURCES = 0x0003,
            DIF_PROPERTIES = 0x0004,
            DIF_REMOVE = 0x0005,
            DIF_FIRSTTIMESETUP = 0x0006,
            DIF_FOUNDDEVICE = 0x0007,
            DIF_SELECTCLASSDRIVERS = 0x0008,
            DIF_VALIDATECLASSDRIVERS = 0x0009,
            DIF_INSTALLCLASSDRIVERS = 0x000A,
            DIF_CALCDISKSPACE = 0x000B,
            DIF_DESTROYPRIVATEDATA = 0x000C,
            DIF_VALIDATEDRIVER = 0x000D,
            DIF_MOVEDEVICE = 0x000E,
            DIF_DETECT = 0x000F,
            DIF_INSTALLWIZARD = 0x0010,
            DIF_DESTROYWIZARDDATA = 0x0011,
            DIF_PROPERTYCHANGE = 0x0012,
            DIF_ENABLECLASS = 0x0013,
            DIF_DETECTVERIFY = 0x0014,
            DIF_INSTALLDEVICEFILES = 0x0015,
            DIF_UNREMOVE = 0x0016,
            DIF_SELECTBESTCOMPATDRV = 0x0017,
            DIF_ALLOW_INSTALL = 0x0018,
            DIF_REGISTERDEVICE = 0x0019
        }

        public enum DiChangeState : int
        {
            DICS_ENABLE = 0x00000001,
            DICS_DISABLE = 0x00000002,
            DICS_PROPCHANGE = 0x00000003,
            DICS_START = 0x00000004,
            DICS_STOP = 0x00000005
        }

        public enum DiChangeStateFlag : int
        {
            DICS_FLAG_GLOBAL = 0x00000001,
            DICS_FLAG_CONFIGSPECIFIC = 0x00000002,
            DICS_FLAG_CONFIGGENERAL = 0x00000004
        }

        public enum DiGetClassFlag : int
        {
            DIGCF_DEFAULT = 0x0001,
            DIGCF_PRESENT = 0x0002,
            DIGCF_ALLCLASSES = 0x0004,
            DIGCF_PROFILE = 0x0008,
            DIGCF_DEVICEINTERFACE = 0x0010
        }

        public enum DiOpenDevice : int
        {
            DIOD_NONE = 0x00000000,
            DIOD_INHERIT_CLASSDRVS = 0x00000002,
            DIOD_CANCEL_REMOVE = 0x00000004
        }

        public enum DiCreateDevice : int
        {
            DICD_GENERATE_ID = 0x00000001,
            DICD_INHERIT_CLASSDRVS = 0x00000002
        }

        public enum DiDeviceRegistryProperty : int
        {
            SPDRP_DEVICEDESC = 0x00000000,
            SPDRP_HARDWAREID = 0x00000001,
            SPDRP_COMPATIBLEIDS = 0x00000002,
            SPDRP_UNUSED0 = 0x00000003,
            SPDRP_SERVICE = 0x00000004,
            SPDRP_UNUSED1 = 0x00000005,
            SPDRP_UNUSED2 = 0x00000006,
            SPDRP_CLASS = 0x00000007,
            SPDRP_CLASSGUID = 0x00000008,
            SPDRP_DRIVER = 0x00000009,
            SPDRP_CONFIGFLAGS = 0x0000000A,
            SPDRP_MFG = 0x0000000B,
            SPDRP_FRIENDLYNAME = 0x0000000C,
            SPDRP_LOCATION_INFORMATION = 0x0000000D,
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E,
            SPDRP_CAPABILITIES = 0x0000000F,
            SPDRP_UI_NUMBER = 0x00000010,
            SPDRP_UPPERFILTERS = 0x00000011,
            SPDRP_LOWERFILTERS = 0x00000012,
            SPDRP_BUSTYPEGUID = 0x00000013,
            SPDRP_LEGACYBUSTYPE = 0x00000014,
            SPDRP_BUSNUMBER = 0x00000015,
            SPDRP_ENUMERATOR_NAME = 0x00000016,
            SPDRP_SECURITY = 0x00000017,
            SPDRP_SECURITY_SDS = 0x00000018,
            SPDRP_DEVTYPE = 0x00000019,
            SPDRP_EXCLUSIVE = 0x0000001A,
            SPDRP_CHARACTERISTICS = 0x0000001B,
            SPDRP_ADDRESS = 0x0000001C,
            SPDRP_UI_NUMBER_DESC_FORMAT = 0X0000001D,
            SPDRP_DEVICE_POWER_DATA = 0x0000001E,
            SPDRP_REMOVAL_POLICY = 0x0000001F,
            SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020,
            SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021,
            SPDRP_INSTALL_STATE = 0x00000022,
            SPDRP_LOCATION_PATHS = 0x00000023,
            SPDRP_BASE_CONTAINERID = 0x00000024,
            SPDRP_MAXIMUM_PROPERTY = 0x00000025
        }

        public enum DIIRFLAG : uint
        {
            DIIRFLAG_INF_ALREADY_COPIED = 0x00000001,
            DIIRFLAG_FORCE_INF = 0x00000002,
            DIIRFLAG_HW_USING_THE_INF = 0x00000004,
            DIIRFLAG_HOTPATCH = 0x00000008,
            DIIRFLAG_NOBACKUP = 0x00000010
        }

        public enum DEVICE_POWER_STATE : int
        {
            PowerDeviceUnspecified = 0,
            PowerDeviceD0 = 1,
            PowerDeviceD1 = 2,
            PowerDeviceD2 = 3,
            PowerDeviceD3 = 4,
            PowerDeviceMaximum = 5
        }

        public enum SYSTEM_POWER_STATE : int
        {
            PowerSystemUnspecified = 0,
            PowerSystemWorking = 1,
            PowerSystemSleeping1 = 2,
            PowerSystemSleeping2 = 3,
            PowerSystemSleeping3 = 4,
            PowerSystemHibernate = 5,
            PowerSystemShutdown = 6,
            PowerSystemMaximum = 7
        }

        public enum PDCAP : uint
        {
            PDCAP_D0_SUPPORTED = 0x00000001,
            PDCAP_D1_SUPPORTED = 0x00000002,
            PDCAP_D2_SUPPORTED = 0x00000004,
            PDCAP_D3_SUPPORTED = 0x00000008,
            PDCAP_S0_SUPPORTED = 0x00010000,
            PDCAP_S1_SUPPORTED = 0x00020000,
            PDCAP_S2_SUPPORTED = 0x00040000,
            PDCAP_S3_SUPPORTED = 0x00080000,
            PDCAP_S4_SUPPORTED = 0x01000000,
            PDCAP_S5_SUPPORTED = 0x02000000,
            PDCAP_WAKE_FROM_D0_SUPPORTED = 0x00000010,
            PDCAP_WAKE_FROM_D1_SUPPORTED = 0x00000020,
            PDCAP_WAKE_FROM_D2_SUPPORTED = 0x00000040,
            PDCAP_WAKE_FROM_D3_SUPPORTED = 0x00000080,
            PDCAP_WAKE_FROM_S0_SUPPORTED = 0x00100000,
            PDCAP_WAKE_FROM_S1_SUPPORTED = 0x00200000,
            PDCAP_WAKE_FROM_S2_SUPPORTED = 0x00400000,
            PDCAP_WAKE_FROM_S3_SUPPORTED = 0x00800000,
            PDCAP_WARM_EJECT_SUPPORTED = 0x00000100
        }
    }
}