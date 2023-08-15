using System.Threading;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVTaskDetails
        {
            public AVTaskDetails(string taskName = "Unknown task")
            {
                Name = taskName;
            }

            public string Name { get; set; } = string.Empty;
            public Task Task { get; set; } = null;
            public CancellationTokenSource TokenSource { get; set; } = null;
            public bool TaskStopRequested { get { return TokenSource != null && TokenSource.IsCancellationRequested; } }
            public bool TaskRunning { get { return Task != null && !Task.IsCompleted; } }
            public bool TaskCompleted { get { return Task != null && Task.IsCompleted; } }

            ///<summary>Signal task to stop</summary>
            public void Stop()
            {
                try
                {
                    if (TokenSource != null)
                    {
                        TokenSource.Cancel();
                    }
                }
                catch { }
            }

            ///<summary>Resets task to default state</summary>
            public void Reset()
            {
                try
                {
                    if (Task != null)
                    {
                        Task.Dispose();
                        Task = null;
                    }
                    if (TokenSource != null)
                    {
                        TokenSource.Dispose();
                        TokenSource = null;
                    }
                }
                catch { }
            }
        }
    }
}