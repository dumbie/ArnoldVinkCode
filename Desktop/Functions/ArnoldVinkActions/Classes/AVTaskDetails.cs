using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVTaskDetails
        {
            public Task Task { get; set; } = null;
            public bool TaskStopRequest { get; set; } = false;
            public bool TaskRunning { get { return Task != null && !Task.IsCompleted; } }
            public bool TaskCompleted { get { return Task == null || Task.IsCompleted; } }
        }
    }
}