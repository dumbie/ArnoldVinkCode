using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<example>AVActions.GetSystemTicksMicro();</example>
        public static long GetSystemTicksMicro()
        {
            try
            {
                long timeStamp;
                QueryPerformanceCounter(out timeStamp);
                return timeStamp;
            }
            catch { }
            return Environment.TickCount64 * 10000;
        }

        ///<example>AVActions.GetSystemTicksMilli();</example>
        public static long GetSystemTicksMilli()
        {
            try
            {
                long timeStamp;
                QueryPerformanceCounter(out timeStamp);
                return timeStamp / 10000;
            }
            catch { }
            return Environment.TickCount;
        }
    }
}