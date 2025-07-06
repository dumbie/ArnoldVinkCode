namespace ArnoldVinkCode
{
    public partial class AVShell
    {
        //Enumerators
        public enum RecycleBin_FLAGS : uint
        {
            SHRB_NOCONFIRMATION = 0x00000001,
            SHRB_NOPROGRESSUI = 0x00000002,
            SHRB_NOSOUND = 0x00000004
        }

        public enum FILEOP_FUNC : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004
        }

        public enum FILEOP_FLAGS : uint
        {
            FOF_MULTIDESTFILES = 0x1,
            FOF_CONFIRMMOUSE = 0x2,
            FOF_SILENT = 0x4,
            FOF_RENAMEONCOLLISION = 0x8,
            FOF_NOCONFIRMATION = 0x10,
            FOF_WANTMAPPINGHANDLE = 0x20,
            FOF_ALLOWUNDO = 0x40,
            FOF_FILESONLY = 0x80,
            FOF_SIMPLEPROGRESS = 0x100,
            FOF_NOCONFIRMMKDIR = 0x200,
            FOF_NOERRORUI = 0x400,
            FOF_NOCOPYSECURITYATTRIBS = 0x800,
            FOF_NORECURSION = 0x1000,
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,
            FOF_WANTNUKEWARNING = 0x4000,
            FOF_NORECURSEREPARSE = 0x8000
        }

        public enum STGM_MODES : uint
        {
            STGM_READ = 0x00000000,
            STGM_WRITE = 0x00000001,
            STGM_READWRITE = 0x00000002,
            STGM_READWRITE_Bits = 0x00000003,
            STGM_SHARE_DENY_NONE = 0x00000040,
            STGM_SHARE_DENY_READ = 0x00000030,
            STGM_SHARE_DENY_WRITE = 0x00000020,
            STGM_SHARE_EXCLUSIVE = 0x00000010
        }

        public enum GETPROPERTYSTORE_FLAGS : uint
        {
            GPS_DEFAULT = 0,
            GPS_HANDLERPROPERTIESONLY = 0x1,
            GPS_READWRITE = 0x2,
            GPS_TEMPORARY = 0x4,
            GPS_FASTPROPERTIESONLY = 0x8,
            GPS_OPENSLOWITEM = 0x10,
            GPS_DELAYCREATION = 0x20,
            GPS_BESTEFFORT = 0x40,
            GPS_NO_OPLOCK = 0x80,
            GPS_PREFERQUERYPROPERTIES = 0x100,
            GPS_EXTRINSICPROPERTIES = 0x200,
            GPS_EXTRINSICPROPERTIESONLY = 0x400,
            GPS_VOLATILEPROPERTIES = 0x800,
            GPS_VOLATILEPROPERTIESONLY = 0x1000,
            GPS_MASK_VALID = 0x1FFF
        }

        public enum SIGDN_FLAGS : uint
        {
            SIGDN_NORMALDISPLAY = 0,
            SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
            SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
            SIGDN_FILESYSPATH = 0x80058000,
            SIGDN_PARENTRELATIVE = 0x80080001,
            SIGDN_PARENTRELATIVEEDITING = 0x80031001,
            SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
            SIGDN_PARENTRELATIVEPARSING = 0x80018001,
            SIGDN_URL = 0x80068000
        }

        public enum SFGAO_FLAGS : uint
        {
            SFGAO_CANCOPY = 0x00000001,
            SFGAO_CANMOVE = 0x00000002,
            SFGAO_CANLINK = 0x00000004,
            SFGAO_STORAGE = 0x00000008,
            SFGAO_CANRENAME = 0x00000010,
            SFGAO_CANDELETE = 0x00000020,
            SFGAO_HASPROPSHEET = 0x00000040,
            SFGAO_DROPTARGET = 0x00000100,
            SFGAO_CAPABILITYMASK = 0x00000177,
            SFGAO_SYSTEM = 0x00001000,
            SFGAO_ENCRYPTED = 0x00002000,
            SFGAO_ISSLOW = 0x00004000,
            SFGAO_GHOSTED = 0x00008000,
            SFGAO_LINK = 0x00010000,
            SFGAO_SHARE = 0x00020000,
            SFGAO_READONLY = 0x00040000,
            SFGAO_HIDDEN = 0x00080000,
            SFGAO_DISPLAYATTRMASK = 0x000FC000,
            SFGAO_FILESYSANCESTOR = 0x10000000,
            SFGAO_FOLDER = 0x20000000,
            SFGAO_FILESYSTEM = 0x40000000,
            SFGAO_HASSUBFOLDER = 0x80000000,
            SFGAO_CONTENTSMASK = 0x80000000,
            SFGAO_VALIDATE = 0x01000000,
            SFGAO_REMOVABLE = 0x02000000,
            SFGAO_COMPRESSED = 0x04000000,
            SFGAO_BROWSABLE = 0x08000000,
            SFGAO_NONENUMERATED = 0x00100000,
            SFGAO_NEWCONTENT = 0x00200000,
            SFGAO_CANMONIKER = 0x00400000,
            SFGAO_HASSTORAGE = 0x00400000,
            SFGAO_STREAM = 0x00400000,
            SFGAO_STORAGEANCESTOR = 0x00800000,
            SFGAO_STORAGECAPMASK = 0x70C50008
        }

        public enum SICHINT_FLAGS : uint
        {
            SICHINT_DISPLAY = 0x00000000,
            SICHINT_ALLFIELDS = 0x80000000,
            SICHINT_CANONICAL = 0x10000000,
            SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000
        }
    }
}