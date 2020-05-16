using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
                    avTask.TaskStopRequest = false;
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
                if (avTask.TaskCompleted || avTask.TaskStopRequest)
                {
                    Debug.WriteLine("Loop task is stopping or not running.");
                    return;
                }

                //Signal the loop task to stop
                avTask.TaskStopRequest = true;

                //Wait for task to have stopped or timeout
                int taskStopTimeout = Environment.TickCount;
                while (!avTask.TaskCompleted && (Environment.TickCount - taskStopTimeout) < 3000)
                {
                    Debug.WriteLine("Waiting for task to stop or timeout...");
                    await Task.Delay(1);
                }

                //Reset the used task
                avTask.TaskStopRequest = false;
                avTask.Task.Dispose();
                avTask.Task = null;
                Debug.WriteLine("Loop task has been stopped.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop loop task: " + ex.Message);
            }
        }

        ///<example>AVActions.TaskDelayLoop(1000, AVTask);</example>
        public static void TaskDelayLoop(int millisecondsDelay, AVTaskDetails avTask)
        {
            try
            {
                if (millisecondsDelay <= 0) { return; }
                int delayTimeMs = Environment.TickCount;
                while (!avTask.TaskStopRequest && (Environment.TickCount - delayTimeMs) < millisecondsDelay)
                {
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to delay loop task: " + ex.Message);
            }
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