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
                IntPtr createEvent = IntPtr.Zero;
                try
                {
                    if (milliSecondsDelay == 0) { milliSecondsDelay = 1; }
                    createEvent = CreateEvent(IntPtr.Zero, true, false, null);
                    WaitForSingleObject(createEvent, milliSecondsDelay);
                }
                catch { }
                finally
                {
                    SafeCloseEvent(createEvent);
                }
            }
        }
    }
}