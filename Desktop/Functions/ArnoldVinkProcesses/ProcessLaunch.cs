﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        /// <summary>
        /// Launch application by ShellExecute
        /// </summary>
        /// <summary>Disables UIAccess from launch process when running as admin.</summary>
        public static bool Launch_ShellExecute(string exePath, string workPath, string arguments, bool asAdmin)
        {
            IntPtr launchTokenHandle = IntPtr.Zero;
            try
            {
                //Check execute path
                if (string.IsNullOrWhiteSpace(exePath))
                {
                    AVDebug.WriteLine("Shell execute failed: execute path is empty.");
                    return false;
                }

                //Check file executable extension
                if (asAdmin)
                {
                    string[] fileExecutables = { "exe", "bat", "cmd", "com", "pif" };
                    string fileExtension = Path.GetExtension(exePath).ToLower();
                    if (!fileExecutables.Contains(fileExtension))
                    {
                        Debug.WriteLine("No executable detected, running as normal user.");
                        asAdmin = false;
                    }
                }

                //Set shell execute info
                ShellExecuteInfo shellExecuteInfo = new ShellExecuteInfo();
                shellExecuteInfo.nShow = WindowShowCommand.Show;
                shellExecuteInfo.fMask = SHELL_EXECUTE_SEE_MASK.SEE_MASK_NOCLOSEPROCESS;
                shellExecuteInfo.lpVerb = asAdmin ? "runas" : "open";
                shellExecuteInfo.lpFile = exePath;

                //Check for url protocol
                if (!Check_PathUrlProtocol(exePath))
                {
                    if (!string.IsNullOrWhiteSpace(arguments))
                    {
                        shellExecuteInfo.lpParameters = arguments;
                    }
                    if (!string.IsNullOrWhiteSpace(workPath) && Directory.Exists(workPath))
                    {
                        shellExecuteInfo.lpDirectory = workPath;
                    }
                    else
                    {
                        shellExecuteInfo.lpDirectory = Path.GetDirectoryName(exePath);
                        AVDebug.WriteLine("Workpath is empty or missing, using exepath.");
                    }
                }

                //Shell execute process
                bool shellExecuteResult = false;
                if (asAdmin)
                {
                    //Get current process token
                    launchTokenHandle = Token_Create_Current();

                    //Disable token ui access
                    Token_Adjust_UIAccess(ref launchTokenHandle, false);

                    //Shell execute inherit user
                    AVDebug.WriteLine("Shell executing with inherited access: " + exePath);
                    shellExecuteResult = ShellExecuteExW(shellExecuteInfo);
                }
                else
                {
                    //Shell execute normal user
                    AVDebug.WriteLine("Shell executing with user access: " + exePath);
                    shellExecuteResult = ShellExecuteUser(shellExecuteInfo);
                }

                //Check execute result
                if (!shellExecuteResult)
                {
                    AVDebug.WriteLine("Shell execute failed: " + exePath);
                    return false;
                }
                else
                {
                    AVDebug.WriteLine("Shell execute succeeded: " + exePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Shell execute failed: " + exePath + "/" + ex.Message);
                return false;
            }
            finally
            {
                CloseHandleAuto(launchTokenHandle);
            }
        }

        /// <summary>
        /// Launch UWP or Win32Store application
        /// </summary>
        public static bool Launch_UwpApplication(string appUserModelId, string arguments)
        {
            try
            {
                //Show launching message
                AVDebug.WriteLine("Launching UWP or Win32Store application: " + appUserModelId + "/" + arguments);

                //Start the process
                UWPActivationManager UWPActivationManager = new UWPActivationManager();
                UWPActivationManager.ActivateApplication(appUserModelId, arguments, UWP_ACTIVATEOPTIONS.AO_NONE, out int processId);

                //Return process id
                AVDebug.WriteLine("Launched UWP or Win32Store process identifier: " + processId);
                return processId > 0;
            }
            catch (Exception ex)
            {
                AVDebug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + "/" + ex.Message);
                return false;
            }
        }
    }
}