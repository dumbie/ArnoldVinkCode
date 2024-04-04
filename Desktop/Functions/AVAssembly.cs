using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using static ArnoldVinkCode.AVEmbedded;

namespace ArnoldVinkCode
{
    public partial class AVAssembly
    {
        public static Assembly AssemblyResolveFile(object sender, ResolveEventArgs args)
        {
            try
            {
                string fileName = args.Name.Split(',')[0] + ".dll";
                string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Resources\\" + fileName;
                Debug.WriteLine("Resolving resource assembly dll: " + fileName);
                return Assembly.LoadFrom(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed resolving assembly dll: " + ex.Message);
                return null;
            }
        }

        public static Assembly AssemblyResolveEmbedded(object sender, ResolveEventArgs args)
        {
            try
            {
                string fileName = args.Name.Split(',')[0] + ".dll";
                string assemblyPath = Assembly.GetEntryAssembly().GetName().Name + ".Assembly." + fileName;
                byte[] fileBytes = EmbeddedResourceToBytes(assemblyPath);
                Debug.WriteLine("Resolving embedded assembly dll: " + assemblyPath);
                return Assembly.Load(fileBytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed resolving assembly dll: " + ex.Message);
                return null;
            }
        }
    }
}