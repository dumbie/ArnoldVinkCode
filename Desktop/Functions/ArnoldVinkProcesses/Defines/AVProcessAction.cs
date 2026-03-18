namespace ArnoldVinkCode
{
    public partial class AVProcess
    {
        public enum AVProcessActions
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

        public class AVProcessAction
        {
            public AVProcessActions Action { get; set; } = AVProcessActions.NoAction;
            public AVProcess Process { get; set; } = null;
        }
    }
}