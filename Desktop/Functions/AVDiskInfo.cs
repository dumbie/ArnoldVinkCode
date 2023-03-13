using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVDiskInfo
    {
        //Imports
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern DriveTypes GetDriveType(string lpRootPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private extern static bool GetVolumeInformation(string lpRootPathName, StringBuilder lpVolumeNameBuffer, int nVolumeNameSize, out uint lpVolumeSerialNumber, out uint lpMaximumComponentLength, out uint lpFileSystemFlags, StringBuilder lpFileSystemNameBuffer, int nFileSystemNameSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        //Enums
        public enum DriveTypes : uint
        {
            Unknown,
            NoRootDirectory,
            Removable,
            Fixed,
            Network,
            CDRom,
            Ram
        }

        //Classes
        public class DiskInfo
        {
            public bool Available { get; set; } = false;
            public string Path { get; set; } = "Unknown";
            public DriveTypes Type { get; set; } = DriveTypes.Unknown;
            public string Label { get; set; } = "Unknown";
            public string FileSystem { get; set; } = "Unknown";
            public ulong SizeDisk { get; set; } = 0;
            public ulong SizeFree { get; set; } = 0;

            public string SizeString
            {
                get
                {
                    try
                    {
                        if (SizeDisk != 0 && SizeFree != 0)
                        {
                            string sizeDiskString = AVFunctions.ConvertBytesSizeToString(SizeDisk);
                            string sizeFreeString = AVFunctions.ConvertBytesSizeToString(SizeFree);
                            return sizeFreeString + "/" + sizeDiskString;
                        }
                        else if (SizeDisk != 0)
                        {
                            return AVFunctions.ConvertBytesSizeToString(SizeDisk);
                        }
                    }
                    catch { }
                    return "Not available";
                }
            }

            public void DebugPrint()
            {
                try
                {
                    Debug.WriteLine("Available: " + Available);
                    Debug.WriteLine("Path: " + Path);
                    Debug.WriteLine("Type: " + Type);
                    Debug.WriteLine("Label: " + Label);
                    Debug.WriteLine("FileSystem: " + FileSystem);
                    Debug.WriteLine("SizeDisk: " + SizeDisk);
                    Debug.WriteLine("SizeFree: " + SizeFree);
                    Debug.WriteLine("SizeString: " + SizeString);
                }
                catch { }
            }
        }

        //Get and return disk information
        public static async Task<DiskInfo> GetDiskInfo(string diskPath)
        {
            DiskInfo diskInfo = new DiskInfo();
            try
            {
                //Check disk path
                if (string.IsNullOrWhiteSpace(diskPath))
                {
                    Debug.WriteLine("Failed to get disk information: disk path is empty.");
                    return diskInfo;
                }

                //Disk path cleanup
                diskPath = diskPath.Replace("/", "\\");
                diskPath = AVFunctions.StringReplaceMulti(diskPath, "\\\\", "\\");
                diskPath = AVFunctions.StringRemoveEnd(diskPath, "\\");
                if (diskPath.Length == 1 && char.IsLetter(diskPath[0]))
                {
                    diskPath += ":";
                }
                if (diskPath.Length == 2 && diskPath[1] == ':')
                {
                    diskPath += "\\";
                }

                //Get disk information
                diskInfo.Path = diskPath;
                diskInfo.Available = await CheckDiskAvailable(diskPath);
                diskInfo.Type = GetDriveType(diskPath);
                if (diskInfo.Available)
                {
                    //Debug.WriteLine("Disk is available reading information:");

                    //Get disk space
                    GetDiskFreeSpaceEx(diskPath, out _, out ulong sizeDisk, out ulong sizeFree);
                    diskInfo.SizeDisk = sizeDisk;
                    diskInfo.SizeFree = sizeFree;

                    //Get disk information
                    uint serialNumber;
                    uint maximumLength;
                    uint volumeFlags;
                    StringBuilder volumeNameStringBuilder = new StringBuilder(261);
                    StringBuilder fileSystemNameStringBuilder = new StringBuilder(261);
                    GetVolumeInformation(diskPath, volumeNameStringBuilder, volumeNameStringBuilder.Capacity, out serialNumber, out maximumLength, out volumeFlags, fileSystemNameStringBuilder, fileSystemNameStringBuilder.Capacity);
                    if (volumeNameStringBuilder != null && volumeNameStringBuilder.Length > 0)
                    {
                        diskInfo.Label = volumeNameStringBuilder.ToString();
                    }
                    if (fileSystemNameStringBuilder != null && fileSystemNameStringBuilder.Length > 0)
                    {
                        diskInfo.FileSystem = fileSystemNameStringBuilder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get disk information: " + ex.Message);
            }
            return diskInfo;
        }

        //Check if disk is available by checking if it responds in 100ms
        private static async Task<bool> CheckDiskAvailable(string diskPath)
        {
            try
            {
                void TaskAction()
                {
                    GetDiskFreeSpaceEx(diskPath, out _, out _, out _);
                }
                return await AVActions.TaskStartTimeout(TaskAction, 100);
            }
            catch { }
            return false;
        }
    }
}