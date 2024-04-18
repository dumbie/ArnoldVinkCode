using System;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVHighResDelay
        {
            public static void Delay(float milliSecondsDelay)
            {
                try
                {
                    if (milliSecondsDelay < 0.1F)
                    {
                        milliSecondsDelay = 0.1F;
                    }

                    long nanoSecondsDelay = (long)(-1.0F * milliSecondsDelay * 10000.0F);
                    IntPtr waitableTimer = CreateWaitableTimerEx(IntPtr.Zero, IntPtr.Zero, CreateWaitableTimerFlags.TIMER_MANUAL_RESET | CreateWaitableTimerFlags.TIMER_HIGH_RESOLUTION, CreateWaitableTimerAccess.TIMER_ALL_ACCESS);
                    if (SetWaitableTimerEx(waitableTimer, ref nanoSecondsDelay, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0))
                    {
                        WaitForSingleObject(waitableTimer, INFINITE);
                    }
                }
                catch { }
            }
        }
    }
}