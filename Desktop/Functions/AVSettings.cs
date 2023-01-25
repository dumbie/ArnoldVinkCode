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
        //AVSettings.Load(Configuration, "SettingName", typeof(Type));
        public static dynamic SettingLoad(Configuration config, string settingName, Type settingType)
        {
            try
            {
                if (config == null) { config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }
                return Convert.ChangeType(config.AppSettings.Settings[settingName].Value, settingType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load setting: " + settingName + " / " + ex.Message);
                return null;
            }
        }

        //Save - Application Setting
        public static bool SettingSave(Configuration config, string settingName, object settingValue)
        {
            try
            {
                if (config == null) { config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }
                config.AppSettings.Settings.Remove(settingName);
                config.AppSettings.Settings.Add(settingName, Convert.ToString(settingValue, CultureInfo.InvariantCulture));
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

        //Create or remove startup shortcut
        public static void ManageStartupShortcut(string executableName)
        {
            try
            {
                //Set application shortcut paths
                string targetFilePath = Assembly.GetEntryAssembly().CodeBase.Replace("file:///", string.Empty);
                if (!string.IsNullOrWhiteSpace(executableName))
                {
                    string originalExecutable = Path.GetFileName(targetFilePath);
                    targetFilePath = targetFilePath.Replace(originalExecutable, executableName);
                    Debug.WriteLine("Replaced executable: " + targetFilePath);
                }

                string targetName = Assembly.GetEntryAssembly().GetName().Name;
                string targetFileShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");

                //Check if the shortcut already exists
                if (!File.Exists(targetFileShortcut))
                {
                    Debug.WriteLine("Adding application to Windows startup.");
                    using (StreamWriter StreamWriter = new StreamWriter(targetFileShortcut))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + targetFilePath);
                        StreamWriter.WriteLine("IconFile=" + targetFilePath);
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }
                }
                else
                {
                    Debug.WriteLine("Removing application from Windows startup.");
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