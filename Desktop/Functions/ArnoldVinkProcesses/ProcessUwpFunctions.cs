using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;
using static ArnoldVinkCode.AVInteropCom;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;

namespace ArnoldVinkCode
{
    public partial class ProcessUwpFunctions
    {
        //Launch an uwp application manually
        public static async Task<Process> ProcessLauncherUwpAndWin32StoreAsync(string appUserModelId, string runArgument)
        {
            try
            {
                //Prepare the process launch
                Process TaskAction()
                {
                    try
                    {
                        //Show launching message
                        Debug.WriteLine("Launching UWP or Win32Store: " + appUserModelId + " / " + runArgument);

                        //Get detailed application information
                        Package appPackage = UwpGetAppPackageByAppUserModelId(appUserModelId);
                        AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);
                        appUserModelId = appxDetails.FamilyNameId;

                        //Start the process
                        UWPActivationManager UWPActivationManager = new UWPActivationManager();
                        UWPActivationManager.ActivateApplication(appUserModelId, runArgument, UWPActivationManagerOptions.None, out int processId);

                        //Return process
                        Process returnProcess = GetProcessById(processId);
                        Debug.WriteLine("Launched UWP or Win32Store process identifier: " + returnProcess.Id);
                        return returnProcess;
                    }
                    catch { }
                    Debug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + " / " + runArgument);
                    return null;
                };

