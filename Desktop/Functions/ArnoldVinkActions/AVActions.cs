using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">void TaskAction() { void(); }</param>
        ///<param name="actionRun">async void TaskAction() { void(); }</param>
        ///<example>AVActions.TaskStart(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static Task TaskStart(Action actionRun)
        {
            try
            {
                return Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start regular task: " + ex.Message);
                return null;
            }
        }

        ///<param name="actionRun">string TaskAction() { return ""; }</param>
        ///<param name="actionRun">async Task<string> TaskAction() { return ""; }</param>
        ///<example>await AVActions.TaskStartReturn(TaskAction);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static Task<T> TaskStartReturn<T>(Func<T> actionRun)
        {
            try
            {
                return Task.Run(actionRun);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return task: " + ex.Message);
                return null;
            }
        }

        ///<param name="actionRun">void TaskAction() { void(); }</param>
        ///<param name="actionRun">async void TaskAction() { void(); }</param>
        ///<example>await AVActions.TaskStartTimeout(TaskAction, 3000);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<bool> TaskStartTimeout(Action actionRun, int timeoutMs)
        {
            try
            {
                Task runTask = Task.Run(actionRun);
                Task delayTask = Task.Delay(timeoutMs);
                Task timeoutTask = await Task.WhenAny(runTask, delayTask);
                if (timeoutTask == runTask)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start timeout task: " + ex.Message);
                return false;
            }
        }

        ///<param name="actionRun">async Task<string> TaskAction() { return ""; }</param>
        ///<param name="actionRun">Task<string> TaskAction() { return Task.FromResult(""); }</param>
        ///<example>await AVActions.TaskStartReturnTimeout(TaskAction(), 3000);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<T> TaskStartReturnTimeout<T>(Task<T> actionRun, int timeoutMs) where T : class
        {
            try
            {
                Task delayTask = Task.Delay(timeoutMs);
                Task timeoutTask = await Task.WhenAny(actionRun, delayTask);
                if (timeoutTask == actionRun)
                {
                    return actionRun.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return timeout task: " + ex.Message);
                return null;
            }
        }

        ///<param name="actionRun">async Task TaskAction() { while (TaskCheckLoop(AVTask)) { void(); await TaskDelayLoop(1000, AVTask); } }</param>
        ///<example>AVActions.TaskStartLoop(TaskAction, AVTask);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStartLoop(Func<Task> actionRun, AVTaskDetails avTask)
        {
            try
            {
                if (avTask == null)
                {
                    Task.Run(actionRun);
                }
                else
                {
                    //Dispose and reset task
                    avTask.DisposeReset();

                    //Create new loop task
                    avTask.Task = Task.Run(actionRun);
                }
                Debug.WriteLine("Loop task has been started.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start loop task: " + ex.Message);
            }
        }

        ///<param name="actionRun">async Task TaskAction() { while (TaskCheckLoop(AVTask)) { void(); await TaskDelayLoop(1000, AVTask); } }</param>
        ///<example>AVActions.TaskStartLoop(TaskAction, AVTask);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStartLoop(Action actionRun, AVTaskDetails avTask)
        {
            try
            {
                if (avTask == null)
                {
                    Task.Run(actionRun);
                }
                else
                {
                    //Dispose and reset task
                    avTask.DisposeReset();

                    //Create new loop task
                    avTask.Task = Task.Run(actionRun);
                }
                Debug.WriteLine("Loop task has been started.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start loop task: " + ex.Message);
            }
        }

        ///<example>AVActions.TaskCheckLoop(AVTask);</example>
        ///<summary>Returns true if loop is still allowed to continue</summary>
        public static bool TaskCheckLoop(AVTaskDetails avTask)
        {
            try
            {
                if (avTask.TaskStopRequest)
                {
                    Debug.WriteLine("Loop task is requested to stop.");
                    return false;
                }
                else if (avTask.TaskCompleted)
                {
                    Debug.WriteLine("Loop task has completed.");
                    return false;
                }
                else if (!avTask.TaskRunning)
                {
                    Debug.WriteLine("Loop task is not running.");
                    return false;
                }
            }
            catch { }
            return true;
        }

        ///<example>AVActions.TaskStopLoop(AVTask);</example>
        public static async Task TaskStopLoop(AVTaskDetails avTask, int taskTimeout)
        {
            try
            {
                //Check if the task is stopped
                if (!TaskCheckLoop(avTask))
                {
                    return;
                }

                //Signal the loop task to stop
                avTask.TokenSource.Cancel();

                //Wait for task to have stopped
                Debug.WriteLine("Waiting for task to stop or timeout...");
                long stopTimeout = GetSystemTicksMs();
                while (!avTask.TaskCompleted)
                {
                    if (taskTimeout > 0 && (GetSystemTicksMs() - stopTimeout) >= taskTimeout)
                    {
                        Debug.WriteLine("Stopping the task has timed out...");
                        break;
                    }
                    await Task.Delay(1);
                }

                //Dispose and reset task
                avTask.DisposeReset();

                Debug.WriteLine("Loop task has been stopped.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop loop task: " + ex.Message);
            }
        }

        ///<example>await AVActions.TaskDelayLoop(1000, AVTask);</example>
        public static async Task TaskDelayLoop(int millisecondsDelay, AVTaskDetails avTask)
        {
            try
            {
                if (millisecondsDelay < 50)
                {
                    TaskDelayMs((uint)millisecondsDelay);
                }
                else
                {
                    await Task.Delay(millisecondsDelay, avTask.TokenSource.Token);
                }
            }
            catch { }
        }

        ///<example>await AVActions.TaskDelayMs(1);</example>
        ///<summary>High resolution delay, only use for sub 50ms.</summary>
        public static void TaskDelayMs(uint millisecondsDelay)
        {
            try
            {
                IntPtr createEvent = CreateEvent(IntPtr.Zero, true, false, null);
                MultimediaTimerCallback callbackDone = delegate
                {
                    SetEvent(createEvent);
                    CloseHandle(createEvent);
                };

                timeSetEvent(millisecondsDelay, 0, callbackDone, 0, 0);
                WaitForSingleObject(createEvent, INFINITE);
                callbackDone.EndInvoke(null);
            }
            catch { }
        }

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

        ///<example>AVActions.ElementGetValue(targetElement, targetProperty);</example>
        public static object ElementGetValue(FrameworkElement targetElement, DependencyProperty targetProperty)
        {
            object returnValue = null;
            try
            {
                ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        returnValue = targetElement.GetValue(targetProperty);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed getting value: " + ex.Message);
                    }
                });
            }
            catch { }
            return returnValue;
        }

        ///<example>AVActions.ElementSetValue(targetElement, targetProperty, targetValue);</example>
        public static void ElementSetValue(FrameworkElement targetElement, DependencyProperty targetProperty, object targetValue)
        {
            try
            {
                ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        targetElement.SetValue(targetProperty, targetValue);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed setting value: " + ex.Message);
                    }
                });
            }
            catch { }
        }

        ///<param name="actionRun">void DispatchAction() { void(); }</param>
        ///<example>AVActions.ActionDispatcherInvoke(DispatchAction, null);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void ActionDispatcherInvoke(Action actionRun)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(actionRun);
            }
            catch { }
        }

        ///<param name="actionRun">async void DispatchAction() { void(); }</param>
        ///<example>await AVActions.ActionDispatcherInvokeAsync(DispatchAction, null);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task ActionDispatcherInvokeAsync(Action actionRun)
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(actionRun);
            }
            catch { }
        }
    }
}