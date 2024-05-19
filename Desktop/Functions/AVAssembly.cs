using System;
using System.Diagnostics;
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
                fileName = fileName.Replace(".resources.dll", ".dll");
                string filePath = AVFunctions.ApplicationPathRoot() + "\\Resources\\" + fileName;
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
                fileName = fileName.Replace(".resources.dll", ".dll");
                string assemblyPath = AVFunctions.ApplicationName() + ".Assembly." + fileName;
                byte[] fileBytes = EmbeddedResourceToBytes(null, assemblyPath);
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