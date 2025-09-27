using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVSettingsJson
    {
        //Variables
        private string vSettingFilePath = string.Empty;
        private JToken vJToken = null;

        //Initialize
        public AVSettingsJson(string settingFilePath)
        {
            try
            {
                vSettingFilePath = settingFilePath;
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
                //Load json settings file
                string jsonText = "{}";
                try
                {
                    jsonText = File.ReadAllText(vSettingFilePath);
                }
                catch
                {
                    Debug.WriteLine("Failed reading settings file, falling back to empty.");
                }

                //Parse json settings file
                vJToken = JToken.Parse(jsonText);

                //Return result
                Debug.WriteLine("Loaded settings file: " + vSettingFilePath);
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed loading settings file: " + vSettingFilePath + " / " + ex.Message);
                return false;
            }
        }

        //Save settings to file
        private bool FileSave()
        {
            try
            {
                //Convert json to string
                string jsonString = vJToken.ToString();

                //Save settings file
                File.WriteAllText(vSettingFilePath, jsonString);

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
                return vJToken[settingName] != null;
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
                vJToken[settingName].Parent.Remove();
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
                //Return result
                return Convert.ChangeType(vJToken[settingName], settingType, CultureInfo.InvariantCulture);
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
                //Set setting value
                vJToken[settingName] = settingValue;
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