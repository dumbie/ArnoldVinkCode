namespace ArnoldVinkCode.AVDevices
{
    public partial class Interop
    {
        public class EnumerateInfo
        {
            public string DevicePath { get; set; }
            public string DeviceInstanceId { get; set; }
            public string Description { get; set; }
            public string HardwareId { get; set; }
            public bool IsWireless { get; set; }
            public CM_POWER_DATA? PowerData { get; set; }
        }
    }
}