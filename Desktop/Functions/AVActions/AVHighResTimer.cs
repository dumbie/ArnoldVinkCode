using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVHighResTimer
        {
            //Variables
            private bool timerAllowed = false;
            private IntPtr timerWaitable = IntPtr.Zero;
            private event EventHandler timerTick = null;
            private long intervalNanoSeconds = 0;
            private uint intervalMilliSeconds = 0;

            /// <summary>
            /// Set action to run on timer tick.
            /// </summary>
            public EventHandler Tick
            {
                set
                {
                    timerTick = null;
                    timerTick += value;
                }
            }
            /// <summary>
            /// Time in milliseconds between triggering tick.
            /// </summary>
            public uint Interval
            {
                get { return intervalMilliSeconds; }
                set
                {
                    intervalMilliSeconds = value;
                    intervalNanoSeconds = (long)(-1.0F * value * 10000.0F);
                }
            }
            /// <summary>
            /// Check if timer is currently running.
            /// </summary>
            public bool IsRunning
            {
                get { return timerWaitable != IntPtr.Zero; }
            }

            //Initialize
            public AVHighResTimer() { }

            public AVHighResTimer(uint milliSecondsInterval)
            {
                Interval = milliSecondsInterval;
            }

            public AVHighResTimer(uint milliSecondsInterval, Action tickAction)
            {
                Interval = milliSecondsInterval;
                Tick = delegate { tickAction(); };
            }

            public AVHighResTimer(uint milliSecondsInterval, Func<Task> tickAction)
            {
                Interval = milliSecondsInterval;
                Tick = async delegate { await tickAction(); };
            }

            //Functions
            public bool Start()
            {
                try
                {
                    if (Interval <= 0)
                    {
                        Debug.WriteLine("Invalid timer interval.");
                        return false;
                    }

                    if (timerTick == null)
                    {
                        Debug.WriteLine("Invalid timer action tick.");
                        return false;
                    }

                    if (timerWaitable != IntPtr.Zero)
                    {
                        //Debug.WriteLine("Timer is already running, extending.");
                        SetWaitableTimerEx(timerWaitable, ref intervalNanoSeconds, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);
                        return false;
                    }

                    //Debug.WriteLine("Creating new timer");

                    //Create waitable timer
                    timerWaitable = CreateWaitableTimerEx(IntPtr.Zero, IntPtr.Zero, CreateWaitableTimerFlags.TIMER_MANUAL_RESET | CreateWaitableTimerFlags.TIMER_HIGH_RESOLUTION, CreateWaitableTimerAccess.TIMER_ALL_ACCESS);

                    //Check waitable timer
                    if (timerWaitable == IntPtr.Zero)
                    {
                        Debug.WriteLine("Failed to create timer.");
                        return false;
                    }

                    //Update variables
                    timerAllowed = true;

                    //Start timer loop
                    Thread timerThread = new Thread(delegate ()
                    {
                        while (timerAllowed)
                        {
                            try
                            {
                                if (SetWaitableTimerEx(timerWaitable, ref intervalNanoSeconds, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0))
                                {
                                    WaitForSingleObject(timerWaitable, INFINITE);
                                    if (timerAllowed)
                                    {
                                        timerTick(this, null);
                                    }
                                }
                            }
                            catch { }
                        }
                    });
                    timerThread.IsBackground = true;
                    timerThread.Start();

                    //Return result
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to start timer: " + ex.Message);
                    return false;
                }
            }

            public bool Stop()
            {
                try
                {
                    if (timerWaitable == IntPtr.Zero)
                    {
                        Debug.WriteLine("Timer is not running.");
                        return false;
                    }

                    //Debug.WriteLine("Stopping timer: " + timerWaitable);

                    //Update variables
                    timerAllowed = false;

                    //Set waitable timer to stop wait
                    long intervalZero = 0;
                    SetWaitableTimerEx(timerWaitable, ref intervalZero, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);

                    //Cancel waitable timer
                    CancelWaitableTimer(timerWaitable);

                    //Close waitable timer handle
                    SafeCloseHandle(ref timerWaitable);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to stop timer: " + ex.Message);
                    return false;
                }
            }
        }
    }
}