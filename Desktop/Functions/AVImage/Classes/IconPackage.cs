using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MEMICONDIR
        {
            public ushort idReserved;
            public ushort idType;
            public ushort idCount;
            public MEMICONDIRENTRY idEntriesArray;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MEMICONDIRENTRY
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort wPlanes;
            public ushort wBitCount;
            public uint dwBytesInRes;
            public ushort nIdentifier;
        }
    }
}