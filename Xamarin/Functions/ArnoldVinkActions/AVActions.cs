using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
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

        ///<param name="actionRun">async Task TaskAction() { while (!AVTask.TaskStopRequest) { void(); await TaskDelayLoop(1000, AVTask); } }</param>
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

        ///<param name="actionRun">async Task TaskAction() { while (!AVTask.TaskStopRequest) { void(); await TaskDelayLoop(1000, AVTask); } }</param>
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

        ///<example>AVActions.TaskStopLoop(AVTask);</example>
        public static async Task TaskStopLoop(AVTaskDetails avTask)
        {
            try
            {
                //Check if the task is stopped
                if (avTask.TaskStopRequest || avTask.TaskCompleted || !avTask.TaskRunning)
                {
                    Debug.WriteLine("Loop task is stopping or not running.");
                    return;
                }

                //Signal the loop task to stop
                avTask.TokenSource.Cancel();

                //Wait for task to have stopped
                Debug.WriteLine("Waiting for task to stop or timeout...");
                long stopTimeout = GetSystemTicksMs();
                while (!avTask.TaskCompleted)
                {
                    if ((GetSystemTicksMs() - stopTimeout) >= 8000)
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
                await Task.Delay(millisecondsDelay, avTask.TokenSource.Token);
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

        ///<param name="actionRun">void DispatchAction() { void(); }</param>
        ///<example>AVActions.ActionDispatcherInvoke(DispatchAction, null);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void ActionDispatcherInvoke(Action actionRun)
        {
            try
            {
                Device.BeginInvokeOnMainThread(actionRun);
            }
            catch { }
        }
    }
}