                //Launch the process
                return await AVActions.TaskStartReturn(TaskAction);
            }
            catch { }
            Debug.WriteLine("Failed launching UWP or Win32Store: " + appUserModelId + " / " + runArgument);
            return null;
        }

        //Get uwp process by window handle
        public static Process GetUwpProcessByWindowHandle(IntPtr targetWindowHandle)
        {
            try
            {
                //Get process from the window handle
                IntPtr threadWindowHandleEx = FindWindowEx(targetWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                if (threadWindowHandleEx != IntPtr.Zero)
                {
                    int processId = GetProcessIdFromWindowHandle(threadWindowHandleEx);
                    if (processId > 0)
                    {
                        return GetProcessById(processId);
                    }
                }

                //Get process from the appx package
                string appUserModelId = GetAppUserModelIdFromWindowHandle(targetWindowHandle);
                Package appPackage = UwpGetAppPackageByAppUserModelId(appUserModelId);
                AppxDetails appxDetails = UwpGetAppxDetailsFromAppPackage(appPackage);
                return GetUwpProcessByProcessNameAndAppUserModelId(Path.GetFileNameWithoutExtension(appxDetails.ExecutableName), appUserModelId);
            }
            catch { }
            return null;
        }

        //Get uwp process by ProcessName and AppUserModelId
        public static Process GetUwpProcessByProcessNameAndAppUserModelId(string targetProcessName, string targetAppUserModelId)
        {
            try
            {
                Process[] uwpProcesses = GetProcessesByNameOrTitle(targetProcessName, false, true);
                foreach (Process uwpProcess in uwpProcesses)
                {
                    try
                    {
                        string processAppUserModelId = GetAppUserModelIdFromProcess(uwpProcess);
                        if (processAppUserModelId == targetAppUserModelId)
                        {
                            //Debug.WriteLine(targetProcessName + "/Id" + uwpProcess.Id + "/App" + processAppUserModelId + "vs" + targetAppUserModelId);
                            return uwpProcess;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        //Close an uwp application by window handle
        public static async Task<bool> CloseProcessUwpByWindowHandleOrProcessId(string appName, int ProcessId, IntPtr ProcessWindowHandle)
        {
            try
            {
                if (ProcessWindowHandle != IntPtr.Zero)
                {
                    //Show the process
                    await FocusProcessWindow(appName, ProcessId, ProcessWindowHandle, 0, false, false);
                    await Task.Delay(500);

                    //Close the process or app
                    return CloseProcessByWindowHandle(ProcessWindowHandle);
                }
                else if (ProcessId > 0)
                {
                    //Close the process or app
                    return CloseProcessById(ProcessId);
                }
            }
            catch { }
            return false;
        }

        //Restart a uwp process or app
        public static async Task<Process> RestartProcessUwp(string processName, string processAppUserModelId, int processId, IntPtr processWindowHandle, string processArgument)
        {
            try
            {
                //Close the process or app
                await CloseProcessUwpByWindowHandleOrProcessId(processName, processId, processWindowHandle);
                await Task.Delay(1000);

                //Relaunch the process or app
                return await ProcessLauncherUwpAndWin32StoreAsync(processAppUserModelId, processArgument);
            }
            catch { }
            return null;
        }

        //Check if a window is an uwp application
        public static bool CheckProcessIsUwp(IntPtr TargetWindowHandle)
        {
            try
            {
                string ClassNamestring = GetClassNameFromWindowHandle(TargetWindowHandle);
                if (ClassNamestring == "ApplicationFrameWindow" || ClassNamestring == "Windows.UI.Core.CoreWindow")
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Get an uwp application window from CoreWindowHandle
        public static IntPtr UwpGetWindowFromCoreWindowHandle(IntPtr TargetCoreWindowHandle)
        {
            try
            {
                Process AllProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false);
                if (AllProcess != null)
                {
                    foreach (ProcessThread ThreadProcess in AllProcess.Threads)
                    {
                        foreach (IntPtr ThreadWindowHandle in EnumThreadWindows(ThreadProcess.Id))
                        {
                            try
                            {
                                //Get class name
                                string ClassNameString = GetClassNameFromWindowHandle(ThreadWindowHandle);

                                //Get information from frame window
                                if (ClassNameString == "ApplicationFrameWindow")
                                {
                                    IntPtr ThreadWindowHandleEx = FindWindowEx(ThreadWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                                    if (ThreadWindowHandleEx == TargetCoreWindowHandle)
                                    {
                                        return ThreadWindowHandle;
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Get uwp application package by FamilyName
        public static Package UwpGetAppPackageByFamilyName(string familyName)
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

        //Get uwp application package by AppUserModelId
        public static Package UwpGetAppPackageByAppUserModelId(string appUserModelId)
        {
            try
            {
                //Debug.WriteLine("Loading app package: " + appUserModelId);

                //Extract the family name from AppUserModelId
                string appFamilyName = appUserModelId.Split('!')[0];

                //Find the application package
                PackageManager deployPackageManager = new PackageManager();
                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                return deployPackageManager.FindPackagesForUser(currentUserIdentity, appFamilyName).FirstOrDefault();
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
                Package appPackage = UwpGetAppPackageByAppUserModelId(appUserModelId);

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

        //Get uwp application details from package
        public static AppxDetails UwpGetAppxDetailsFromAppPackage(Package appPackage)
        {
            IStream inputStream = null;
            IAppxFactory appxFactory = (IAppxFactory)new AppxFactory();
            AppxDetails appxDetails = new AppxDetails();
            try
            {
                //Get detailed information from app package
                string appFamilyName = appPackage.Id.FamilyName;
                appxDetails.FullPackageName = appPackage.Id.FullName;
                appxDetails.InstallPath = appPackage.InstalledLocation.Path;
                string manifestPath = appxDetails.InstallPath + "/AppXManifest.xml";
                //Debug.WriteLine("Reading uwp app manifest file: " + manifestPath);

                //Open the uwp application manifest file
                SHCreateStreamOnFileEx(manifestPath, STGM_MODES.STGM_SHARE_DENY_NONE, 0, false, IntPtr.Zero, out inputStream);
                if (inputStream != null)
                {
                    IAppxManifestReader appxManifestReader = appxFactory.CreateManifestReader(inputStream);
                    IAppxManifestApplication appxManifestApplication = appxManifestReader.GetApplications().GetCurrent();

                    //Get and set the application executable name
                    appxManifestApplication.GetStringValue("Executable", out string executableName);
                    appxDetails.ExecutableName = Path.GetFileName(executableName);

                    //Get and set the family name identifier
                    appxManifestApplication.GetStringValue("Id", out string appIdentifier);
                    appxDetails.FamilyNameId = appFamilyName + "!" + appIdentifier;

                    //Get and set the application display name
                    appxManifestApplication.GetStringValue("DisplayName", out string displayName);
                    appxDetails.DisplayName = UwpGetMsResourceString(appIdentifier, appxDetails.FullPackageName, displayName);

                    //Get all the available application logo images
                    appxManifestApplication.GetStringValue("Square30x30Logo", out appxDetails.Square30x30Logo);
                    appxManifestApplication.GetStringValue("Square70x70Logo", out appxDetails.Square70x70Logo);
                    appxManifestApplication.GetStringValue("Square150x150Logo", out appxDetails.Square150x150Logo);
                    appxManifestApplication.GetStringValue("Square310x310Logo", out appxDetails.Square310x310Logo);
                    appxManifestApplication.GetStringValue("Wide310x150Logo", out appxDetails.Wide310x150Logo);

                    //Check the largest available square logo
                    if (!string.IsNullOrWhiteSpace(appxDetails.Square310x310Logo))
                    {
                        appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxDetails.Square310x310Logo);
                    }
                    else if (!string.IsNullOrWhiteSpace(appxDetails.Square150x150Logo))
                    {
                        appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxDetails.Square150x150Logo);
                    }
                    else if (!string.IsNullOrWhiteSpace(appxDetails.Square70x70Logo))
                    {
                        appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxDetails.Square70x70Logo);
                    }
                    else if (!string.IsNullOrWhiteSpace(appxDetails.Square30x30Logo))
                    {
                        appxDetails.SquareLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxDetails.Square30x30Logo);
                    }
                    string originalSquareLargestLogoPath = appxDetails.SquareLargestLogoPath;
                    appxDetails.SquareLargestLogoPath = UwpGetImageSizePath(appxDetails.SquareLargestLogoPath);

                    //Check if the file can be accessed
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(appxDetails.SquareLargestLogoPath))
                        {
                            FileStream fileStream = File.OpenRead(appxDetails.SquareLargestLogoPath);
                            fileStream.Dispose();
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
                    if (!string.IsNullOrWhiteSpace(appxDetails.Wide310x150Logo))
                    {
                        appxDetails.WideLargestLogoPath = Path.Combine(appxDetails.InstallPath, appxDetails.Wide310x150Logo);
                    }
                    string originalWideLargestLogoPath = appxDetails.WideLargestLogoPath;
                    appxDetails.WideLargestLogoPath = UwpGetImageSizePath(appxDetails.WideLargestLogoPath);

                    //Check if the file can be accessed
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(appxDetails.WideLargestLogoPath))
                        {
                            FileStream fileStream = File.OpenRead(appxDetails.WideLargestLogoPath);
                            fileStream.Dispose();
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading details from uwp manifest: " + appPackage.Id.FamilyName + "/" + ex.Message);
            }

            Marshal.ReleaseComObject(inputStream);
            Marshal.ReleaseComObject(appxFactory);
            return appxDetails;
        }

        //Get ms uwp resources string from package
        public static string UwpGetMsResourceString(string appIdentifier, string packageFullName, string resourceString)
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
        public static string UwpGetImageSizePath(string imagePath)
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

                string[] imageFilesAllSizes = Directory.GetFiles(fileDirectory, fileName + "*" + fileExtension, SearchOption.AllDirectories);
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

        //Convert Indirect UWP application information to string
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