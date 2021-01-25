using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVImage
    {
        public enum BI_COMPRESSION : uint
        {
            BI_RGB = 0x0000,
            BI_RLE8 = 0x0001,
            BI_RLE4 = 0x0002,
            BI_BITFIELDS = 0x0003,
            BI_JPEG = 0x0004,
            BI_PNG = 0x0005,
            BI_CMYK = 0x000B,
            BI_CMYKRLE8 = 0x000C,
            BI_CMYKRLE4 = 0x000D
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public uint bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public uint bfOffBits;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public BI_COMPRESSION biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct MEMICONDIR
        {
            public ushort wReserved;
            public ushort wType;
            public ushort wCount;
            [MarshalAs(UnmanagedType.ByValArray)]
            public MEMICONDIRENTRY[] arEntries;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct MEMICONDIRENTRY
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort wPlanes;
            public ushort wBitCount;
            public uint dwBytesInRes;
            public ushort wIdentifier;
        }
    }
}