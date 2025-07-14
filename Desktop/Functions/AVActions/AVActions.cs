using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<example>AVActions.GetSystemTicksMs();</example>
        public static long GetSystemTicksMs()
        {
            try
            {
                return Stopwatch.GetTimestamp() / 10000;
            }
            catch { }
            return Environment.TickCount;
        }
    }
}