using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVFunctions
    {
        //Check if string contains string
        public static bool StringContains(string source, string check, bool ignoreCase)
        {
            bool containsResult = false;
            try
            {
                if (ignoreCase)
                {
                    return source.IndexOf(check, StringComparison.InvariantCultureIgnoreCase) >= 0;
                }
                else
                {
                    return source.IndexOf(check) >= 0;
                }
            }
            catch { }
            return containsResult;
        }

        //Check if string matches part of other string
        public static bool StringMatch(string source, string check, bool ignoreCase)
        {
            bool containsResult = false;
            try
            {
                if (ignoreCase)
                {
                    containsResult = source.IndexOf(check, StringComparison.InvariantCultureIgnoreCase) >= 0;
                    if (!containsResult)
                    {
                        containsResult = check.IndexOf(source, StringComparison.InvariantCultureIgnoreCase) >= 0;
                    }
                }
                else
                {
                    containsResult = source.IndexOf(check) >= 0;
                    if (!containsResult)
                    {
                        containsResult = check.IndexOf(source) >= 0;
                    }
                }
            }
            catch { }
            return containsResult;
        }

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
            int Position = stringText.IndexOf(SearchFor, StringComparison.InvariantCultureIgnoreCase);
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

        //Remove unicode characters from string
        public static string StringRemoveUnicode(string stringText)
        {
            try
            {
                stringText = Regex.Replace(stringText, @"[^\u0020-\u007E]", string.Empty);
            }
            catch { }
            return stringText;
        }

        //Remove text after certain character
        public static string StringRemoveAfter(string stringText, string removeCharacter, int removeAfter)
        {
            try
            {
                int position = stringText.IndexOf(removeCharacter, StringComparison.InvariantCultureIgnoreCase);
                stringText = stringText.Substring(0, position + removeAfter);
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
        public static string StringShowAfter(string stringText, string searchTerm, int returnLength)
        {
            int posA = stringText.LastIndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase);
            if (posA == -1) { return string.Empty; }

            int adjustedPosA = posA + searchTerm.Length;
            if (adjustedPosA >= stringText.Length) { return string.Empty; }

            if (returnLength == 0) { return stringText.Substring(adjustedPosA); }
            else { return stringText.Substring(adjustedPosA, returnLength); }
        }

        //Validate if string is valid link
        public static bool StringLinkValidate(string stringLink)
        {
            try
            {
                return Uri.TryCreate(stringLink, UriKind.RelativeOrAbsolute, out Uri uriLink) && (uriLink.Scheme == Uri.UriSchemeHttp || uriLink.Scheme == Uri.UriSchemeHttps);
            }
            catch { }
            return false;
        }

        //Fixup string to valid link
        public static string StringLinkFixup(string stringLink)
        {
            try
            {
                stringLink = stringLink.Replace(",", ".");
                if (!stringLink.Contains(":"))
                {
                    stringLink = "https://" + stringLink;
                }
            }
            catch { }
            return stringLink;
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

        //Convert Seconds To Hour:Minute:Second
        public static string SecondsToHms(int secondsInt, bool useChar, bool emptyMinutes)
        {
            try
            {
                int convertedDays = secondsInt / 86400;
                int convertedHours = secondsInt % 86400 / 3600;
                int convertedMinutes = secondsInt % 3600 / 60;
                int convertedSeconds = secondsInt % 60;

                string hmsString = string.Empty;
                if (convertedDays != 0)
                {
                    hmsString += convertedDays + (useChar ? "d " : ":");
                }
                
                if (convertedHours != 0)
                {
                    hmsString += convertedHours + (useChar ? "h " : ":");
                }

                if (convertedMinutes != 0)
                {
                    if (useChar)
                    {
                        hmsString += convertedMinutes + "m ";
                    }
                    else
                    {
                        hmsString += convertedMinutes.ToString("00") + ":";
                    }
                }
                else if (convertedMinutes == 0 && !useChar && emptyMinutes)
                {
                    hmsString += "00:";
                }

                if (useChar)
                {
                    hmsString += convertedSeconds + "s";
                }
                else
                {
                    hmsString += convertedSeconds.ToString("00");
                }

                return hmsString;
            }
            catch
            {
                return secondsInt.ToString() + (useChar ? "s" : "");
            }
        }

        //Get filename without extension with space check
        public static string GetFileNameNoExtension(string path)
        {
            try
            {
                if (!Path.HasExtension(path))
                {
                    //Debug.WriteLine("File has no extension: " + Path.GetFileName(path));
                    return Path.GetFileName(path);
                }
                else if (Path.GetExtension(path).Length > 1 && Path.GetExtension(path)[1] == ' ')
                {
                    //Debug.WriteLine("File has space extension: " + Path.GetFileName(path));
                    return Path.GetFileName(path);
                }
                else
                {
                    //Debug.WriteLine("File has proper extension: " + Path.GetFileNameWithoutExtension(path));
                    return Path.GetFileNameWithoutExtension(path);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get filename no extension: " + ex.Message);
                return path;
            }
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
        public static bool BetweenNumbersOr(int NowNumber, int StartNumber, int EndNumber, bool Inclusive)
        {
            if (Inclusive) { return (NowNumber >= StartNumber) || (NowNumber <= EndNumber); }
            else { return (NowNumber > StartNumber) || (NowNumber < EndNumber); }
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

        //Find all visual children of object
        public static List<T> FindVisualChildren<T>(DependencyObject objChild) where T : DependencyObject
        {
            List<T> childrenList = new List<T>();
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(objChild); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(objChild, i);
                    if (child != null && child is T)
                    {
                        childrenList.Add((T)child);
                    }
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        childrenList.Add(childOfChild);
                    }
                }
            }
            catch { }
            return childrenList;
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

        //Application window move
        public static void WindowMove(IntPtr windowHandle, int horLeft, int verTop)
        {
            try
            {
                SetWindowPos(windowHandle, IntPtr.Zero, horLeft, verTop, 0, 0, (int)SWP_WindowFlags.NOSIZE);
            }
            catch { }
        }

        //Application window resize
        public static void WindowResize(IntPtr windowHandle, int width, int height)
        {
            try
            {
                SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, width, height, (int)SWP_WindowFlags.NOMOVE);
            }
            catch { }
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

                if (Greatest != 0)
                {
                    return ScreenWidth / Greatest + ":" + ScreenHeight / Greatest;
                }
                else
                {
                    return ScreenWidth + ":" + ScreenHeight;
                }
            }
            catch
            {
                return ScreenWidth + ":" + ScreenHeight;
            }
        }

        //Move byte in a byte array to left
        public static void MoveByteInArrayLeft(byte[] SerialBytes, int MoveIndex, int NewIndex)
        {
            try
            {
                byte MoveValue = SerialBytes[MoveIndex];
                Array.Copy(SerialBytes, MoveIndex + 1, SerialBytes, MoveIndex, SerialBytes.Length - 1 - MoveIndex);
                SerialBytes[NewIndex] = MoveValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to move byte left: " + ex.Message);
            }
        }

        public static void MoveByteInArrayLeft(int totalByteSize, byte[] serialBytes, int MoveIndex, int NewIndex)
        {
            try
            {
                byte MoveValue = serialBytes[MoveIndex];
                Array.Copy(serialBytes, MoveIndex + 1, serialBytes, MoveIndex, totalByteSize - 1 - MoveIndex);
                serialBytes[NewIndex] = MoveValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to move byte left: " + ex.Message);
            }
        }

        //Move byte in a byte array to right
        public static void MoveByteInArrayRight(byte[] SerialBytes, int MoveIndex, int NewIndex)
        {
            try
            {
                byte MoveValue = SerialBytes[MoveIndex];
                Array.Copy(SerialBytes, NewIndex, SerialBytes, NewIndex + 1, SerialBytes.Length - 1 - NewIndex);
                SerialBytes[NewIndex] = MoveValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to move byte right: " + ex.Message);
            }
        }

        public static void MoveByteInArrayRight(int totalByteSize, byte[] serialBytes, int MoveIndex, int NewIndex)
        {
            try
            {
                byte MoveValue = serialBytes[MoveIndex];
                Array.Copy(serialBytes, NewIndex, serialBytes, NewIndex + 1, totalByteSize - 1 - NewIndex);
                serialBytes[NewIndex] = MoveValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to move byte right: " + ex.Message);
            }
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

                string sizeString = bytesRaw.ToString("0.00");
                sizeString = StringRemoveEnd(sizeString, ",00");
                sizeString = StringRemoveEnd(sizeString, ".00");

                string[] formatBytesSuffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
                return sizeString + formatBytesSuffix[bytesCounter];
            }
            catch { }
            return "Unknown";
        }

        //Linear Interpolation Math
        public static float MathLerp(float aFloat, float bFloat, float amountFloat)
        {
            //return aFloat * (1.0F - amountFloat) + bFloat * amountFloat;
            return aFloat + (bFloat - aFloat) * amountFloat;
        }

        //Count decimals in number
        public static int DecimalGetLength(dynamic decimalValue, int decimalLimit)
        {
            try
            {
                string decimalConverted = decimalValue.ToString(CultureInfo.InvariantCulture);
                int decimalIndex = decimalConverted.IndexOf(".") + 1;
                if (decimalIndex == 0)
                {
                    return 0;
                }
                else
                {
                    int decimalLength = decimalConverted.Substring(decimalIndex).Length;
                    if (decimalLimit > 0 && decimalLength > decimalLimit)
                    {
                        return decimalLimit;
                    }
                    else
                    {
                        return decimalLength;
                    }
                }
            }
            catch { }
            return 0;
        }
    }
}