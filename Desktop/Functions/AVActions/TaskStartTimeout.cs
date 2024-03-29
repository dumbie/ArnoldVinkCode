﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        ///<param name="actionRun">void TaskAction() { void(); }</param>
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

        ///<param name="actionRun">string TaskAction() { return ""; }</param>
        ///<example>await AVActions.TaskStartTimeout(TaskAction, 3000);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<T> TaskStartTimeout<T>(Func<T> actionRun, int timeoutMs) where T : class
        {
            try
            {
                Task<T> runTask = Task.Run(actionRun);
                Task delayTask = Task.Delay(timeoutMs);
                Task timeoutTask = await Task.WhenAny(runTask, delayTask);
                if (timeoutTask == runTask)
                {
                    return runTask.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start timeout task: " + ex.Message);
                return null;
            }
        }

        ///<param name="actionRun">async Task TaskAction() { void(); }</param>
        ///<example>await AVActions.TaskStartTimeout(TaskAction, 3000);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<bool> TaskStartTimeout(Func<Task> actionRun, int timeoutMs)
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
        ///<example>await AVActions.TaskStartTimeout(TaskAction, 3000);</example>
        ///<summary>Don't forget to use try and catch to improve stability</summary>
        public static async Task<T> TaskStartTimeout<T>(Func<Task<T>> actionRun, int timeoutMs) where T : class
        {
            try
            {
                Task<T> runTask = Task.Run(actionRun);
                Task delayTask = Task.Delay(timeoutMs);
                Task timeoutTask = await Task.WhenAny(runTask, delayTask);
                if (timeoutTask == runTask)
                {
                    return runTask.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start timeout task: " + ex.Message);
                return null;
            }
        }
    }
}