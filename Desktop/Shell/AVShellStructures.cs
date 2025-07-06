using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVShell
    {
        //Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hWnd;
            [MarshalAs(UnmanagedType.U4)]
            public FILEOP_FUNC wFunc;
            public string pFrom;
            public string pTo;
            public FILEOP_FLAGS fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PropertyKey
        {
            public Guid fmtId;
            public uint pId;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct ShellFileTime
        {
            public int dwLowDateTime;
            public int dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct PropertyArray
        {
            public uint cElems;
            public IntPtr pElems;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct PropertyVariant
        {
            [FieldOffset(0)] public VarEnum varType;
            [FieldOffset(2)] public ushort wReserved1;
            [FieldOffset(4)] public ushort wReserved2;
            [FieldOffset(6)] public ushort wReserved3;
            [FieldOffset(8)] public byte bVal;
            [FieldOffset(8)] public sbyte cVal;
            [FieldOffset(8)] public ushort uiVal;
            [FieldOffset(8)] public short iVal;
            [FieldOffset(8)] public uint uintVal;
            [FieldOffset(8)] public int intVal;
            [FieldOffset(8)] public ulong ulVal;
            [FieldOffset(8)] public long lVal;
            [FieldOffset(8)] public float fltVal;
            [FieldOffset(8)] public double dblVal;
            [FieldOffset(8)] public short boolVal;
            [FieldOffset(8)] public IntPtr pclsidVal;
            [FieldOffset(8)] public IntPtr pszVal;
            [FieldOffset(8)] public IntPtr pwszVal;
            [FieldOffset(8)] public IntPtr punkVal;
            [FieldOffset(8)] public PropertyArray ca;
            [FieldOffset(8)] public ShellFileTime filetime;
        }
    }
}