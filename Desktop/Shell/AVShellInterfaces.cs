using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVShell
    {
        //Interfaces
        [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItem { }

        [ComImport, Guid("7E9FB0D3-919F-4307-AB2E-9B1860310C93"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItem2
        {
            object BindToHandler(object pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
            IShellItem GetParent();
            string GetDisplayName(SIGDN_FLAGS sigdnName);
            SFGAO_FLAGS GetAttributes(SFGAO_FLAGS sfgaoMask);
            int Compare(IShellItem psi, SICHINT_FLAGS hint);
            object GetPropertyStore(GETPROPERTYSTORE_FLAGS flags, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
            object GetPropertyStoreWithCreateObject(GETPROPERTYSTORE_FLAGS flags, [MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
            object GetPropertyStoreForKeys([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] PropertyKey[] rgKeys, uint cKeys, GETPROPERTYSTORE_FLAGS flags, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
            object GetPropertyDescriptionList([In] ref PropertyKey keyType, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
            void Update(object pbc);
            PropertyVariant GetProperty([In] ref PropertyKey key);
            Guid GetCLSID([In] ref PropertyKey key);
            ShellFileTime GetFileTime([In] ref PropertyKey key);
            int GetInt32([In] ref PropertyKey key);
            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetString([In] ref PropertyKey key);
            uint GetUInt32([In] ref PropertyKey key);
            ulong GetUInt64([In] ref PropertyKey key);
            bool GetBool([In] ref PropertyKey key);
        }

        [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyStore
        {
            int GetCount([Out] out uint propertyCount);
            int GetAt([In] uint propertyIndex, [Out, MarshalAs(UnmanagedType.Struct)] out PropertyKey key);
            PropertyVariant GetValue([In, MarshalAs(UnmanagedType.Struct)] PropertyKey key, [Out, MarshalAs(UnmanagedType.Struct)] out PropertyVariant pv);
            int SetValue([In, MarshalAs(UnmanagedType.Struct)] PropertyKey key, [In, MarshalAs(UnmanagedType.Struct)] ref PropertyVariant pv);
            int Commit();
        }
    }
}