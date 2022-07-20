using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class ApiGitHub
    {
        public static Uri GetPathLatestReleases(string userName, string repoName)
        {
            try
            {
                return new Uri("https://api.github.com/repos/" + userName + "/" + repoName + "/releases/latest", UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get GitHub latest releases path: " + ex.Message);
                return null;
            }
        }

        public static Uri GetPathLatestDownload(string userName, string repoName, string fileName)
        {
            try
            {
                return new Uri("https://github.com/" + userName + "/" + repoName + "/releases/latest/download/" + fileName, UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get GitHub latest download path: " + ex.Message);
                return null;
            }
        }
    }
}