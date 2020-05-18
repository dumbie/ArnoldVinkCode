using System.Threading;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVTaskDetails
        {
            public Task Task { get; set; } = null;
            public CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();
            public CancellationToken TokenCancel { get { return TokenSource.Token; } }
            public bool TaskStopRequest { get { return TokenSource.IsCancellationRequested; } }
            public bool TaskRunning { get { return Task != null && !Task.IsCompleted; } }
            public bool TaskCompleted { get { return Task == null || Task.IsCompleted; } }

            public void DisposeReset()
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
                        TokenSource = new CancellationTokenSource();
                    }
                }
                catch { }
            }
        }
    }
}