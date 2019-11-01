using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArnoldVinkExtensions
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        Assembly GetAssemblyByName(string assemblyName)
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == assemblyName);
            }
            catch { return null; }
        }

        bool CheckIfImageExists(string fileName, Assembly assembly)
        {
            try
            {
                return assembly.GetManifestResourceNames().Any(x => x == fileName);
            }
            catch { return false; }
        }

        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                if (Source == null) { return null; }

                //Set the pcl assembly and asset name
                Assembly imageAssembly = typeof(ImageResourceExtension).GetTypeInfo().Assembly;
                string assemblyName = imageAssembly.GetName().Name;
                string imageSource = assemblyName + "." + Source;

                //Check if platform has it's own image
                imageAssembly = GetAssemblyByName(assemblyName + "." + Device.RuntimePlatform);
                imageSource = imageAssembly.GetName().Name + "." + Source;

                //Workaround for the default android namespace
                imageSource = imageSource.Replace(".Android.", ".Droid.");

                //Check if specific platform image exists
                if (!CheckIfImageExists(imageSource, imageAssembly))
                {
                    imageAssembly = typeof(ImageResourceExtension).GetTypeInfo().Assembly;
                    imageSource = imageAssembly.GetName().Name + "." + Source;
                }

                return ImageSource.FromResource(imageSource, imageAssembly);
            }
            catch
            {
                return null;
            }
        }
    }
}