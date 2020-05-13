﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        //async void TaskAction() { void(); }
        //AVActions.TaskStart(TaskAction);
        //Tip: Don't forget to use try and catch to improve stability
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

        //string TaskAction() { return ""; }
        //async Task<string> TaskAction() { return ""; }
        //await AVActions.TaskStartReturn(TaskAction);
        //Tip: Don't forget to use try and catch to improve stability
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

        //async void TaskAction() { void(); await TaskDelayLoop(1000, AVTaskDetails); }
        //AVActions.TaskStartLoop(TaskAction, AVTaskDetails);
        //Tip: Don't forget to use try and catch to improve stability
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
                    avTask.Status = AVTaskStatus.Running;
                    avTask.Task = Task.Run(actionRun);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start loop task: " + ex.Message);
            }
        }

        //Example: AVActions.TaskStopLoop(AVTaskDetails);
        //Tip: Don't forget to set status to cancelled after loop ends
        public static async Task TaskStopLoop(AVTaskDetails avTask)
        {
            try
            {
                //Check if the task is stopped
                if (avTask.Status != AVTaskStatus.Running)
                {
                    Debug.WriteLine("Task is already stopped.");
                    return;
                }

                //Signal the loop task to stop
                avTask.Status = AVTaskStatus.StopRequested;

                //Wait for task to have stopped or timeout
                int taskStopTimeout = Environment.TickCount;
                while (avTask.Status != AVTaskStatus.Stopped && (Environment.TickCount - taskStopTimeout) < 3000)
                {
                    Debug.WriteLine("Waiting for task to stop or timeout...");
                    await Task.Delay(1);
                }

                //Reset the used task
                avTask.Status = AVTaskStatus.Null;
                avTask.Task = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to stop loop task: " + ex.Message);
            }
        }

        //Example: AVActions.TaskDelayLoop(1000, AVTaskDetails);
        public static async Task TaskDelayLoop(int millisecondsDelay, AVTaskDetails avTask)
        {
            try
            {
                int delayedTime = Environment.TickCount;
                while (avTask.Status == AVTaskStatus.Running && (Environment.TickCount - delayedTime) < millisecondsDelay)
                {
                    await Task.Delay(1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to delay loop task: " + ex.Message);
            }
        }

        //AVActions.ElementGetValue(targetElement, targetProperty);
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

        //AVActions.ElementSetValue(targetElement, targetProperty, targetValue);
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