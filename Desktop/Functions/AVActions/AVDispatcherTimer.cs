using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Threading;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVDispatcherTimer : DispatcherTimer
        {
            /// <summary>
            /// Set tick and remove previous tick events
            /// </summary>
            public void SetTick(EventHandler tickEvent)
            {
                try
                {
                    //Remove tick events
                    FieldInfo fieldInfo = typeof(DispatcherTimer).GetField("Tick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(this, null);
                    }

                    //Set tick event
                    this.Tick += tickEvent;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to set timer tick: " + ex.Message);
                }
            }

            /// <summary>
            /// Add tick and keep previous tick events
            /// </summary>
            public void AddTick(EventHandler tickEvent)
            {
                try
                {
                    //Add tick event
                    this.Tick += tickEvent;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to add timer tick: " + ex.Message);
                }
            }

            /// <summary>
            /// Set timer interval in milliseconds
            /// </summary>
            public void SetInterval(uint intervalMs)
            {
                try
                {
                    this.Interval = TimeSpan.FromMilliseconds(intervalMs);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to set timer interval: " + ex.Message);
                }
            }

            /// <summary>
            /// Start or extend current timer
            /// </summary>
            public void StartOrExtend()
            {
                try
                {
                    this.Stop();
                    this.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to start or extend timer: " + ex.Message);
                }
            }
        }
    }
}