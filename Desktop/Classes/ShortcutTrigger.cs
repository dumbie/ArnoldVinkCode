using static ArnoldVinkCode.AVInputOutputClass;

namespace ArnoldVinkCode
{
    public partial class AVClasses
    {
        public class ShortcutTriggerKeyboard
        {
            public string Name { get; set; }
            public KeysVirtual[] Trigger { get; set; }
        }

        public class ShortcutTriggerController
        {
            public string Name { get; set; }
            public bool Hold { get; set; }
            public ControllerButtons[] Trigger { get; set; }
        }
    }
}