using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace ArnoldVinkCode
{
    public partial class AVSettingsConfig
    {
        //Variables
        private string vConfigName = string.Empty;
        private Configuration vConfig = null;

        //Initialize
        public AVSettingsConfig(string configName)
        {
            try
            {
                vConfigName = configName;
                FileLoad();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed initializing settings: " + ex.Message);
            }
        }

        //Load settings from file
        private bool FileLoad()
        {
            try
            {
                //Load config settings file
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = vConfigName;

                vConfig = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                if (vConfig == null) { vConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }

                //Return result
                Debug.WriteLine("Loaded settings file: " + vConfigName);
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed loading settings file: " + vConfigName + " / " + ex.Message);
                return false;
            }
        }

        //Save settings to file
        private bool FileSave()
        {
            try
            {
                //Save settings file
                vConfig.Save();
                ConfigurationManager.RefreshSection("appSettings");

                //Return result
                Debug.WriteLine("Saved settings file.");
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed saving settings file: " + ex.Message);
                return false;
            }
        }

        //Check if setting exists
        public bool Check(string settingName)
        {
            try
            {
                //Check setting
                return vConfig.AppSettings.Settings[settingName] != null;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed checking setting: "+ settingName + " / " + ex.Message);
                return false;
            }
        }

        //Remove setting
        public bool Remove(string settingName)
        {
            try
            {
                //Remove setting
                vConfig.AppSettings.Settings.Remove(settingName);
                Debug.WriteLine("Removed setting: " + settingName);

                //Save setting file
                FileSave();

                //Return result
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed removing setting: " + settingName + " / " + ex.Message);
                return false;
            }
        }

        //Load setting value
        /// <summary>
        /// Load("SettingName", typeof(Type));
        /// </summary>
        public dynamic Load(string settingName, Type settingType)
        {
            try
            {
                //Load setting
                string loadValue = vConfig.AppSettings.Settings[settingName].Value;
                if (settingType == typeof(double) || settingType == typeof(float) || settingType == typeof(decimal))
                {
                    loadValue = loadValue.Replace(",", ".");
                }

                //Return result
                return Convert.ChangeType(loadValue, settingType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed loading setting: " + settingName + " / " + ex.Message);
                return null;
            }
        }

        //Set setting value
        public bool Set(string settingName, dynamic settingValue)
        {
            try
            {
                //Remove setting
                vConfig.AppSettings.Settings.Remove(settingName);

                //Set setting value
                if (settingValue.GetType() == typeof(string))
                {
                    vConfig.AppSettings.Settings.Add(settingName, settingValue);
                }
                else
                {
                    vConfig.AppSettings.Settings.Add(settingName, Convert.ToString(settingValue, CultureInfo.InvariantCulture));
                }
                Debug.WriteLine("Setted value: " + settingName + " / " + (object)settingValue);

                //Save setting file
                FileSave();

                //Return result
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed setting value: " + settingName + " / " + ex.Message);
                return false;
            }
        }
    }
}