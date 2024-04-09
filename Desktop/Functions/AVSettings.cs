using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace ArnoldVinkCode
{
    public partial class AVSettings
    {
        //Load - Application Config
        public static Configuration SettingLoadConfig(string configName)
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = configName;
                return ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load config: " + configName + " / " + ex.Message);
                return null;
            }
        }

        //Check - Application Setting
        public static bool SettingCheck(Configuration config, string settingName)
        {
            try
            {
                if (config == null) { config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }
                if (config.AppSettings.Settings[settingName] == null) { return false; } else { return true; }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check setting: " + settingName + " / " + ex.Message);
                return false;
            }
        }

        //Load - Application Setting
        //SettingLoad(Configuration, "SettingName", typeof(Type));
        public static dynamic SettingLoad(Configuration config, string settingName, Type settingType)
        {
            try
            {
                if (config == null) { config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }

                string loadValue = config.AppSettings.Settings[settingName].Value;
                if (settingType == typeof(double) || settingType == typeof(float) || settingType == typeof(decimal))
                {
                    loadValue = loadValue.Replace(",", ".");
                }

                return Convert.ChangeType(loadValue, settingType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load setting: " + settingName + " / " + ex.Message);
                return null;
            }
        }

        //Save - Application Setting
        public static bool SettingSave(Configuration config, string settingName, dynamic settingValue)
        {
            try
            {
                if (config == null) { config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }
                config.AppSettings.Settings.Remove(settingName);

                if (settingValue.GetType() == typeof(string))
                {
                    config.AppSettings.Settings.Add(settingName, settingValue);
                }
                else
                {
                    config.AppSettings.Settings.Add(settingName, Convert.ToString(settingValue, CultureInfo.InvariantCulture));
                }

                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save setting: " + settingName + " / " + ex.Message);
                return false;
            }
        }

        //Remove - Application Setting
        public static bool SettingRemove(Configuration config, string settingName)
        {
            try
            {
                if (config == null) { config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }
                config.AppSettings.Settings.Remove(settingName);
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove setting: " + settingName + " / " + ex.Message);
                return false;
            }
        }

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