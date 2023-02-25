using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVUwpAppx
    {
        //Get uwp application details from package
        public static AppxDetails GetUwpAppxDetailsByAppPackage(Package appPackage)
        {
            AppxDetails appxDetails = new AppxDetails();
            try
            {
                //Get detailed information from app package
                appxDetails.InstallPath = appPackage.InstalledLocation.Path;
                string manifestPath = appxDetails.InstallPath + "\\AppXManifest.xml";
                //Debug.WriteLine("Reading uwp app manifest file: " + manifestPath);

                //Open uwp application manifest file
                AppxManifest appxManifest = null;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppxManifest));
                using (FileStream myFileStream = new FileStream(manifestPath, FileMode.Open, FileAccess.Read))
                {
                    appxManifest = (AppxManifest)xmlSerializer.Deserialize(myFileStream);
                }

                //Get first appx application
                AppxApplication appxApplication = appxManifest.Applications.Application.FirstOrDefault();

                //Get and set executable name
                appxDetails.ExecutableName = Path.GetFileName(appxApplication.Executable);

                //Get and set executable alias name
                if (appxApplication.Extensions != null && appxApplication.Extensions.ExtensionUAP3 != null)
                {
                    foreach (ExtensionUAP3 extUAP3 in appxApplication.Extensions.ExtensionUAP3)
                    {
                        try
                        {
                            appxDetails.ExecutableAliasName = Path.GetFileName(extUAP3.AppExecutionAlias.ExecutionAlias.Alias);
                        }
                        catch { }
                    }
                }
                if (string.IsNullOrWhiteSpace(appxDetails.ExecutableAliasName))
                {
                    appxDetails.ExecutableAliasName = appxDetails.ExecutableName;
                }

                //Get and set application names
                appxDetails.AppIdentifier = appxApplication.Id;
                appxDetails.AppUserModelId = appPackage.Id.FamilyName + "!" + appxApplication.Id;
                appxDetails.FamilyName = appPackage.Id.FamilyName;
                appxDetails.FullPackageName = appPackage.Id.FullName;

                //Get and set application display name
                string displayNameResource = appxApplication.VisualElements.DisplayName;
                if (string.IsNullOrWhiteSpace(displayNameResource))
                {
                    displayNameResource = appxManifest.Properties.DisplayName;
                }
                appxDetails.DisplayName = GetUwpMsResourceString(appxApplication.Id, appxDetails.FullPackageName, displayNameResource);

                //Check the largest available square logo
                if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.Square310x310Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.Square310x310Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.DefaultTile.Square310x310Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.DefaultTile.Square310x310Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.Square150x150Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.Square150x150Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.DefaultTile.Square150x150Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.DefaultTile.Square150x150Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.Square71x71Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.Square71x71Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.DefaultTile.Square71x71Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.DefaultTile.Square71x71Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.Square44x44Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.Square44x44Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.DefaultTile.Square44x44Logo))
                {
                    appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.DefaultTile.Square44x44Logo);
                }
                string originalSquareLargestLogoPath = appxDetails.SquareLargestLogoPath;
                appxDetails.SquareLargestLogoPath = GetUwpImageSizePath(appxDetails.SquareLargestLogoPath);

                //Check if the file can be accessed
                try
                {
                    if (!string.IsNullOrWhiteSpace(appxDetails.SquareLargestLogoPath))
                    {
                        using (new FileStream(appxDetails.SquareLargestLogoPath, FileMode.Open, FileAccess.Read)) { }
                    }
                    else
                    {
                        appxDetails.SquareLargestLogoPath = originalSquareLargestLogoPath;
                    }
                }
                catch
                {
                    //Debug.WriteLine("No permission to open: " + appxDetails.SquareLargestLogoPath);
                    appxDetails.SquareLargestLogoPath = originalSquareLargestLogoPath;
                }

                //Check the largest available wide logo
                if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.Wide310x150Logo))
                {
                    appxDetails.WideLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.Wide310x150Logo);
                }
                else if (!string.IsNullOrWhiteSpace(appxApplication.VisualElements.DefaultTile.Wide310x150Logo))
                {
                    appxDetails.WideLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxApplication.VisualElements.DefaultTile.Wide310x150Logo);
                }
                string originalWideLargestLogoPath = appxDetails.WideLargestLogoPath;
                appxDetails.WideLargestLogoPath = GetUwpImageSizePath(appxDetails.WideLargestLogoPath);

                //Check if the file can be accessed
                try
                {
                    if (!string.IsNullOrWhiteSpace(appxDetails.WideLargestLogoPath))
                    {
                        using (new FileStream(appxDetails.WideLargestLogoPath, FileMode.Open, FileAccess.Read)) { }
                    }
                    else
                    {
                        appxDetails.WideLargestLogoPath = originalWideLargestLogoPath;
                    }
                }
                catch
                {
                    //Debug.WriteLine("No permission to open: " + appxDetails.WideLargestLogoPath);
                    appxDetails.WideLargestLogoPath = originalWideLargestLogoPath;
                }

                //Debug.WriteLine("AppxDisplayName: " + appxDetails.DisplayName);
                //Debug.WriteLine("AppxExecutableName: " + appxDetails.ExecutableName);
                //Debug.WriteLine("AppxExecutableAliasName: " + appxDetails.ExecutableAliasName);
                //Debug.WriteLine("AppxSquareLargestLogoPath: " + appxDetails.SquareLargestLogoPath);
                //Debug.WriteLine("AppxWideLargestLogoPath: " + appxDetails.WideLargestLogoPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading details from uwp manifest: " + appPackage.Id.FamilyName + "/" + ex.Message);
            }
            return appxDetails;
        }

        /// <summary>
        /// Get uwp application package by FamilyName
        /// </summary>
        /// <param name="familyName">Example: Microsoft.WindowsCalculator_8wekyb3d8bbwe</param>
        public static Package GetUwpAppPackageByFamilyName(string familyName)
        {
            try
            {
                //Debug.WriteLine("Loading app package: " + familyName);

                //Find the application package
                PackageManager deployPackageManager = new PackageManager();
                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                return deployPackageManager.FindPackagesForUser(currentUserIdentity, familyName).FirstOrDefault();
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Get uwp application package by AppUserModelId
        /// </summary>
        /// <param name="appUserModelId">Example: Microsoft.WindowsCalculator_8wekyb3d8bbwe!App</param>
        public static Package GetUwpAppPackageByAppUserModelId(string appUserModelId)
        {
            try
            {
                //Extract the family name from AppUserModelId
                string appFamilyName = appUserModelId.Split('!')[0];
                //Debug.WriteLine("Loading app package: " + appFamilyName);

                //Find the application package
                PackageManager deployPackageManager = new PackageManager();
                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                return deployPackageManager.FindPackagesForUser(currentUserIdentity, appFamilyName).FirstOrDefault();
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Get uwp application package by FullPackageName
        /// </summary>
        /// <param name="fullPackageName">Example: Microsoft.WindowsCalculator_11.2209.0.0_x64__8wekyb3d8bbwe</param>
        public static Package GetUwpAppPackageByFullPackageName(string fullPackageName)
        {
            try
            {
                Debug.WriteLine("Loading app package: " + fullPackageName);

                //Find the application package
                PackageManager deployPackageManager = new PackageManager();
                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                return deployPackageManager.FindPackageForUser(currentUserIdentity, fullPackageName);
            }
            catch { }
            return null;
        }

        //Update uwp application package by AppUserModelId
        public static void UwpUpdateApplicationByAppUserModelId(string appUserModelId)
        {
            try
            {
                Debug.WriteLine("Updating app package: " + appUserModelId);

                //Get the application package
                Package appPackage = GetUwpAppPackageByAppUserModelId(appUserModelId);

                //Check for application update
                IAsyncOperation<PackageUpdateAvailabilityResult> updatePackage = appPackage.CheckUpdateAvailabilityAsync();
            }
            catch { }
        }

        //Remove uwp application package by PackageFullName
        public static bool UwpRemoveApplicationByPackageFullName(string packageFullName)
        {
            try
            {
                Debug.WriteLine("Removing app package: " + packageFullName);

                //Remove application from pc
                PackageManager deployPackageManager = new PackageManager();
                IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> removePackage = deployPackageManager.RemovePackageAsync(packageFullName, RemovalOptions.RemoveForAllUsers);

                //Check if application is removed
                return true;
            }
            catch { }
            return false;
        }

        //Get Microsoft uwp resource string from package
        public static string GetUwpMsResourceString(string appIdentifier, string packageFullName, string resourceString)
        {
            string convertedString = string.Empty;
            string resourceScheme = "ms-resource:";
            try
            {
                if (!resourceString.StartsWith(resourceScheme))
                {
                    return resourceString;
                }

                convertedString = ConvertIndirectString("@{" + packageFullName + "?" + resourceString + "}");
                if (!string.IsNullOrWhiteSpace(convertedString)) { return convertedString; }

                string resourceTarget = resourceString.Substring(resourceScheme.Length);

                string resourceString1 = resourceScheme + "///" + resourceTarget;
                convertedString = ConvertIndirectString("@{" + packageFullName + "?" + resourceString1 + "}");
                if (!string.IsNullOrWhiteSpace(convertedString)) { return convertedString; }

                string resourceString2 = resourceScheme + "///Resources/" + resourceTarget;
                convertedString = ConvertIndirectString("@{" + packageFullName + "?" + resourceString2 + "}");
                if (!string.IsNullOrWhiteSpace(convertedString)) { return convertedString; }

                string resourceString3 = resourceScheme + "///" + appIdentifier + "/" + resourceTarget;
                convertedString = ConvertIndirectString("@{" + packageFullName + "?" + resourceString3 + "}");
            }
            catch { }
            return convertedString;
        }

        //Check the available application image sizes
        public static string GetUwpImageSizePath(string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    //Debug.WriteLine("Uwp application image path not found.");
                    return string.Empty;
                }

                string fileExtension = Path.GetExtension(imagePath);
                string fileDirectory = Path.GetDirectoryName(imagePath);
                string fileName = Path.GetFileNameWithoutExtension(imagePath);
                string searchContrastBlack = "contrast-black";
                string searchContrastWhite = "contrast-white";

                List<string> imageFilesAllSizes = AVFiles.GetFilesLevel(fileDirectory, fileName + "*" + fileExtension, 2);
                string[] imageFilesContrastNone = imageFilesAllSizes.Where(x => !x.Contains(searchContrastBlack) && !x.Contains(searchContrastWhite)).ToArray();
                string[] imageFilesContrastBlack = imageFilesAllSizes.Where(x => x.Contains(searchContrastBlack)).ToArray();
                if (imageFilesContrastNone.Count() > 0)
                {
                    bool containsSize = imageFilesContrastNone.Any(x => Path.GetFileNameWithoutExtension(x).Any(char.IsDigit));
                    if (containsSize)
                    {
                        return imageFilesContrastNone.LastOrDefault();
                    }
                    else
                    {
                        return imageFilesContrastNone.FirstOrDefault();
                    }
                }
                else if (imageFilesContrastBlack.Count() > 0)
                {
                    bool containsSize = imageFilesContrastBlack.Any(x => Path.GetFileNameWithoutExtension(x).Any(char.IsDigit));
                    if (containsSize)
                    {
                        return imageFilesContrastBlack.LastOrDefault();
                    }
                    else
                    {
                        return imageFilesContrastBlack.FirstOrDefault();
                    }
                }
                else if (imageFilesAllSizes.Count() > 0)
                {
                    bool containsSize = imageFilesAllSizes.Any(x => Path.GetFileNameWithoutExtension(x).Any(char.IsDigit));
                    if (containsSize)
                    {
                        return imageFilesAllSizes.LastOrDefault();
                    }
                    else
                    {
                        return imageFilesAllSizes.FirstOrDefault();
                    }
                }
            }
            catch { }
            return imagePath;
        }

        //Convert indirect UWP application information to string
        public static string ConvertIndirectString(string indirectString)
        {
            try
            {
                StringBuilder indirectStringBuild = new StringBuilder(1024);
                SHLoadIndirectString(indirectString, indirectStringBuild, indirectStringBuild.Capacity, IntPtr.Zero);
                return indirectStringBuild.ToString();
            }
            catch { }
            return string.Empty;
        }
    }
}