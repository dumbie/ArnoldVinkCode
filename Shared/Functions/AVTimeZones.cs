using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public class AVTimeZones
    {
        [DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
        static extern int EnumDynamicTimeZoneInformation(int dwIndex, out DynamicTimeZoneInformation lpTimeZoneInformation);
        [DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
        static extern bool SystemTimeToTzSpecificLocalTime(ref TimeZoneInformation lpTimeZone, ref SystemTime lpUniversalTime, out SystemTime lpLocalTime);
        [DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
        static extern bool GetTimeZoneInformationForYear(short wYear, ref DynamicTimeZoneInformation pdtzi, out TimeZoneInformation ptzi);

        public static DateTime GetTimeZoneTime(string TimeZone)
        {
            try
            {
                SystemTime SystemTimeLocal;
                TimeZoneInformation TimeZoneInformation;
                SystemTime SystemTimeNow = new SystemTime(DateTime.UtcNow);
                DynamicTimeZoneInformation DynamicTimeZoneInformation = AllEnumerateSystemTimeZones.First(x => x.TimeZoneKeyName == TimeZone);
                if (GetTimeZoneInformationForYear(SystemTimeNow.Year, ref DynamicTimeZoneInformation, out TimeZoneInformation))
                {
                    if (SystemTimeToTzSpecificLocalTime(ref TimeZoneInformation, ref SystemTimeNow, out SystemTimeLocal))
                    { return new DateTime(SystemTimeLocal.Year, SystemTimeLocal.Month, SystemTimeLocal.Day, SystemTimeLocal.Hour, SystemTimeLocal.Minute, 0, 0, DateTimeKind.Utc); }
                }
            }
            catch { }
            return new DateTime();
        }

        private static IReadOnlyList<DynamicTimeZoneInformation> AllEnumerateSystemTimeZones = EnumerateSystemTimeZones().ToList();
        private static IEnumerable<DynamicTimeZoneInformation> EnumerateSystemTimeZones()
        {
            int i = 0;
            DynamicTimeZoneInformation DynamicTimeZoneInformation;
            while (true)
            {
                if (EnumDynamicTimeZoneInformation(i++, out DynamicTimeZoneInformation) != 0) { yield break; }
                yield return DynamicTimeZoneInformation;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SystemTime
        {
            [MarshalAs(UnmanagedType.U2)]
            public short Year;
            [MarshalAs(UnmanagedType.U2)]
            public short Month;
            [MarshalAs(UnmanagedType.U2)]
            public short DayOfWeek;
            [MarshalAs(UnmanagedType.U2)]
            public short Day;
            [MarshalAs(UnmanagedType.U2)]
            public short Hour;
            [MarshalAs(UnmanagedType.U2)]
            public short Minute;
            [MarshalAs(UnmanagedType.U2)]
            public short Second;
            [MarshalAs(UnmanagedType.U2)]
            public short Millisecond;
            public SystemTime(DateTime DateTime)
            {
                Year = (short)DateTime.Year;
                Month = (short)DateTime.Month;
                DayOfWeek = (short)DateTime.DayOfWeek;
                Day = (short)DateTime.Day;
                Hour = (short)DateTime.Hour;
                Minute = (short)DateTime.Minute;
                Second = (short)DateTime.Second;
                Millisecond = (short)DateTime.Millisecond;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct TimeZoneInformation
        {
            [MarshalAs(UnmanagedType.I4)]
            public Int32 Bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string StandardName;
            public SystemTime StandardDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 StandardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DaylightName;
            public SystemTime DaylightDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 DaylightBias;
            public TimeZoneInformation(DynamicTimeZoneInformation DynamicTimeZoneInformation)
            {
                Bias = DynamicTimeZoneInformation.Bias;
                StandardName = DynamicTimeZoneInformation.StandardName;
                StandardDate = DynamicTimeZoneInformation.StandardDate;
                StandardBias = DynamicTimeZoneInformation.StandardBias;
                DaylightName = DynamicTimeZoneInformation.DaylightName;
                DaylightDate = DynamicTimeZoneInformation.DaylightDate;
                DaylightBias = DynamicTimeZoneInformation.DaylightBias;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DynamicTimeZoneInformation
        {
            [MarshalAs(UnmanagedType.I4)]
            public Int32 Bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string StandardName;
            public SystemTime StandardDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 StandardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DaylightName;
            public SystemTime DaylightDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 DaylightBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string TimeZoneKeyName;
            [MarshalAs(UnmanagedType.Bool)]
            public bool DynamicDaylightTimeDisabled;
        }
    }
}