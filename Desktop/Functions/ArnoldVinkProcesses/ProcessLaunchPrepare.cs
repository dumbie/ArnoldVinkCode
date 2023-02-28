using System;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public static int Launch_Prepare(string exePath, string workPath, string arguments, bool asNormal, bool asAdmin, bool allowUiAccess)
        {
            //Note: this might disable UIAccess from your process
            try
            {
                //Check required process action
                bool launchShellExecute = true;
                bool launchUnelevated = false;

                //Check current process access
                ProcessAccess currentProcessAccess = AVProcess.Get_ProcessAccessStatus(0, true);

                //Check for url protocol
                bool shellCommand = Check_PathUrlProtocol(exePath);
                if (shellCommand)
                {
                    AVDebug.WriteLine("Shell launch command detected.");
                    asAdmin = false;
                    workPath = string.Empty;
                    arguments = string.Empty;
                }
                else
                {
                    //Check launch access
                    if (asAdmin)
                    {
                        AVDebug.WriteLine("Starting process as administrator.");
                    }
                    else if (asNormal)
                    {
                        AVDebug.WriteLine("Starting process as normal user.");
                        if (currentProcessAccess.AdminAccess)
                        {
                            launchShellExecute = false;
                            launchUnelevated = true;
                        }
                    }

                    //Check working path
                    if (string.IsNullOrWhiteSpace(workPath))
                    {
                        workPath = Path.GetDirectoryName(exePath);
                        AVDebug.WriteLine("Workpath is empty, using exepath: " + workPath);
                    }
                    else if (!Directory.Exists(workPath))
                    {
                        workPath = Path.GetDirectoryName(exePath);
                        AVDebug.WriteLine("Workpath not found, using exepath: " + workPath);
                    }
                }

                //Set launch token
                IntPtr launchTokenHandle = IntPtr.Zero;
                if (launchUnelevated)
                {
                    launchTokenHandle = Token_Create_Unelevated();
                }
                else
                {
                    launchTokenHandle = Token_Create_Current();
                }

                //Duplicate launch token
                if (!launchShellExecute)
                {
                    launchTokenHandle = Token_Duplicate(launchTokenHandle);
                }

                //Adjust token ui access
                if (currentProcessAccess.UiAccess && !allowUiAccess)
                {
                    AVDebug.WriteLine("Starting process without uiaccess.");
                    Token_Adjust_UIAccess(ref launchTokenHandle, false);
                }

                //Launch application
                if (launchShellExecute)
                {
                    return Launch_ShellExecute(exePath, workPath, arguments, asAdmin);
                }
                else
                {
                    return Launch_CreateProcess(exePath, workPath, arguments, launchTokenHandle, currentProcessAccess.AdminAccess);
                }
            }
            catch { }
            return 0;
        }
    }
}