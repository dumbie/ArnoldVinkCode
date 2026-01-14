using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class ApiGitHub
    {
        public static async Task<string> ApiGitHub_GetLatestVersion(string userName, string repoName)
        {
            try
            {
                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestToken = new[] { "Authorization", "token " + ApiTokens.GitHub };
                string[][] requestHeaders = new string[][] { requestAccept, requestToken };

                //Download latest releases
                string currentVersion = await AVDownloader.DownloadStringAsync(5000, repoName, requestHeaders, GetPathLatestReleases(userName, repoName));
                if (string.IsNullOrWhiteSpace(currentVersion))
                {
                    Debug.WriteLine("Failed to get latest GitHub version, empty string.");
                    return string.Empty;
                }

                //Deserialize latest releases
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