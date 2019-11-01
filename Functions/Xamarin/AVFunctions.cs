using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArnoldVinkCode
{
    class AVFunctions
    {
        //Convert String To Title Case
        internal static string ToTitleCase(string ToTitleCase)
        {
            char[] TitleCase = ToTitleCase.ToLower().ToCharArray();
            for (int i = 0; i < TitleCase.Count(); i++) { TitleCase[i] = i == 0 || TitleCase[i - 1] == ' ' ? char.ToUpper(TitleCase[i]) : TitleCase[i]; }
            return new string(TitleCase);
        }

        //Replace first text occurence in string
        internal static string StringReplaceFirst(string StringText, string SearchFor, string ReplaceWith, bool StartsWith)
        {
            if (StartsWith) { if (!StringText.ToLower().StartsWith(SearchFor.ToLower())) { return StringText; } }
            int Position = StringText.IndexOf(SearchFor, StringComparison.CurrentCultureIgnoreCase);
            if (Position < 0) { return StringText; }
            return StringText.Substring(0, Position) + ReplaceWith + StringText.Substring(Position + SearchFor.Length);
        }

        //Replace last text occurence in string
        internal static string StringReplaceLast(string String, string ReplaceWith) { try { return String.Remove(String.Length - 1, 1) + ReplaceWith; } catch { return ""; } }

        //Add string to string with character
        internal static string StringAdd(string OldString, string AddString, string Character)
        {
            try
            {
                if (!String.IsNullOrEmpty(OldString)) { OldString = OldString + Character + " " + AddString; }
                else { OldString = AddString; }
                return OldString;
            }
            catch { return ""; }
        }

        //Convert String To Cutted String
        internal static string CutString(string CutString, int CutAt, string AddString)
        {
            if (CutString.Length > CutAt) { return CutString.Substring(0, CutAt) + AddString; }
            else { return CutString; }
        }

        //Convert Number To Text
        internal static string NumberToText(string StrNumber)
        {
            try
            {
                string[] ones = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
                string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                string[] tens = { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                int IntNumber = Convert.ToInt32(StrNumber);
                if (IntNumber < 10) { StrNumber = ones[IntNumber]; }
                else if (IntNumber < 20) { StrNumber = teens[IntNumber - 10]; }
                else if (IntNumber < 100)
                {
                    if (IntNumber % 10 != 0) { StrNumber = tens[IntNumber / 10 - 2] + "-" + ones[IntNumber % 10]; }
                    else { StrNumber = tens[IntNumber / 10 - 2]; }
                }

                return StrNumber;
            }
            catch { return "Unknown"; }
        }

        //Check if device is mobile
        internal static bool DevMobile()
        {
            try
            {
                if(Device.Idiom == TargetIdiom.Phone) { return true; }
                else { return false; }
            }
            catch { return false; }
        }

        //Get device os type
        internal static string GetOsType()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS) { return "iOS"; }
                else if (Device.RuntimePlatform == Device.Android) { return "Android"; }
                else if (Device.RuntimePlatform == Device.UWP) { return "Windows"; }
                else{ return "Unknown";}
            }
            catch { return "NoConnection"; }
        }

        //Get network connection type
        internal static string GetNetworkType()
        {
            try
            {
                //ConnectionProfile ConnectionProfile = System.Net.NetworkInformation.GetInternetConnectionProfile();
                //if (ConnectionProfile == null) { return "NoConnection"; }
                //else if (ConnectionProfile.IsWlanConnectionProfile) { return "Wireless"; }
                //else if (ConnectionProfile.IsWwanConnectionProfile) { return "Mobile"; }
                //else { return "Wired"; }
                //Fix return network type
                return "Unknown";
            }
            catch { return "NoConnection"; }
        }

        //Get network connection name
        internal static async Task<string> GetNetworkName()
        {
            try
            {
                ////Check connection
                //ConnectionProfile ConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                //if (ConnectionProfile == null) { return "No connection"; }
                //else
                //{
                //    //Get Wi-Fi / Ethernet name
                //    string FirstNetwork = ConnectionProfile.GetNetworkNames().First();
                //    if (!String.IsNullOrEmpty(FirstNetwork)) { return FirstNetwork; }

                //    //Get Cellular name
                //    if (ConnectionProfile.IsWwanConnectionProfile)
                //    {
                //        PhoneCallStore phoneCallStore = await PhoneCallManager.RequestStoreAsync();
                //        PhoneLine phoneLine = await PhoneLine.FromIdAsync(await phoneCallStore.GetDefaultLineAsync());
                //        return phoneLine.NetworkName;
                //    }

                    return "Unknown";
                //}
            }
            catch { return "Unknown"; }
        }

        //Check if datetime is between dates
        internal static bool TimeBetween(DateTime NowTime, DateTime StartTime, DateTime EndTime, bool Inclusive)
        {
            if (Inclusive) { return (NowTime >= StartTime) && (NowTime <= EndTime); }
            else { return (NowTime > StartTime) && (NowTime < EndTime); }
        }

        ////Reset a timer tick estimate
        //internal static void ResetTimer(Timer ResetTimer)
        //{
        //    try
        //    {
        //        ResetTimer.Stop();
        //        ResetTimer.Start();
        //    }
        //    catch { }
        //}

        //Scroll to certain ui element
        public static void ScrollViewToElement(ScrollView ScrollViewer, Element UIElement, bool VertScrolling, bool InstantScroll)
        {
            try
            {
                ////GeneralTransform GenTransform = UIElement.TransformToVisual((UIElement)ScrollViewer.Content);
                ////Point TransPoint = GenTransform.TransformPoint(new Point(0, 0));

                //if (VertScrolling) { ScrollViewer.(null, TransPoint.Y, null, InstantScroll); }
                //else { ScrollViewer.ChangeView(TransPoint.X, null, null, InstantScroll); }
                //ScrollViewer.UpdateLayout();
            }
            catch { }
        }

        //Convert Degrees to Cardinal string
        internal static string DegreesToCardinal(double degrees)
        {
            try
            {
                string[] CardinalList = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
                return CardinalList[(int)Math.Round((degrees % 360) / 45)];
            }
            catch { return "N/A"; }
        }
    }
}