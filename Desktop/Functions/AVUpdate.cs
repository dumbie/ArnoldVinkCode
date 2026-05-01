using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ApiGitHub;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public partial class AVUpdate
    {
        //Update result class
        public class UpdateCheckResult
        {
            public bool UpdateFound { get; set; }
            public string UpdateVersion { get; set; }
        }

        //Clean application update files
        public static async Task UpdateCleanup()
        {
            try
            {
                Debug.WriteLine("Cleaning application update.");

                //Close running application updater
                if (Close_ProcessByName("Updater.exe", true))
                {
                    await Task.Delay(500);
                }

                //Move new updater executable file
                File_Move("Settings\\UpdaterReplace.exe", "Updater.exe", true);
            }
            catch { }
        }

        //Launch updater and restart application
        public static void UpdateRestart()
        {
            try
            {
                //Launch updater
                Launch_ApplicationDesktop("Updater.exe", "", "-ProcessLaunch", true);

                //Exit application
                Environment.Exit(0);
            }
            catch { }
        }

        //Check for available application update
        public static async Task<UpdateCheckResult> UpdateCheck(string gitUsername, string gitRepoName)
        {
            UpdateCheckResult updateCheckResult = new UpdateCheckResult();
            try
            {
                Debug.WriteLine("Checking for application update: " + gitUsername + " / " + gitRepoName);

                //Get online version
                string onlineVersion = (await ApiGitHub_GetLatestVersion(gitUsername, gitRepoName)).ToLower();

                //Get current version
                string currentVersion = "v" + AVFunctions.ApplicationVersion();

                //Check if version matches
                if (!string.IsNullOrWhiteSpace(onlineVersion) && currentVersion != onlineVersion)
                {
                    Debug.WriteLine("Application update found: " + onlineVersion + " / " + currentVersion);
                    updateCheckResult.UpdateFound = true;
                    updateCheckResult.UpdateVersion = onlineVersion;
                }
                else
                {
                    Debug.WriteLine("No application update found.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed checking application update: " + ex.Message);
            }
            //Return result
            return updateCheckResult;
        }
    }
}