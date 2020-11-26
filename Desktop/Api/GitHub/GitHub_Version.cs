using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ApiGitHub
    {
        public static async Task<string> ApiGitHub_GetLatestVersion(string userName, string projectName)
        {
            try
            {
                string currentVersion = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", null, new Uri("https://api.github.com/repos/" + userName + "/" + projectName + "/releases/latest"));
                if (string.IsNullOrWhiteSpace(currentVersion))
                {
                    Debug.WriteLine("Failed to get latest GitHub version, empty string.");
                    return string.Empty;
                }

                ClassReleases releaseInformation = JsonConvert.DeserializeObject<ClassReleases>(currentVersion);
                return releaseInformation.name;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get latest GitHub version: " + ex.Message);
                return string.Empty;
            }
        }
    }
}