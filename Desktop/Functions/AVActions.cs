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
        public static Task TaskStart(Action ActionRun, CancellationTokenSource TaskToken)
        {
            try
            {
                if (TaskToken == null)
                {
                    TaskToken = new CancellationTokenSource();
                    using (TaskToken)
                    {
                        return Task.Run(ActionRun, TaskToken.Token);
                    }
                }
                else
                {
                    return Task.Run(ActionRun, TaskToken.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start task: " + ex.Message);
                return null;
            }
        }

        //async Task<string> TaskAction() { return ""; }
        //AVActions.TaskStartReturn(TaskAction, null);
        //Tip: Don't forget to use try and catch to improve stability
        public static Task<T> TaskStartReturn<T>(Func<T> ActionRun, CancellationTokenSource TaskToken)
        {
            try
            {
                if (TaskToken == null)
                {
                    TaskToken = new CancellationTokenSource();
                    using (TaskToken)
                    {
                        return Task.Run(ActionRun, TaskToken.Token);
                    }
                }
                else
                {
                    return Task.Run(ActionRun, TaskToken.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start return task: " + ex.Message);
                return null;
            }
        }

        //Check if a task is still running
        public static bool TaskRunningCheck(CancellationTokenSource TaskToken)
        {
            try
            {
                return TaskToken != null && !TaskToken.IsCancellationRequested;
            }
            catch { }
            return false;
        }

        //Example: AVActions.TaskStop(Task, Token);
        public async static Task TaskStop(Task TaskStop, CancellationTokenSource TaskToken)
        {
            try
            {
                //Cancel the task token
                TaskToken.Cancel();

                //Wait for task loop
                while (!TaskStop.IsCompleted)
                {
                    Debug.WriteLine("Waiting for task loop to complete.");
                    await Task.Delay(500);
                }

                //Wait for task stop
                await Task.Delay(1000);

                //Dispose the used task
                TaskStop.Dispose();
                TaskToken.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop task: " + ex.Message);
            }
        }

        //async void DispatchAction() { void(); }
        //AVActions.ActionDispatcherInvoke(DispatchAction, null);
        //Tip: Don't forget to use try and catch to improve stability
        public static void ActionDispatcherInvoke(Action ActionRun)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(ActionRun);
            }
            catch { }
        }

        //async void DispatchAction() { void(); }
        //await AVActions.ActionDispatcherInvokeAsync(DispatchAction, null);
        //Tip: Don't forget to use try and catch to improve stability
        public static async Task ActionDispatcherInvokeAsync(Action ActionRun)
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(ActionRun);
            }
            catch { }
        }
    }
}