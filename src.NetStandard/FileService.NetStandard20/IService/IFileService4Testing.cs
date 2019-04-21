using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BiT21.FileService.IService
{
    /// <summary>
    /// Interface for testing IFileService class.
    /// </summary>
    public interface IFileService4Testing
    {
        /// <summary>
        /// Retrieve when the file was last written.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task<DateTime> GetCacheCreation(string fileName, string contentFolder = null);

        /// <summary>
        /// For Testing Purposes only. Set last write time of the file.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        Task SetCacheCreation(DateTime dateTime, string fileName, string contentFolder = null);
    }
}
