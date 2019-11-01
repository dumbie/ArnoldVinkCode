using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    class AVImports
    {
        [DllImport("api-ms-win-core-sysinfo-l1-2-1.dll")]
        internal static extern UInt64 GetTickCount64();
    }
}