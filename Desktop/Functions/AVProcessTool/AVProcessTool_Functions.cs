namespace ArnoldVinkCode
{
    public partial class AVProcessTool
    {
        public static string CommandLine_PrepareArgument(string argument)
        {
            try
            {
                argument = argument.Replace("\\", "\\\\");
                argument = argument.Replace("\"", "\\\"");
                argument = "\"" + argument + "\"";
            }
            catch { }
            return argument;
        }
    }
}