using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVHighResDelay
        {
            public static void Delay(uint milliSecondsDelay)
            {
                try
                {
                    if (milliSecondsDelay == 0) { milliSecondsDelay = 1; }
                    IntPtr createEvent = CreateEvent(IntPtr.Zero, true, false, null);
                    WaitForSingleObject(createEvent, milliSecondsDelay);
                    SafeCloseEvent(createEvent);
                }
                catch { }
            }
        }
    }
}