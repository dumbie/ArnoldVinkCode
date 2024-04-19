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
                IntPtr createEvent = IntPtr.Zero;
                try
                {
                    if (milliSecondsDelay < 0.1F)
                    {
                        milliSecondsDelay = 0.1F;
                    }

                    long nanoSecondsDelay = (long)(-1.0F * milliSecondsDelay * 10000.0F);
                    createEvent = CreateWaitableTimerEx(IntPtr.Zero, IntPtr.Zero, CreateWaitableTimerFlags.TIMER_MANUAL_RESET | CreateWaitableTimerFlags.TIMER_HIGH_RESOLUTION, CreateWaitableTimerAccess.TIMER_ALL_ACCESS);
                    if (SetWaitableTimerEx(createEvent, ref nanoSecondsDelay, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0))
                    {
                        WaitForSingleObject(createEvent, INFINITE);
                    }
                }
                catch { }
                finally
                {
                    SafeCloseHandle(createEvent);
                }
            }
        }
    }
}