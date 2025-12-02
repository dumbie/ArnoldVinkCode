using System;
using System.Diagnostics;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVSettings
    {
        public enum StartupShortcutType
        {
            StartMenu = 0,
            Startup = 1
        }

        //Check startup shortcut
        public static bool StartupShortcutCheck(StartupShortcutType targetType)
        {
            try
            {
                //Set shortcut type
                Environment.SpecialFolder shortcutType = Environment.SpecialFolder.StartMenu;
                if (targetType == StartupShortcutType.Startup)
                {
                    shortcutType = Environment.SpecialFolder.Startup;
                }

                //Get startup path
                string targetName = AVFunctions.ApplicationName();
                string targetFileShortcut = Path.Combine(Environment.GetFolderPath(shortcutType), targetName + ".url");

                //Return result
                return File.Exists(targetFileShortcut);
            }
            catch
            {
                return false;
            }
        }

        //Create or remove startup shortcut
        public static bool StartupShortcutManage(string executableCustomName, bool useLauncher, StartupShortcutType targetType)
        {
            try
            {
                //Set shortcut type
                Environment.SpecialFolder shortcutType = Environment.SpecialFolder.StartMenu;
                if (targetType == StartupShortcutType.Startup)
                {
                    shortcutType = Environment.SpecialFolder.Startup;
                }

                //Set shortcut details
                string targetName = AVFunctions.ApplicationName();
                string targetFileShortcut = Path.Combine(Environment.GetFolderPath(shortcutType), targetName + ".url");
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

                //Return result
                return true;
            }
            catch
            {
                //Return result
                Debug.WriteLine("Failed creating startup shortcut.");
                return false;
            }
        }
    }
}