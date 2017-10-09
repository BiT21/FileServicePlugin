using Plugin.FileService.Plugin.Abstractions;
using System;

namespace Plugin.FileService.Plugin
{
  /// <summary>
  /// Cross platform FileService.Plugin implemenations
  /// </summary>
  public class CrossFileService.Plugin
  {
    static Lazy<IFileService.Plugin> Implementation = new Lazy<IFileService.Plugin>(() => CreateFileService.Plugin(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IFileService.Plugin Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IFileService.Plugin CreateFileService.Plugin()
    {
#if PORTABLE
        return null;
#else
        return new FileService.PluginImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
