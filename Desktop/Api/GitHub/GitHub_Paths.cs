using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class ApiGitHub
    {
        public static string ApiGitHub_GetDownloadPath(string userName, string projectName, string fileName)
        {
            try
            {
                return "https://github.com/" + userName + "/" + projectName + "/releases/latest/download/" + fileName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get GitHub download path: " + ex.Message);
                return string.Empty;
            }
        }
    }
}