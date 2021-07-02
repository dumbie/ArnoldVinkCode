using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArnoldVinkCode
{
    partial class ArnoldVinkSettings
    {
        //Load a specific application setting
        public static object AppSettingLoad(string SettingName)
        {
            try
            {
                //Check if there are any application settings
                if (!Application.Current.Properties.Any())
                {
                    Debug.WriteLine("App has no stored settings.");
                    return null;
                }

                if (Application.Current.Properties.ContainsKey(SettingName))
                {
                    //Debug.WriteLine("Setting " + SettingName + " has been found.");
                    return Application.Current.Properties[SettingName];
                }
                else
                {
                    Debug.WriteLine("Setting " + SettingName + " not found.");
                    return null;
                }
            }
            catch
            {
                Debug.WriteLine("Failed loading app setting: " + SettingName);
                return null;
            }
        }

        //Check a specific application setting
        public static bool AppSettingCheck(string SettingName)
        {
            try
            {
                //Check if there are any application settings
                if (!Application.Current.Properties.Any())
                {
                    Debug.WriteLine("App has no stored settings.");
                    return false;
                }

                if (Application.Current.Properties.ContainsKey(SettingName))
                {
                    Debug.WriteLine("Setting " + SettingName + " has been found.");
                    return true;
                }
                else
                {
                    Debug.WriteLine("Setting " + SettingName + " not found.");
                    return false;
                }
            }
            catch
            {
                Debug.WriteLine("Failed checking app setting: " + SettingName);
                return false;
            }
        }

        //Save a specific application setting
        public static async Task<bool> AppSettingSave(string SettingName, object SettingValue)
        {
            try
            {
                Application.Current.Properties[SettingName] = SettingValue;
                await Application.Current.SavePropertiesAsync();
                Debug.WriteLine("Setting " + SettingName + " has been updated to: " + SettingValue);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving setting: " + ex.Message);
                return false;
            }
        }

        //Remove a specific application setting
        public static async Task<bool> AppSettingRemove(string SettingName)
        {
            try
            {
                //Check if there are any application settings
                if (!Application.Current.Properties.Any())
                {
                    Debug.WriteLine("App has no stored settings.");
                    return false;
                }

                Application.Current.Properties[SettingName] = null;
                await Application.Current.SavePropertiesAsync();
                Debug.WriteLine("Setting " + SettingName + " has been removed.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed removing setting: " + ex.Message);
                return false;
            }
        }
    }
}