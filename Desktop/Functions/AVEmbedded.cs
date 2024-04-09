using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ArnoldVinkCode
{
    public partial class AVEmbedded
    {
        public static Stream EmbeddedResourceToStream(Assembly targetAssembly, string resourcePath)
        {
            try
            {
                if (targetAssembly == null) { targetAssembly = Assembly.GetEntryAssembly(); }
                return targetAssembly.GetManifestResourceStream(resourcePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get embedded resource stream: " + ex.Message);
                return null;
            }
        }

        public static byte[] EmbeddedResourceToBytes(Assembly targetAssembly, string resourcePath)
        {
            try
            {
                if (targetAssembly == null) { targetAssembly = Assembly.GetEntryAssembly(); }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (Stream fileStream = targetAssembly.GetManifestResourceStream(resourcePath))
                    {
                        fileStream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get embedded resource bytes: " + ex.Message);
                return null;
            }
        }
    }
}