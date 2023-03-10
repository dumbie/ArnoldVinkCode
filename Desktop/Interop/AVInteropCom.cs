using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVInteropCom
    {
        //PropertyKey
        public static PropertyKey PKEY_AppUserModel_ID = new PropertyKey { fmtId = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), pId = 5 };
        public static PropertyKey PKEY_Device_FriendlyName = new PropertyKey { fmtId = new Guid("A45C254E-DF1C-4EFD-8020-67D146A850E0"), pId = 14 };

        [StructLayout(LayoutKind.Sequential)]
        public struct PropertyKey
        {
            public Guid fmtId;
            public uint pId;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
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
            [FieldOffset(8)] public FILETIME filetime;
        }

        [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyStore
        {
            int GetCount([Out] out uint propertyCount);
            int GetAt([In] uint propertyIndex, [Out, MarshalAs(UnmanagedType.Struct)] out PropertyKey key);
            PropertyVariant GetValue([In, MarshalAs(UnmanagedType.Struct)] ref PropertyKey key, [Out, MarshalAs(UnmanagedType.Struct)] out PropertyVariant pv);
            int SetValue([In, MarshalAs(UnmanagedType.Struct)] ref PropertyKey key, [In, MarshalAs(UnmanagedType.Struct)] ref PropertyVariant pv);
            int Commit();
        }
    }
}