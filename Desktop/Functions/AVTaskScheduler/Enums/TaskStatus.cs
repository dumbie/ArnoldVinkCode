namespace ArnoldVinkCode
{
    public partial class AVTaskScheduler
    {
        public enum TaskStatus
        {
            Unknown = 0,
            TaskOk = 1,
            TaskNotFound = 2,
            ExeNotFound = 3,
            PathChanged = 4
        }
    }
}