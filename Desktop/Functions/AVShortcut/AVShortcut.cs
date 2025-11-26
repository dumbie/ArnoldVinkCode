using Shell32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVProcess;

namespace ArnoldVinkCode
{
    public partial class AVShortcut
    {
        //Get details from a shortcut file
        public static ShortcutDetails ReadShortcutFile(string shortcutPath)
        {
            ShortcutDetails shortcutDetails = new ShortcutDetails();
            try
            {
                Thread thread = new Thread(delegate ()
                {
                    try
                    {
                        string folderString = Path.GetDirectoryName(shortcutPath);
                        string filenameString = Path.GetFileName(shortcutPath);
                        //Debug.WriteLine("Reading shortcut: " + shortcutPath);

                        Shell shell = new Shell();
                        Folder folder = shell.NameSpace(folderString);
                        FolderItem folderItem = folder.ParseName(filenameString);
                        ShellLinkObject shellLinkObject = folderItem.GetLink;

                        int iconIndex = 0;
                        string iconPath = string.Empty;
                        try
                        {
                            iconIndex = shellLinkObject.GetIconLocation(out iconPath);
                            iconPath = iconPath.Replace("file:///", string.Empty);
                            iconPath = WebUtility.UrlDecode(iconPath);
                        }
                        catch { }

                        string argumentString = string.Empty;
                        try
                        {
                            argumentString = shellLinkObject.Arguments;
                        }
                        catch { }

                        string targetPath = string.Empty;
                        try
                        {
                            targetPath = shellLinkObject.Path;
                        }
                        catch { }
                        try
                        {
                            if (string.IsNullOrWhiteSpace(targetPath))
                            {
                                targetPath = shellLinkObject.Target.Path;
                            }
                        }
                        catch { }

                        string workingPath = string.Empty;
                        try
                        {
                            workingPath = shellLinkObject.WorkingDirectory;
                        }
                        catch { }

                        string commentString = string.Empty;
                        try
                        {
                            commentString = shellLinkObject.Description;
                        }
                        catch { }

                        //Expand environment variables
                        targetPath = ConvertEnvironmentPath(targetPath);
                        workingPath = ConvertEnvironmentPath(workingPath);
                        iconPath = ConvertEnvironmentPath(iconPath);
                        shortcutPath = ConvertEnvironmentPath(shortcutPath);

                        //Check shortcut type
                        if (Check_PathUrlProtocol(targetPath))
                        {
                            shortcutDetails.Type = ShortcutType.UrlProtocol;
                        }
                        else if (Check_PathUwpApplication(targetPath))
                        {
                            shortcutDetails.Type = ShortcutType.UWP;
                        }
                        else
                        {
                            shortcutDetails.NameExe = Path.GetFileName(targetPath);
                        }

                        //Set shortcut details
                        shortcutDetails.Title = StripShortcutFilename(Path.GetFileNameWithoutExtension(shortcutPath));
                        shortcutDetails.TargetPath = targetPath;
                        shortcutDetails.WorkingPath = workingPath;
                        shortcutDetails.IconIndex = iconIndex;
                        shortcutDetails.IconPath = iconPath;
                        shortcutDetails.ShortcutPath = shortcutPath;
                        shortcutDetails.Argument = argumentString;
                        shortcutDetails.Comment = commentString;
                        shortcutDetails.DateModified = folderItem.ModifyDate;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed reading shortcut: " + ex.Message + "/" + shortcutPath);
                    }
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
            catch { }
            return shortcutDetails;
        }

        //Strip shortcut file name
        public static string StripShortcutFilename(string shortcutFilename)
        {
            try
            {
                return shortcutFilename.Replace(".lnk", string.Empty).Replace(".url", string.Empty).Replace(".exe - Shortcut", string.Empty).Replace(" - Shortcut", string.Empty);
            }
            catch { }
            return shortcutFilename;
        }
    }
}