using System;
using System.Diagnostics;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVSettings
    {
        //Check startup shortcut
        public static bool StartupShortcutCheck()
        {
            try
            {
                string targetName = AVFunctions.ApplicationName();
                string targetFileShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                return File.Exists(targetFileShortcut);
            }
            catch
            {
                return false;
            }
        }

        //Create or remove startup shortcut
        public static void StartupShortcutManage(string executableCustomName, bool useLauncher)
        {
            try
            {
                //Set shortcut details
                string targetName = AVFunctions.ApplicationName();
                string targetFileShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");
                string targetFilePath = AVFunctions.ApplicationPathExecutable();
                string targetExecutableFile = Path.GetFileName(targetFilePath);

                //Check custom executable
                if (!string.IsNullOrWhiteSpace(executableCustomName))
                {
                    targetFilePath = targetFilePath.Replace(targetExecutableFile, executableCustomName);
                }

                //Check launcher executable
                if (useLauncher)
                {
                    string executableLauncher = string.Empty;
                    if (File.Exists(targetName + "-Launcher.exe"))
                    {
                        executableLauncher = targetName + "-Launcher.exe";
                    }
                    else if (File.Exists("Launcher.exe"))
                    {
                        executableLauncher = "Launcher.exe";
                    }

                    if (!string.IsNullOrWhiteSpace(executableLauncher))
                    {
                        targetFilePath = targetFilePath.Replace(targetExecutableFile, executableLauncher);
                    }
                }

                //Check if the shortcut already exists
                if (!File.Exists(targetFileShortcut))
                {
                    Debug.WriteLine("Adding application to Windows startup: " + targetFilePath);
                    using (StreamWriter streamWriter = new StreamWriter(targetFileShortcut))
                    {
                        streamWriter.WriteLine("[InternetShortcut]");
                        streamWriter.WriteLine("URL=" + targetFilePath);
                        streamWriter.WriteLine("IconFile=" + targetFilePath);
                        streamWriter.WriteLine("IconIndex=0");
                        streamWriter.Flush();
                    }
                }
                else
                {
                    Debug.WriteLine("Removing application from Windows startup: " + targetFilePath);
                    File.Delete(targetFileShortcut);
                }
            }
            catch
            {
                Debug.WriteLine("Failed creating startup shortcut.");
            }
        }
    }
}