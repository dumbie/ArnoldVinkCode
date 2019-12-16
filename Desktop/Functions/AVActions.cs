using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        //async void TaskAction() { void(); }
        //AVActions.TaskStart(TaskAction, null);
        //Tip: Don't forget to use try and catch to improve stability
        public static Task TaskStart(Action actionRun, CancellationTokenSource taskToken)
        {
            try
            {
                if (taskToken == null)
                {
                    taskToken = new CancellationTokenSource();
                    using (taskToken)
                    {
                        return Task.Run(actionRun, taskToken.Token);
                    }
                }
                else
                {
                    return Task.Run(actionRun, taskToken.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start task: " + ex.Message);
                return null;
            }
        }

        //async Task TaskAction() { void(); }
        //await AVActions.TaskStartAsync(TaskAction, null);
        //Tip: Don't forget to use try and catch to improve stability
        public static async Task TaskStartAsync(Func<Task> actionRun, CancellationTokenSource taskToken)
        {
            try
            {
                if (taskToken == null)
                {
                    taskToken = new CancellationTokenSource();
                    using (taskToken)
                    {
                        await Task.Run(actionRun, taskToken.Token);
                    }
                }
                else
                {
                    await Task.Run(actionRun, taskToken.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start task: " + ex.Message);
            }
        }

        //string TaskAction() { return ""; }
        //await AVActions.TaskStartReturn(TaskAction, null);
        //Tip: Don't forget to use try and catch to improve stability
        public static Task<T> TaskStartReturn<T>(Func<T> actionRun, CancellationTokenSource taskToken)
        {
            try
            {
                if (taskToken == null)
                {
                    taskToken = new CancellationTokenSource();
                    using (taskToken)
                    {
                        return Task.Run(actionRun, taskToken.Token);
                    }
                }
                else
                {
                    return Task.Run(actionRun, taskToken.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return task: " + ex.Message);
                return null;
            }
        }

        //Check if a task is still running
        public static bool TaskRunningCheck(CancellationTokenSource taskToken)
        {
            try
            {
                return taskToken != null && !taskToken.IsCancellationRequested;
            }
            catch { }
            return false;
        }

        //Example: AVActions.TaskStop(Task, Token);
        public async static Task TaskStop(Task taskStop, CancellationTokenSource taskToken)
        {
            try
            {
                //Cancel the task token
                taskToken.Cancel();

                //Wait for task loop
                while (!taskStop.IsCompleted)
                {
                    Debug.WriteLine("Waiting for task loop to complete.");
                    await Task.Delay(500);
                }

                //Wait for task stop
                await Task.Delay(1000);

                //Dispose the used task
                taskStop.Dispose();
                taskToken.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop task: " + ex.Message);
            }
        }

        //async void DispatchAction() { void(); }
        //AVActions.ActionDispatcherInvoke(DispatchAction, null);
        //Tip: Don't forget to use try and catch to improve stability
        public static void ActionDispatcherInvoke(Action actionRun)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(actionRun);
            }
            catch { }
        }

        //async void DispatchAction() { void(); }
        //await AVActions.ActionDispatcherInvokeAsync(DispatchAction, null);
        //Tip: Don't forget to use try and catch to improve stability
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