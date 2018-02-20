using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileServiceNS
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
		/// Read from file system the file expecting TResponse object class.
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
		/// Read a byteArray from fileName in contentFolder.
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
		/// <remarks>If file exist it will be overwrite.</remarks>
		Task SaveObjectFileAsync<T>(T content, string fileName, string contentFolder = null);

		/// <summary>
		/// Retrieve the root folder where the plugin saves files.
		/// </summary>
		/// <param name="contentFolder"></param>
		/// <returns>The effective path is the system personal folder. To it we can add contentFolder</returns>
		string GetFullPath(string contentFolder = null);

		/// <summary>
		/// Retrieve the list of files from the given folder.
		/// </summary>
		/// <param name="contentFolder"></param>
		/// <returns></returns>
		Task<List<string>> GetFilesNamesAsync(string contentFolder = null);

		/// <summary>
		/// Delete the content of the Sandbox managed by FileService.Current
		/// </summary>
		/// <returns></returns>
		Task DeleteSandboxAsync();

		/// <summary>
		/// Delete the entire folder.
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
