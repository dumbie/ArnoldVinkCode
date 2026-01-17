namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public enum ProcessMultiActions
        {
            Launch,
            Close,
            CloseAll,
            Restart,
            RestartAll,
            Select,
            NoAction,
            Cancel
        }

        public class ProcessMultiAction
        {
            public ProcessMultiActions Action { get; set; } = ProcessMultiActions.NoAction;
            public ProcessMulti ProcessMulti { get; set; } = null;
        }
    }
}