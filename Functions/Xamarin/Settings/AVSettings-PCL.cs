using System;
using System.Diagnostics;
using System.Linq;
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
                    Debug.WriteLine("Setting " + SettingName + " has been found.");
                    return Application.Current.Properties[SettingName].ToString();
                }
                else
                {
                    Debug.WriteLine("Setting has not been found: " + SettingName);
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
                    Debug.WriteLine("Setting has not been found.");
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
        public static void AppSettingSave(string SettingName, object SettingValue)
        {
            try
            {
                Application.Current.Properties[SettingName] = SettingValue;
                Debug.WriteLine("Setting " + SettingName + " has been updated to: " + SettingValue);
            }
            catch (Exception ex) { Debug.WriteLine("Failed saving setting: " + ex.Message); }
        }

        //Remove a specific application setting
        public static void AppSettingRemove(string SettingName)
        {
            try
            {
                ////Check if the setting exists
                //if (AppSettingCheck(sName))
                //{
                //    AppSettings removeSetting = vAppSettings.Where(x => x.Name == sName).First();
                //    vAppSettings.Remove(removeSetting);

                //    //Save the application settings
                //    string SerializedList = JsonConvert.SerializeObject(vAppSettings);
                //    ArnoldVinkFiles AVFiles = DependencyService.Get<ArnoldVinkFiles>();
                //    await AVFiles.SaveText(@"AppSettings.json", SerializedList);

                //    Debug.WriteLine("Removed the app setting: " + sName);
                //}
                //else
                //{
                //    Debug.WriteLine("Setting to remove not found: " + sName);
                //    return;
                //}
            }
            catch (Exception ex) { Debug.WriteLine("Failed removing setting: " + ex.Message); }
        }
    }
}