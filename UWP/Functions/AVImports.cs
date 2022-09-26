using System;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public class AVImports
    {
        [DllImport("api-ms-win-core-sysinfo-l1-2-1.dll")]
        public static extern UInt64 GetTickCount64();
    }
}