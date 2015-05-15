using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Resources;

namespace NoteOne_Core.Resources
{
    public class ResourcesLoader
    {
        private static ResourcesLoader _loader;

        private readonly ResourceLoader resourceLoader;

        private ResourcesLoader()
        {
#if !WIN8
            resourceLoader = ResourceLoader.GetForViewIndependentUse(@"NoteOne_Core/Resources");
#else
            resourceLoader = new ResourceLoader(@"NoteOne_Core/Resources"); // only work for Windows 8 project
#endif
        }

        public static ResourcesLoader Loader
        {
            get
            {
                if (_loader == null)
                    _loader = new ResourcesLoader();
                return _loader;
            }
        }

        public string this[string name]
        {
            get
            {
                string result = resourceLoader.GetString(name);
                if (!string.IsNullOrEmpty(result))
                    return result;
                else
                    return name;
            }
        }
    }

    public static class ResourceLoadersupportExtesion
    {
        // Object Extensions
        public static string GetDescriptionViaResources<T>(this T value)
        {
            return ResourcesLoader.Loader[value.GetDescription()];
        }
    }
}