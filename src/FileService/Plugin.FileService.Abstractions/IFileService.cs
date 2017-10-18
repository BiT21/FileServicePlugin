using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.FileService.Abstractions
{
  /// <summary>
  /// Interface for FileService
  /// </summary>
  public interface IFileService
  {
        /// <summary>
        /// Holds the Sandbox identifier for all FileService operations.
        /// </summary>
        string SandboxTag { get; set; }

        /// <summary>
        /// Read from file sistem the file expecting TResponmse object class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <returns>TResponse object or Default if not exist or if file content does not match TResponse.</returns>
        Task<T> ReadObjectFileAsync<T>(string fileName, string contentFolder = null);

        /// <summary>
        /// Read the content of the file as string.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="contentFolder"></param>
        /// <returns>the string content of the file.</returns>
        Task<string> ReadTextFileAsync(string filename, string contentFolder = null);
        
        /// <summary>
        /// Read a byteArry from fileName in contentFolder.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task<byte[]> ReadByteFileAsync(string fileName, string contentFolder = null);

        /// <summary>
        /// Save a text file to disk.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filename"></param>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task SaveTextFileAsync(string content, string filename, string contentFolder = null);

        /// <summary>
        /// Save byteArray for file.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="fileName"></param>
        /// <param name="contentFolder">Base folder to store the file.</param>
        /// <remarks>THe final path will be ContentFolder\Path\fileName</remarks>
        Task SaveByteFileAsync(byte[] byteArray, string fileName, string contentFolder = null);

        /// <summary>
        /// Save to file system the T object content to contentFolder\fileName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        /// <remarks>If file exist it will be overwriten.</remarks>
        Task SaveObjectFileAsync<T>(T content, string fileName, string contentFolder = null);

        /// <summary>
        /// Retreive the root folder where the plugin saves files.
        /// </summary>
        /// <param name="contentFolder"></param>
        /// <returns>The efective path is the system personal folder. To it we can add contentFolder</returns>
        string GetFullPath(string contentFolder=null);

        /// <summary>
        /// Retreive the list of files from the given folder.
        /// </summary>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task<List<string>> GetFilesNamesAsync(string contentFolder = null);

        /// <summary>
        /// Delete the content of the Sandobox managed by FileService.Current
        /// </summary>
        /// <returns></returns>
        Task DeleteSandboxAsync();

        /// <summary>
        /// Delete the entier folder.
        /// </summary>
        /// <param name="folderName"></param>
        Task DeleteFolderAsync(string folderName);

        /// <summary>
        /// Delete contentFolder\fileName if exists
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        Task DeleteFileAsync(string fileName, string contentFolder = null);

        /// <summary>
        /// Delete all the files in contentFolder
        /// </summary>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task DeleteFilesAsync(string contentFolder = null);

        /// <summary>
        /// Check if the sandbox exists.
        /// </summary>
        /// <returns></returns>
        Task<bool> ExistSandBoxAsync();

        /// <summary>
        /// Check if file exist since longer than the given cacheTime in Hours.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cacheValidPeriod">Period through witch cache is valid.</param>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task<bool> ExistRecentCacheAsync(string fileName, TimeSpan cacheValidPeriod, string contentFolder = null);

        /// <summary>
        /// Check if contentFolder\fileName exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task<bool> ExistFileAsync(string fileName, string contentFolder = null);

        /// <summary>
        /// Check if the folder exists in sandbox
        /// </summary>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        Task<bool> ExistFolderAsync(string contentFolder);
    }
}
