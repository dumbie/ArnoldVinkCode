using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputClass
    {
        public class KeyboardMessage
        {
            public WindowMessages windowMessage { get; set; }
            public KeysVirtual keyVirtual { get; set; }
        }
    }
}