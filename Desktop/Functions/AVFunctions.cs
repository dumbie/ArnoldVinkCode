using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace ArnoldVinkCode
{
    public partial class AVFunctions
    {
        //Convert string To Title Case
        public static string ToTitleCase(string ToTitleCase)
        {
            char[] TitleCase = ToTitleCase.ToLower().ToCharArray();
            for (int i = 0; i < TitleCase.Count(); i++) { TitleCase[i] = i == 0 || TitleCase[i - 1] == ' ' ? char.ToUpper(TitleCase[i]) : TitleCase[i]; }
            return new string(TitleCase);
        }

        //Replace first text occurence in string
        public static string StringReplaceFirst(string stringText, string SearchFor, string ReplaceWith, bool StartsWith)
        {
            if (StartsWith && !stringText.ToLower().StartsWith(SearchFor.ToLower())) { return stringText; }
            int Position = stringText.IndexOf(SearchFor, StringComparison.CurrentCultureIgnoreCase);
            if (Position < 0) { return stringText; }
            return stringText.Substring(0, Position) + ReplaceWith + stringText.Substring(Position + SearchFor.Length);
        }

        //Remove starting text occurence in string
        public static string StringRemoveStart(string stringText, string toRemove)
        {
            try
            {
                while (stringText.StartsWith(toRemove))
                {
                    stringText = stringText.Substring(toRemove.Length);
                }
            }
            catch { }
            return stringText;
        }

        //Remove multiple starting text occurence in string
        public static string StringRemoveMultiStart(string stringText, string[] toRemove)
        {
            try
            {
                while (toRemove.Any(stringText.StartsWith))
                {
                    foreach (string Remove in toRemove)
                    {
                        if (stringText.StartsWith(Remove)) { stringText = stringText.Substring(Remove.Length); }
                    }
                }
            }
            catch { }
            return stringText;
        }

        //Remove ending text occurence in string
        public static string StringRemoveEnd(string stringText, string toRemove)
        {
            try
            {
                while (stringText.EndsWith(toRemove))
                {
                    stringText = stringText.Substring(0, stringText.Length - toRemove.Length);
                }
            }
            catch { }
            return stringText;
        }

        //Remove multiple ending text occurence in string
        public static string StringRemoveMultiEnd(string stringText, string[] toRemove)
        {
            try
            {
                while (toRemove.Any(stringText.EndsWith))
                {
                    foreach (string Remove in toRemove)
                    {
                        if (stringText.EndsWith(Remove)) { stringText = stringText.Substring(0, stringText.Length - Remove.Length); }
                    }
                }
            }
            catch { }
            return stringText;
        }

        //Replace last text occurence in string
        public static string StringReplaceLast(string stringText, string ReplaceWith)
        {
            try
            {
                return stringText.Remove(stringText.Length - 1, 1) + ReplaceWith;
            }
            catch { return stringText; }
        }

        //Add string to string with character
        public static string StringAdd(string Oldstring, string Addstring, string Character)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Oldstring)) { Oldstring = Oldstring + Character + " " + Addstring; }
                else { Oldstring = Addstring; }
                return Oldstring;
            }
            catch { return Oldstring; }
        }

        //Remove text after certain character
        public static string StringRemoveAfter(string stringText, string RemoveCharacter, int RemoveAfter)
        {
            try
            {
                stringText = stringText.Substring(0, stringText.IndexOf(RemoveCharacter) + RemoveAfter);
            }
            catch { }
            return stringText;
        }

        //Convert string To Cutted string
        public static string StringCut(string Cutstring, int CutAt, string Addstring)
        {
            if (Cutstring.Length > CutAt) { return Cutstring.Substring(0, CutAt) + Addstring; }
            else { return Cutstring; }
        }

        //Get text after certain word in string
        public static string StringShowAfter(string stringText, string SearchTerm, int ReturnLength)
        {
            int posA = stringText.LastIndexOf(SearchTerm);
            if (posA == -1) { return string.Empty; }

            int adjustedPosA = posA + SearchTerm.Length;
            if (adjustedPosA >= stringText.Length) { return string.Empty; }

            if (ReturnLength == 0) { return stringText.Substring(adjustedPosA); }
            else { return stringText.Substring(adjustedPosA, ReturnLength); }
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
            //Not available on desktop
            return false;
        }

        //Check device os version
        public static int DevOsVersion() { return Environment.OSVersion.Version.Major; }

        //Get network connection type
        public static string GetNetworkType()
        {
            try
            {
                //Not available on desktop
                return "Unknown";
            }
            catch { return "NoConnection"; }
        }

        //Get network connection name
        public static string GetNetworkName()
        {
            try
            {
                //Not available on desktop
                return "Unknown";
            }
            catch { return "NoConnection"; }
        }

        //Check if datetime is between dates
        public static bool BetweenTime(DateTime NowTime, DateTime StartTime, DateTime EndTime, bool Inclusive)
        {
            if (Inclusive) { return (NowTime >= StartTime) && (NowTime <= EndTime); }
            else { return (NowTime > StartTime) && (NowTime < EndTime); }
        }

        //Check if number is between numbers
        public static bool BetweenNumbers(int NowNumber, int StartNumber, int EndNumber, bool Inclusive)
        {
            if (Inclusive) { return (NowNumber >= StartNumber) && (NowNumber <= EndNumber); }
            else { return (NowNumber > StartNumber) && (NowNumber < EndNumber); }
        }

        //Reset dispatch timer tick estimate
        public static void TimerReset(DispatcherTimer dispatchTimer)
        {
            try
            {
                dispatchTimer.Stop();
                dispatchTimer.Start();
            }
            catch { }
        }

        //Renew dispatch timer
        public static void TimerRenew(ref DispatcherTimer dispatchTimer)
        {
            try
            {
                if (dispatchTimer != null) { dispatchTimer.Stop(); }
                dispatchTimer = new DispatcherTimer();
            }
            catch { }
        }

        //Find the visual child of object
        public static T FindVisualChild<T>(DependencyObject objChild) where T : DependencyObject
        {
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(objChild); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(objChild, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    else
                    {
                        T subChild = FindVisualChild<T>(child);
                        if (subChild != null) { return subChild; }
                    }
                }
            }
            catch { }
            return null;
        }

        //Find the visual parent of object
        public static T FindVisualParent<T>(DependencyObject objChild) where T : DependencyObject
        {
            try
            {
                DependencyObject parentObject = VisualTreeHelper.GetParent(objChild);
                if (parentObject == null) { return null; }

                //Check if parent matches the type
                T parent = parentObject as T;
                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return FindVisualParent<T>(parentObject);
                }
            }
            catch { }
            return null;
        }

        //Check if an actual ListBoxItem is clicked
        public static bool ListBoxItemClickCheck(DependencyObject dependencyObject)
        {
            try
            {
                if (FindVisualParent<ListBoxItem>(dependencyObject) == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch { return false; }
        }

        //Get application form window
        public static T GetWindowOfType<T>() where T : Window
        {
            try
            {
                return System.Windows.Application.Current.Windows.OfType<T>().FirstOrDefault();
            }
            catch { return null; }
        }

        //Get screen aspect ratio
        public static string ScreenAspectRatio(int ScreenWidth, int ScreenHeight, bool AutoResolution)
        {
            try
            {
                if (AutoResolution)
                {
                    ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
                    ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
                }

                int Remainder = 0;
                int Greatest = ScreenWidth;
                int Common = ScreenHeight;
                while (Common != 0)
                {
                    Remainder = Greatest % Common;
                    Greatest = Common;
                    Common = Remainder;
                }

                if (Greatest != 0) { return ScreenWidth / Greatest + ":" + ScreenHeight / Greatest; }
                else { return string.Empty; }
            }
            catch { return string.Empty; }
        }

        //Move byte in a byte array
        public static void MoveByteInArray(byte[] SerialBytes, int MoveIndex, int NewIndex)
        {
            try
            {
                byte MoveValue = SerialBytes[MoveIndex];
                int CalcNewIndex = NewIndex - 1;

                Array.Copy(SerialBytes, MoveIndex + 1, SerialBytes, MoveIndex, SerialBytes.Length - MoveIndex - 1);
                SerialBytes[CalcNewIndex] = MoveValue;
            }
            catch { }
        }

        //Convert bytes size to display string
        public static string ConvertBytesSizeToString(float bytesRaw)
        {
            try
            {
                int bytesCounter = 0;
                while (bytesRaw / 1024 >= 1)
                {
                    try
                    {
                        bytesRaw = bytesRaw / 1024;
                        bytesCounter++;
                    }
                    catch { }
                }
                string[] formatBytesSuffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
                return bytesRaw.ToString("0.00") + formatBytesSuffix[bytesCounter];
            }
            catch { }
            return "Unknown";
        }

        //Get screen by number
        public static Screen GetScreenByNumber(int monitorNumber, out bool monitorSuccess)
        {
            try
            {
                Screen targetScreen = Screen.AllScreens[monitorNumber];
                monitorSuccess = true;
                return targetScreen;
            }
            catch
            {
                Screen targetScreen = Screen.PrimaryScreen;
                monitorSuccess = false;
                return targetScreen;
            }
        }
    }
}