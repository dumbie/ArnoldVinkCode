using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVActions
    {
        public class AVTaskDetails
        {
            public AVTaskStatus Status { get; set; } = AVTaskStatus.Null;
            public Task Task { get; set; } = null;
        }

        public enum AVTaskStatus : int
        {
            Null = 0,
            Running = 1,
            StopRequested = 2,
            Stopped = 3
        }
    }
}