using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ApiGitHub;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public partial class AVUpdate
    {
        //Clean application update files
        public static async Task UpdateCleanup()
        {
            try
            {
                Debug.WriteLine("Cleaning application update.");

                //Close running application updater
                if (Close_ProcessesByName("Updater.exe", true))
                {
                    await Task.Delay(1000);
                }

                //Check if the updater has been updated
                File_Move("Resources/UpdaterReplace.exe", "Updater.exe", true);
                File_Move("Updater/UpdaterReplace.exe", "Updater.exe", true);
            }
            catch { }
        }

        //Launch updater and restart application
        public static void UpdateRestart()
        {
            try
            {
                Launch_ShellExecute("Updater.exe", "", "-ProcessLaunch", true);
                Environment.Exit(0);
            }
            catch { }
        }

        //Check for available application update
        public static async Task<bool> UpdateCheck(string gitUsername, string gitRepoName, bool silentUpdate)
        {
            try
            {
                Debug.WriteLine("Checking for application update.");

                string onlineVersion = (await ApiGitHub_GetLatestVersion(gitUsername, gitRepoName)).ToLower();
                string currentVersion = "v" + AVFunctions.ApplicationVersion();
                if (!string.IsNullOrWhiteSpace(onlineVersion) && onlineVersion != currentVersion)
                {
                    List<string> messageBoxAnswers = new List<string>();
                    messageBoxAnswers.Add("Update");
                    messageBoxAnswers.Add("Cancel");

                    string MsgBoxResult = await new AVMessageBox().Popup(null, "Newer version has been found: " + onlineVersion, "Would you like to update the application to the newest version available?", messageBoxAnswers);
                    if (MsgBoxResult == "Update")
                    {
                        UpdateRestart();
                    }

                    return true;
                }
                else
                {
                    if (!silentUpdate)
                    {
                        List<string> messageBoxAnswers = new List<string>();
                        messageBoxAnswers.Add("Ok");

                        await new AVMessageBox().Popup(null, "No new application update has been found.", "", messageBoxAnswers);
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed checking for application update: " + ex.Message);
                return false;
            }
        }
    }
}