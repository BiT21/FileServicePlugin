using Newtonsoft.Json;
using Plugin.FileService.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.FileService
{
    /// <summary>
    /// Abstract class with common logic for platform specific FileService class.
    /// </summary>
    public abstract class FileServiceBase : IFileService
    {
        /// <summary>
        /// Holds the Sandbox identifier for all FileService operations.
        /// </summary>
        public string SandboxTag { get; set; }

        async Task IFileService.SaveObjectFileAsync<T>(T content, string fileName, string contentFolder)
        {
            await Task.Run(() =>
            {
                var filePath = GetAndCreatePath(contentFolder, fileName);

                if (FileExists(filePath))
                {
                    FileDelete(filePath);
                }

                //DateTime dt = TraceCallOut("SaveAsync", "Saving data to : " + filePath);

                string result = JsonConvert.SerializeObject(content, Formatting.Indented);
                FileWriteAllText(filePath, result);

                //TraceCallReturn("Saving data to : " + filePath, dt);

            });
        }

        async Task<TResponse> IFileService.ReadObjectFileAsync<TResponse>(string fileName, string contentFolder)
        {
            //DateTime dt = TraceCallOut("LoadAsync", "Loading data from fileName : " + fileName);
            try
            {
                return await Task.Run(() =>
                {
                    if (ExistsFile(fileName, contentFolder))
                    {
                        var filePath = GetPath(contentFolder, fileName);

                        string result = FileReadAllText(filePath);

                        if (string.IsNullOrWhiteSpace(result) || result.Equals("[]") || result.Equals("null"))
                        {
                            //TraceCallReturn("Exist. File empty : " + filePath, dt);
                            return default(TResponse);
                        }
                        else
                        {
                            //TraceCallReturn("Exist. Retreive content: " + filePath, dt);
                            return JsonConvert.DeserializeObject<TResponse>(result);
                        }
                    }
                    else
                    {
                        //TraceCallReturn("Exist. File does not exist: " + fileName, dt);
                        return default(TResponse);
                    }
                });
            }
            catch
            {

                //TraceCallReturn("Error Exception loading " + fileName + "\n[" + ex.Message + "]", dt);

                return default(TResponse);
            }
        }

        async Task IFileService.DeleteFilesAsync(string contentFolder)
        {
            await Task.Run(() =>
            {
                var documentsPath = GetPath(contentFolder);

                if (DirectoryExists(documentsPath))
                {
                    string[] files = DirectoryGetFiles(documentsPath);
                    foreach (string file in files)
                    {
                        TraceVerbose("Deleting : " + file);

                        FileSetAttributesNormal(file);
                        FileDelete(file);
                    }
                }
            });
        }

        async Task IFileService.DeleteFileAsync(string fileName, string contentFolder)
        {
            await Task.Run(() =>
            {
                var filePath = GetPath(contentFolder, fileName);

                if (FileExists(filePath))
                {
                    FileDelete(filePath);

                    Trace("Delete: " + filePath);
                }
            });
        }

        async Task IFileService.DeleteFolderAsync(string folderName)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(folderName))
                    throw new ArgumentNullException("folderName", "Please specify a folder to delete");

                var folderPath = GetPath(folderName);

                DeleteFolder(folderPath);
            });

        }

        async Task IFileService.DeleteSandboxAsync()
        {
            await Task.Run(() =>
            {
                var root = GetPath();

                DeleteFolder(root);

            });
        }

        async Task<bool> IFileService.ExistRecentCacheAsync(string fileName, TimeSpan cacheValidTime, string contentFolder)
        {
            bool ret = false;

            await Task.Run(() =>
           {
               var filePath = GetPath(contentFolder, fileName);
               DateTime creationTime = DateTime.MinValue;

               if (FileExists(filePath))
               {
                   creationTime = FileGetLastWriteTime(filePath);

                   ret = (creationTime + cacheValidTime) > DateTime.Now;
               }
               else
               {
                   ret = false;
               }

               Trace($"ExistRecentCache: [{ret.ToString().ToUpper()}] {creationTime}+{cacheValidTime} > {DateTime.Now} {filePath}");
           });
            return ret;
        }

        async Task IFileService.SaveByteFileAsync(byte[] byteArray, string fileName, string contentFolder)
        {
            await Task.Run(() =>
            {
                var filePath = GetAndCreatePath(contentFolder, fileName);

                Trace("SaveFile: " + filePath);

                FileWriteAllBytes(filePath, byteArray);
            });

        }

        async Task<byte[]> IFileService.ReadByteFileAsync(string fileName, string contentFolder)
        {
            return await Task.Run<byte[]>(() =>
            {
                var filePath = GetPath(contentFolder, fileName);

                try
                {

                    byte[] dataBytes = null;

                    if (FileExists(filePath))
                    {
                        dataBytes = FileReadAllBytes(filePath);
                        return dataBytes;
                    }
                    else
                        return default(byte[]);
                }
                catch (Exception ex)
                {
                    TraceError("Saving to :" + filePath, ex);
                    return default(byte[]);
                }
            });
        }

        string IFileService.GetFullPath(string contentFolder)
        {
            return GetPath(contentFolder);
        }

        async Task<bool> IFileService.ExistFileAsync(string fileName, string contentFolder)
        {
            bool ret = false;

            await Task.Run(() =>
            {
                ret = ExistsFile(fileName, contentFolder);
            });

            return ret;
        }

        async Task<bool> IFileService.ExistFolderAsync(string contentFolder)
        {
            if (string.IsNullOrWhiteSpace(contentFolder))
                return false;

            var ret = false;
            await Task.Run(() => 
            {
                ret = ExistFolder(contentFolder);
            });

            return ret;
        }

        async Task<bool> IFileService.ExistSandBoxAsync()
        {
            var ret = false;
            await Task.Run(() =>
            {
                ret = ExistFolder(string.Empty);
            });

            return ret;
        }

        async Task<List<string>> IFileService.GetFilesNamesAsync(string contentFolder)
        {
            return await Task.Run(() =>
            {
                var documentsPath = GetPath(contentFolder);

                if (DirectoryExists(documentsPath))
                {
                    var files = DirectoryGetFiles(documentsPath).Select(f => f.Split('/').LastOrDefault());
                    return files.ToList();
                }
                else
                    return new List<string>();
            });
        }

        async Task<string> IFileService.ReadTextFileAsync(string filename, string contentFolder)
        {
            var ret = string.Empty;
            await Task.Run(async () =>
            {
                var path = GetPath(contentFolder, filename);

                if (!FileExists(path))
                {
                    ret = string.Empty;
                }
                else
                {
                    Trace("Load: " + path);

                    using (StreamReader sr = FileOpenText(path))
                        ret = await sr.ReadToEndAsync();
                }
            });

            return ret;
        }

        async Task IFileService.SaveTextFileAsync(string data, string filename, string contentFolder)
        {
            await Task.Run(() =>
            {
                var path = GetAndCreatePath(contentFolder, filename);
                Trace("Write: " + path);

                FileWriteAllText(path, data);
            });
        }

        //////           Privates           //////

        private string GetPath(string folder = null, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(SandboxTag))
                throw new ArgumentNullException("SandboxTag", "Please set Plugin.FileService.CrossFileService.Current.SandboxTag to a value before making any call to FileService");

            var final = Path.Combine(EnvironmentGetFolderPath(), "Plugin.FileService." + SandboxTag);

            final = string.IsNullOrWhiteSpace(folder) ? final : Path.Combine(final, folder);

            final = string.IsNullOrWhiteSpace(fileName) ? final : Path.Combine(final, fileName);

            return final;
        }

        private string GetAndCreatePath(string folder = null, string fileName = null)
        {
            var folderPath = GetPath(folder);

            if (!DirectoryExists(folderPath))
                DirectoryCreateDirectory(folderPath);

            return GetPath(folder, fileName);
        }

        private bool ExistsFile(string fileName, string contentFolder)
        {
            bool ret;

            if (string.IsNullOrEmpty(fileName))
            {
                TraceVerbose("NotFound as fileName is null or emtpu");
                ret = false;
            }
            else
            {
                var filePath = GetPath(contentFolder, fileName);

                ret = FileExists(filePath);

                TraceVerbose((ret ? "Found: " : "Does Not Exist: ") + filePath);
            }

            return ret;
        }

        private bool ExistFolder(string folderName)
        {
            var path = GetPath(folderName);

            return DirectoryExists(path);                
        }

        private void DeleteFolder(string folderPath)
        {
            Trace("DeleteFolder: " + folderPath);

            if (DirectoryExists(folderPath))
            {
                string[] files = DirectoryGetFiles(folderPath);
                foreach (string file in files)
                {
                    FileSetAttributesNormal(file);
                    FileDelete(file);
                }
                DirectoryDelete(folderPath, true);
            }
        }

        //////           Testing           //////

        /// <summary>
        /// For Testing Perposes only. Set the 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <remark>For testing purposes.</remark>
        public async Task SetCacheCreation(DateTime dateTime, string fileName, string contentFolder = null)
        {
            await Task.Run(() =>
            {
                var path = GetPath(fileName, contentFolder);
                FileSetLastWriteTime(path, dateTime);
            });
        }
        /// <summary>
        /// Returns the date when the Cache file was last updated. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <returns>File last update date</returns>
        /// <remark>For testing purposes.</remark>
        public async Task<DateTime> GetCacheCreation(string fileName, string contentFolder = null)
        {
            DateTime dt = DateTime.MinValue;

            await Task.Run(() =>
            {
                var path = GetPath(fileName, contentFolder);
                dt = FileGetLastWriteTime(path);
            });

            return dt;
        }

        //////           Tracing           //////

        const string TRACETAG = "FileService.";
        /// <summary>
        /// Trace msg using func to format the trace output.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        private void Trace(string msg, [CallerMemberName] string func = "<Empty>")
        {
            //dbgService.TraceInfo(msg, TraceTag + func);
            Debug.WriteLine($"{func} | {msg}");
        }

        /// <summary>
        /// Trace as verbose
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        private void TraceVerbose(string msg, [CallerMemberName] string func = "<Empty>")
        {
            //dbgService.TraceVerbose(msg, TraceTag + func);
            Trace(msg, func);
        }

        /// <summary>
        /// Trace as Error
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        private void TraceError(string msg, [CallerMemberName] string func = "<Empty>")
        {
            //dbgService.TraceError(msg, TraceTag + func);
            Trace(msg, func);

        }

        /// <summary>
        /// Trace as Error
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        /// <param name="func"></param>
        private void TraceError(string msg, Exception ex, [CallerMemberName] string func = "<Empty>")
        {
            //dbgService.TraceError(msg, ex, TraceTag + func);
            Trace(msg + "\n\n" + ex.ToString(), func);

        }

        //////           Abstracts functions           //////
        
        /// <summary>
        /// File.GetCreationTime
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected abstract DateTime FileGetLastWriteTime(string filePath);

        /// <summary>
        /// File.SetCreationTime
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dateTime"></param>
        protected abstract void FileSetLastWriteTime(string filePath, DateTime dateTime);

        /// <summary>
        /// File.WriteAllBytes
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="byteArray"></param>
        protected abstract void FileWriteAllBytes(string filename, byte[] byteArray);

        /// <summary>
        /// Path.GetTempPath
        /// </summary>
        /// <returns></returns>
        protected abstract string PathGetTempPath();

        /// <summary>
        /// File.ReadAllBytes
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected abstract byte[] FileReadAllBytes(string filename);

        /// <summary>
        /// Directory.EnumerateFiles
        /// </summary>
        /// <param name="documentsPath"></param>
        /// <returns></returns>
        protected abstract IEnumerable<string> DirectoryEnumerateFiles(string documentsPath);

        /// <summary>
        /// Encoding.UTF8.GetString
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected abstract string EncodingUTF8GetString(byte[] buffer);

        /// <summary>
        /// File.OpenText
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected abstract StreamReader FileOpenText(string path);

        /// <summary>
        /// File.WriteAllText
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="result"></param>
        protected abstract void FileWriteAllText(string filePath, string result);

        /// <summary>
        /// File.Delete
        /// </summary>
        /// <param name="file"></param>
        protected abstract void FileDelete(string file);

        /// <summary>
        /// File.Exists
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected abstract bool FileExists(string filePath);

        /// <summary>
        /// Directory.CreateDirectory
        /// </summary>
        /// <param name="folder"></param>
        protected abstract void DirectoryCreateDirectory(string folder);

        /// <summary>
        /// Directory.Exists
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        protected abstract bool DirectoryExists(string folder);

        /// <summary>
        /// Environment.GetFolderPath
        /// </summary>
        /// <returns></returns>
        protected abstract string EnvironmentGetFolderPath();

        /// <summary>
        /// File.ReadAllText
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected abstract string FileReadAllText(string filePath);

        /// <summary>
        /// Directory.GetFiles
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected abstract string[] DirectoryGetFiles(string filePath);

        /// <summary>
        /// Directory.Delete
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bRecursive"></param>
        protected abstract void DirectoryDelete(string filePath, bool bRecursive);

        /// <summary>
        /// File.SetAttributesNormal
        /// </summary>
        /// <param name="file"></param>
        protected abstract void FileSetAttributesNormal(string file);
    }
}