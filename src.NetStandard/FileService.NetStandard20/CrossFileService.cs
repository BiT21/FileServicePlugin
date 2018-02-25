﻿using BiT21.FileService.IService;
using BiT21.FileService.Service;
using System;

namespace BiT21.FileService
{
  /// <summary>
  /// Cross platform FileService implementations
  /// </summary>
  public class CrossFileService
  {
    static Lazy<IFileService> implementation = new Lazy<IFileService>(() => CreateFileService(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IFileService Current
    {
      get
      {
         var ret = implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IFileService CreateFileService()
    {
        return new FileServiceImplementation();
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
