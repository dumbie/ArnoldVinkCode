namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public class ProcessAccess
        {
            public bool UiAccess { get; set; } = false;
            public bool AdminAccess { get; set; } = false;
            public bool Elevation { get; set; } = false;
            public TOKEN_ELEVATION_TYPE ElevationType { get; set; } = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
        }

        public class ProcessThread
        {
            public int ThreadId { get; set; } = 0;
            public ProcessThreadState ThreadState { get; set; } = ProcessThreadState.Unknown;
            public ProcessThreadWaitReason ThreadWaitReason { get; set; } = ProcessThreadWaitReason.Unknown;
        }
    }
}