using System;
using System.Diagnostics;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVDebug
    {
        //Write line to Debug and Console
        public static void WriteLine(object writeLine)
        {
            try
            {
                Debug.WriteLine(writeLine);
                Console.WriteLine(writeLine);
            }
            catch { }
        }

        //Write debug text to file
        public static bool WriteFile(string debugText)
        {
            try
            {
                if (!debugText.EndsWith("\n") && !debugText.EndsWith("\r\n"))
                {
                    File.AppendAllText("AVDebug.txt", debugText + "\r\n");
                }
                else
                {
                    File.AppendAllText("AVDebug.txt", debugText);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed appending debug text: " + ex.Message);
                return false;
            }
        }
    }
}