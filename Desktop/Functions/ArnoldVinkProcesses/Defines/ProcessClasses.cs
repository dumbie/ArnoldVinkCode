namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public class ProcessAccessStatus
        {
            public bool UiAccess { get; set; } = false;
            public bool AdminAccess { get; set; } = false;
            public bool Elevation { get; set; } = false;
            public TOKEN_ELEVATION_TYPE ElevationType { get; set; } = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
        }

        public class ProcessThreadInfo
        {
            public int Identifier { get; set; } = 0;
            public ProcessThreadState ThreadState { get; set; } = ProcessThreadState.Unknown;
            public ProcessThreadWaitReason ThreadWaitReason { get; set; } = ProcessThreadWaitReason.Unknown;

            public bool Suspended
            {
                get
                {
                    return ThreadState == ProcessThreadState.Waiting && ThreadWaitReason == ProcessThreadWaitReason.Suspended;
                }
            }
        }
    }
}