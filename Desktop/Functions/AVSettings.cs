using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace ArnoldVinkCode
{
    public partial class AVSettings
    {
        //Load - Application Config
        public static Configuration LoadConfig(string configName)
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
        public static bool Check(Configuration config, string settingName)
        {
            try
            {
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
        public static dynamic Load(Configuration config, string settingName, Type settingType)
        {
            try
            {
                return Convert.ChangeType(config.AppSettings.Settings[settingName].Value, settingType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load setting: " + settingName + " / " + ex.Message);
                return null;
            }
        }

        //Save - Application Setting
        public static bool Save(Configuration config, string settingName, object settingValue)
        {
            try
            {
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
    }
}