using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
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
        public static void ProcessLauncherUwpAndWin32Store(string pathExe, string argument)
        {
            try
            {
                //Show launching message
                Debug.WriteLine("Launching UWP or Win32Store: " + pathExe + "/" + argument);

                //Prepare the launching task
                void TaskAction()
                {
                    try
                    {
                        UWPActivationManager UWPActivationManager = new UWPActivationManager();
                        UWPActivationManager.ActivateApplication(pathExe, argument, UWPActivationManagerOptions.None, out int ProcessId);
                    }
                    catch { }
                }

                //Launch the application
                AVActions.TaskStart(TaskAction, null);
            }
            catch
            {
                Debug.WriteLine("Failed launching UWP or Win32Store: " + pathExe + "/" + argument);
            }
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

        //Get an uwp application process id by window handle
        public static async Task<int> GetUwpProcessIdByWindowHandle(string processTitle, string processAppUserModelId, IntPtr processWindowHandle)
        {
            try
            {
                //Show the uwp process
                GetWindowThreadProcessId(processWindowHandle, out int processIdTarget);
                await FocusProcessWindow(processTitle, processIdTarget, processWindowHandle, 0, false, false);
                await Task.Delay(500);

                //Get the process id
                ProcessMulti uwpRunningNew = UwpGetProcessMultiFromAppUserModelId(processAppUserModelId).Where(x => x.WindowHandle == processWindowHandle).FirstOrDefault();
                if (uwpRunningNew != null)
                {
                    Debug.WriteLine("Uwp workaround process id: " + uwpRunningNew.Identifier + " vs " + processIdTarget);
                    return uwpRunningNew.Identifier;
                }
            }
            catch { }
            return -1;
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
        public static async Task<bool> RestartProcessUwp(string appName, string pathExe, string argument, int processId, IntPtr processWindowHandle)
        {
            try
            {
                //Close the process or app
                await CloseProcessUwpByWindowHandleOrProcessId(appName, processId, processWindowHandle);
                await Task.Delay(1000);

                //Relaunch the process or app
                ProcessLauncherUwpAndWin32Store(pathExe, argument);
                return true;
            }
            catch { }
            return false;
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
                else
                {
                    return false;
                }
            }
            catch { return false; }
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

        //Get an uwp application process from AppUserModelId
        public static List<ProcessMulti> UwpGetProcessMultiFromAppUserModelId(string targetAppUserModelId)
        {
            List<ProcessMulti> processList = new List<ProcessMulti>();
            try
            {
                Process allProcess = GetProcessByNameOrTitle("ApplicationFrameHost", false);
                if (allProcess != null)
                {
                    foreach (ProcessThread threadProcess in allProcess.Threads)
                    {
                        try
                        {
                            int processId = -1;
                            IntPtr windowHandle = IntPtr.Zero;
                            bool processUserRun = false;

                            foreach (IntPtr threadWindowHandle in EnumThreadWindows(threadProcess.Id))
                            {
                                try
                                {
                                    //Get the current class name
                                    string classNameString = GetClassNameFromWindowHandle(threadWindowHandle);

                                    //Get information from frame window
                                    if (classNameString == "ApplicationFrameWindow")
                                    {
                                        //Get window handle
                                        windowHandle = threadWindowHandle;

                                        //Get process id
                                        IntPtr threadWindowHandleEx = FindWindowEx(threadWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                                        if (threadWindowHandleEx != IntPtr.Zero)
                                        {
                                            GetWindowThreadProcessId(threadWindowHandleEx, out processId);
                                        }
                                    }

                                    //Check if user started uwp application
                                    if (classNameString == "MSCTFIME UI")
                                    {
                                        processUserRun = true;
                                    }
                                }
                                catch { }
                            }

                            //Check if user started uwp application
                            if (processUserRun)
                            {
                                string processExecutablePath = GetAppUserModelIdFromWindowHandle(windowHandle);
                                if (targetAppUserModelId == processExecutablePath)
                                {
                                    ProcessMulti processMultiNew = new ProcessMulti();
                                    processMultiNew.Type = ProcessType.UWP;
                                    processMultiNew.Identifier = processId;
                                    processMultiNew.WindowHandle = windowHandle;
                                    processList.Add(processMultiNew);
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return processList;
        }

        //Get uwp application package from AppUserModelId
        public static Package UwpGetAppPackageFromAppUserModelId(string appUserModelId)
        {
            try
            {
                //Extract the family name from AppUserModelId
                string appFamilyName = appUserModelId.Split('!')[0];

                //Get the application path from application package
                PackageManager deployPackageManager = new PackageManager();
                string currentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                return deployPackageManager.FindPackagesForUser(currentUserIdentity, appFamilyName).FirstOrDefault();
            }
            catch { }
            return null;
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
                string manifestPath = appxDetails.InstallPath + "\\AppXManifest.xml";
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
                    appxManifestApplication.GetStringValue("Wide310x310Logo", out appxDetails.Wide310x310Logo);

                    //Check the largest available square logo
                    if (!string.IsNullOrWhiteSpace(appxDetails.Square310x310Logo))
                    {
                        appxDetails.SquareLargestLogoPath = appxDetails.InstallPath + "\\" + appxDetails.Square310x310Logo;
                    }
                    else if (!string.IsNullOrWhiteSpace(appxDetails.Square150x150Logo))
                    {
                        appxDetails.SquareLargestLogoPath = appxDetails.InstallPath + "\\" + appxDetails.Square150x150Logo;
                    }
                    else if (!string.IsNullOrWhiteSpace(appxDetails.Square70x70Logo))
                    {
                        appxDetails.SquareLargestLogoPath = appxDetails.InstallPath + "\\" + appxDetails.Square70x70Logo;
                    }
                    else if (!string.IsNullOrWhiteSpace(appxDetails.Square30x30Logo))
                    {
                        appxDetails.SquareLargestLogoPath = appxDetails.InstallPath + "\\" + appxDetails.Square30x30Logo;
                    }
                    appxDetails.SquareLargestLogoPath = UwpGetImageSizePath(appxDetails.SquareLargestLogoPath);

                    //Check the largest available wide logo
                    if (!string.IsNullOrWhiteSpace(appxDetails.Wide310x310Logo))
                    {
                        appxDetails.WideLargestLogoPath = appxDetails.InstallPath + "\\" + appxDetails.Wide310x310Logo;
                    }
                    appxDetails.WideLargestLogoPath = UwpGetImageSizePath(appxDetails.WideLargestLogoPath);
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

        //Get msi uwp resources string from package
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
                    return imageFilesContrastNone.LastOrDefault();
                }
                else if (imageFilesContrastBlack.Count() > 0)
                {
                    return imageFilesContrastBlack.LastOrDefault();
                }
                else if (imageFilesAllSizes.Count() > 0)
                {
                    return imageFilesAllSizes.LastOrDefault();
                }
                else
                {
                    return imagePath;
                }
            }
            catch { }
            return string.Empty;
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