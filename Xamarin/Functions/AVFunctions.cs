using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace ArnoldVinkCode
{
    class AVFunctions
    {
        //Convert String To Title Case
        public static string ToTitleCase(string ToTitleCase)
        {
            char[] TitleCase = ToTitleCase.ToLower().ToCharArray();
            for (int i = 0; i < TitleCase.Count(); i++) { TitleCase[i] = i == 0 || TitleCase[i - 1] == ' ' ? char.ToUpper(TitleCase[i]) : TitleCase[i]; }
            return new string(TitleCase);
        }

        //Replace first text occurence in string
        public static string StringReplaceFirst(string StringText, string SearchFor, string ReplaceWith, bool StartsWith)
        {
            if (StartsWith) { if (!StringText.ToLower().StartsWith(SearchFor.ToLower())) { return StringText; } }
            int Position = StringText.IndexOf(SearchFor, StringComparison.CurrentCultureIgnoreCase);
            if (Position < 0) { return StringText; }
            return StringText.Substring(0, Position) + ReplaceWith + StringText.Substring(Position + SearchFor.Length);
        }

        //Remove starting text occurence in string
        public static string StringRemoveStart(string String, string toRemove)
        {
            try
            {
                while (String.StartsWith(toRemove))
                {
                    String = String.Substring(toRemove.Length);
                }
            }
            catch { }
            return String;
        }

        //Remove multiple starting text occurence in string
        public static string StringRemoveMultiStart(string String, string[] toRemove)
        {
            try
            {
                while (toRemove.Any(String.StartsWith))
                {
                    foreach (string Remove in toRemove)
                    {
                        if (String.StartsWith(Remove)) { String = String.Substring(Remove.Length); }
                    }
                }
            }
            catch { }
            return String;
        }

        //Remove ending text occurence in string
        public static string StringRemoveEnd(string String, string toRemove)
        {
            try
            {
                while (String.EndsWith(toRemove))
                {
                    String = String.Substring(0, String.Length - toRemove.Length);
                }
            }
            catch { }
            return String;
        }

        //Remove multiple ending text occurence in string
        public static string StringRemoveMultiEnd(string String, string[] toRemove)
        {
            try
            {
                while (toRemove.Any(String.EndsWith))
                {
                    foreach (string Remove in toRemove)
                    {
                        if (String.EndsWith(Remove)) { String = String.Substring(0, String.Length - Remove.Length); }
                    }
                }
            }
            catch { }
            return String;
        }

        //Replace last text occurence in string
        public static string StringReplaceLast(string String, string ReplaceWith)
        {
            try
            {
                return String.Remove(String.Length - 1, 1) + ReplaceWith;
            }
            catch { return String; }
        }

        //Add string to string with character
        public static string StringAdd(string OldString, string AddString, string Character)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(OldString)) { OldString = OldString + Character + " " + AddString; }
                else { OldString = AddString; }
                return OldString;
            }
            catch { return OldString; }
        }

        //Remove text after certain character
        public static string StringRemoveAfter(string String, string RemoveCharacter, int RemoveAfter)
        {
            try
            {
                String = String.Substring(0, String.IndexOf(RemoveCharacter) + RemoveAfter);
            }
            catch { }
            return String;
        }

        //Convert String To Cutted String
        public static string StringCut(string CutString, int CutAt, string AddString)
        {
            if (CutString.Length > CutAt) { return CutString.Substring(0, CutAt) + AddString; }
            else { return CutString; }
        }

        //Convert Number To Text
        public static string NumberToText(string StrNumber)
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
        public static bool DevMobile()
        {
            try
            {
                if (Device.Idiom == TargetIdiom.Phone) { return true; }
                else { return false; }
            }
            catch { return false; }
        }

        //Get device os type
        public static string GetOsType()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS) { return "iOS"; }
                else if (Device.RuntimePlatform == Device.Android) { return "Android"; }
                else if (Device.RuntimePlatform == Device.UWP) { return "Windows"; }
                else { return "Unknown"; }
            }
            catch { return "NoConnection"; }
        }

        //Get network connection type
        public static string GetNetworkType()
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
        public static async Task<string> GetNetworkName()
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
        public static bool TimeBetween(DateTime NowTime, DateTime StartTime, DateTime EndTime, bool Inclusive)
        {
            if (Inclusive) { return (NowTime >= StartTime) && (NowTime <= EndTime); }
            else { return (NowTime > StartTime) && (NowTime < EndTime); }
        }

        //Reset a timer tick estimate
        public static void ResetTimer(Timer ResetTimer)
        {
            try
            {
                ResetTimer.Stop();
                ResetTimer.Start();
            }
            catch { }
        }

        //Find all visual children of object
        public static List<T> FindVisualChildren<T>(Element searchElement)
        {
            List<T> resultList = new List<T>();
            try
            {
                //Get properties
                IEnumerable<PropertyInfo> propertyInfo = searchElement.GetType().GetRuntimeProperties();

                //Check children
                PropertyInfo contProp = propertyInfo.Where(x => x.Name == "Content" || x.Name == "Children" || x.Name == "TemplatedItems").FirstOrDefault();
                if (contProp != null)
                {
                    IEnumerable children = contProp.GetValue(searchElement) as IEnumerable;
                    foreach (object child in children)
                    {
                        if (child is T)
                        {
                            resultList.Add((T)child);
                        }
                        else if (child is Element)
                        {
                            FindVisualChildren<T>((Element)child);
                        }
                    }
                }
            }
            catch { }
            return resultList;
        }

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
        public static string DegreesToCardinal(double degrees)
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