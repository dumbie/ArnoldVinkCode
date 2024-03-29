﻿namespace ArnoldVinkCode
{
    public partial class AVUwpAppx
    {
        public enum AppxDeviceFamily
        {
            Universal = 0,
            Desktop = 1
        }

        public class AppxDetails
        {
            public string AppIdentifier { get; set; } = string.Empty;
            public AppxDeviceFamily AppDeviceFamily { get; set; } = AppxDeviceFamily.Universal;
            public string AppUserModelId { get; set; } = string.Empty;
            public string FamilyName { get; set; } = string.Empty;
            public string FullPackageName { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public string InstallPath { get; set; } = string.Empty;
            public string ExecutableName { get; set; } = string.Empty;
            public string ExecutableAliasName { get; set; } = string.Empty;
            public string SquareLargestLogoPath { get; set; } = string.Empty;
            public string WideLargestLogoPath { get; set; } = string.Empty;
        }
    }
}