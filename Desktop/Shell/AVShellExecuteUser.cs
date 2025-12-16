using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class AVShell
    {
        //Enumerators
        private enum ShellWindowTypeConstants : int
        {
            SWC_EXPLORER = 0,
            SWC_BROWSER = 0x1,
            SWC_3RDPARTY = 0x2,
            SWC_CALLBACK = 0x4,
            SWC_DESKTOP = 0x8
        }

        private enum ShellWindowFindWindowOptions : int
        {
            SWFO_NEEDDISPATCH = 0x1,
            SWFO_INCLUDEPENDING = 0x2,
            SWFO_COOKIEPASSED = 0x4
        }

        private enum ShellViewGetItemObject : uint
        {
            SVGIO_BACKGROUND = 0,
            SVGIO_SELECTION = 0x1,
            SVGIO_ALLVIEW = 0x2,
            SVGIO_CHECKED = 0x3,
            SVGIO_TYPE_MASK = 0xF,
            SVGIO_FLAG_VIEWORDER = 0x80000000
        }

        //Interfaces
        [ComImport, Guid("00020400-0000-0000-C000-000000000046")]
        private interface IDispatch { }

        [ComImport, Guid("85CB6900-4D95-11CF-960C-0080C7F4EE85")]
        private interface IShellWindows
        {
            void _VtblGap0_8();
            int FindWindowSW([In] ref object pvarLoc, [In] ref object pvarLocRoot, [In] ShellWindowTypeConstants swClass, out int phwnd, [In] ShellWindowFindWindowOptions swfwOptions, out IServiceProvider ppdispOut);
        }

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IServiceProvider
        {
            int QueryService(Guid guidService, Guid riid, out IShellBrowser ppvOut);
        }

        [ComImport, Guid("000214E2-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellBrowser
        {
            void _VtblGap0_12();
            int QueryActiveShellView(out IShellView ppshv);
        }

        [ComImport, Guid("000214E3-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellView
        {
            void _VtblGap0_12();
            int GetItemObject(ShellViewGetItemObject uItem, Guid riid, out IShellFolderViewDual ppvOut);
        }

        [ComImport, Guid("E7A1AF80-4D96-11CF-960C-0080C7F4EE85")]
        private interface IShellFolderViewDual
        {
            int get_Application(out IShellDispatch2 ppid);
        }

        [ComImport, Guid("A4C6892C-3BA9-11D2-9DEA-00C04FB16162")]
        private interface IShellDispatch2
        {
            void _VtblGap0_24();
            int ShellExecute([In] string File, [In, Optional] object vArgs, [In, Optional] object vDir, [In, Optional] object vOperation, [In, Optional] object vShow);
        }

        //Functions
        public static bool ShellExecuteUser(ShellExecuteInfo shellExecuteInfo)
        {
            try
            {
                //Create shell windows instance
                IShellWindows shellWindows = (IShellWindows)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ShellWindows));
                if (shellWindows == null)
                {
                    Debug.WriteLine("Failed to create shell windows instance.");
                    return false;
                }

                //Find shell desktop window
                object emptyObject = new object();
                shellWindows.FindWindowSW(ref emptyObject, ref emptyObject, ShellWindowTypeConstants.SWC_DESKTOP, out _, ShellWindowFindWindowOptions.SWFO_NEEDDISPATCH, out IServiceProvider desktopWindow);
                if (desktopWindow == null)
                {
                    Debug.WriteLine("Failed to find shell desktop window.");
                    return false;
                }

                //Query top level shell browser
                desktopWindow.QueryService(SID_STopLevelBrowser, typeof(IShellBrowser).GUID, out IShellBrowser shellBrowser);
                if (shellBrowser == null)
                {
                    Debug.WriteLine("Failed to query top level shell browser.");
                    return false;
                }

                //Query active shell view
                shellBrowser.QueryActiveShellView(out IShellView desktopView);
                if (desktopView == null)
                {
                    Debug.WriteLine("Failed to query active shell view.");
                    return false;
                }

                //Get shell folder view dual
                desktopView.GetItemObject(ShellViewGetItemObject.SVGIO_BACKGROUND, typeof(IDispatch).GUID, out IShellFolderViewDual shellFolderViewDual);
                if (shellFolderViewDual == null)
                {
                    Debug.WriteLine("Failed to get shell folder view dual.");
                    return false;
                }

                //Get application dispatch
                shellFolderViewDual.get_Application(out IShellDispatch2 dispApplication);
                if (dispApplication == null)
                {
                    Debug.WriteLine("Failed to get application dispatch.");
                    return false;
                }

                //Shell execute
                int executeResult = dispApplication.ShellExecute(shellExecuteInfo.lpFile, shellExecuteInfo.lpParameters, shellExecuteInfo.lpDirectory, shellExecuteInfo.lpVerb, shellExecuteInfo.nShow);

                //Return result
                return executeResult == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}