using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">void TaskAction() { while (TaskCheckLoop(AVTask)) { void(); } }</param>
        ///<example>AVActions.TaskStartLoop(TaskAction, AVTask);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStartLoop(Action actionRun, AVTaskDetails avTask)
        {
            try
            {
                if (avTask != null)
                {
                    //Dispose and reset task
                    avTask.DisposeReset();

                    //Create new loop task
                    avTask.TokenSource = new CancellationTokenSource();
                    avTask.Task = Task.Run(actionRun);

                    Debug.WriteLine("Loop task has been started: " + avTask.Name);
                }
                else
                {
                    Debug.WriteLine("Failed to start loop task: AVTask is null.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start loop task: " + avTask.Name + " / " + ex.Message);
            }
        }

        ///<param name="actionRun">async Task TaskAction() { while (TaskCheckLoop(AVTask)) { void(); await TaskDelay(1000, AVTask); } }</param>
        ///<example>AVActions.TaskStartLoop(TaskAction, AVTask);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static void TaskStartLoop(Func<Task> actionRun, AVTaskDetails avTask)
        {
            try
            {
                if (avTask != null)
                {
                    //Dispose and reset task
                    avTask.DisposeReset();

                    //Create new loop task
                    avTask.TokenSource = new CancellationTokenSource();
                    avTask.Task = Task.Run(actionRun);

                    Debug.WriteLine("Loop task has been started: " + avTask.Name);
                }
                else
                {
                    Debug.WriteLine("Failed to start loop task: AVTask is null.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start loop task: " + avTask.Name + " / " + ex.Message);
            }
        }

        ///<example>AVActions.TaskCheckLoop(AVTask);</example>
        ///<summary>Returns true if loop is still allowed to continue</summary>
        public static bool TaskCheckLoop(AVTaskDetails avTask)
        {
            try
            {
                if (avTask == null)
                {
                    Debug.WriteLine("Loop task is null: " + avTask.Name);
                    return false;
                }
                else if (avTask.TokenSource == null)
                {
                    Debug.WriteLine("Loop token is null: " + avTask.Name);
                    return false;
                }
                else if (avTask.TokenSource.IsCancellationRequested)
                {
                    Debug.WriteLine("Loop token cancellation requested: " + avTask.Name);
                    return false;
                }
                else if (avTask.TaskStopRequest)
                {
                    Debug.WriteLine("Loop task is requested to stop: " + avTask.Name);
                    return false;
                }
                else if (avTask.TaskCompleted)
                {
                    Debug.WriteLine("Loop task has completed: " + avTask.Name);
                    return false;
                }
                else if (!avTask.TaskRunning)
                {
                    Debug.WriteLine("Loop task is not running: " + avTask.Name);
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
                Debug.WriteLine("Waiting for task " + avTask.Name + " to stop or timeout...");
                long stopTimeout = GetSystemTicksMs();
                while (!avTask.TaskCompleted)
                {
                    if (taskTimeout > 0 && (GetSystemTicksMs() - stopTimeout) >= taskTimeout)
                    {
                        Debug.WriteLine("Stopping task " + avTask.Name + " has timed out...");
                        break;
                    }
                    await Task.Delay(1);
                }

                //Dispose and reset task
                avTask.DisposeReset();

                Debug.WriteLine("Loop task " + avTask.Name + " has been stopped.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop loop task: " + avTask.Name + " / " + ex.Message);
            }
        }
    }
}