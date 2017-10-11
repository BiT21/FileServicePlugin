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
        public string SandboxTag { get; set; }

        async Task IFileService.SaveObjectFileAsync<T>(string fileName, T content, string contentFolder )
        {
            await Task.Run(() =>
            {
                var filePath = GetAndCreatePath(contentFolder, fileName);

                if (FileExists(filePath))
                {
                    FileDelete(filePath);
                }

                //DateTime dt = TraceCallOut("SaveAsync", "Saving data to : " + filePath);

                string result = JsonConvert.SerializeObject(content,Formatting.Indented);
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
                    if (FileExist(fileName, contentFolder))
                    {
                        var filePath = GetPath(contentFolder, fileName);

                        string result = FileReadAllText(filePath);

                        if (result.Equals("{}") || string.IsNullOrWhiteSpace(result))
                        {
                            //TraceCallReturn("Exist. File empty : " + filePath, dt);
                            return default(TResponse);
                        }
                        else
                        {
                            //TraceCallReturn("Exist. Retreive content: " + filePath, dt);
                            return  JsonConvert.DeserializeObject<TResponse>(result);
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

        async Task IFileService.DeleteFileAsync(string fileName, string contentFolder )
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

                DeleteFolder(folderName);                
            });
            
        }

        async Task IFileService.DeleteSandbox()
        {
            await Task.Run(() => 
            {
                var root = GetPath();

                DeleteFolder(root);

            });
        }
        
        async Task<bool> IFileService.ExistRecentCacheAsync(string fileName, int cacheTime, string contentFolder )
        {
            bool ret = false;

            await  Task.Run(() => 
            {
                var filePath = GetPath(contentFolder, fileName);

                if (FileExists(filePath))
                {
                    var creationTime = FileGetCreationTime(filePath);

                    ret = creationTime.Add(new TimeSpan(cacheTime, 0, 0)) > DateTime.Now;
                }
                else
                {
                    ret = false;
                }

                Trace("ExistRecentCache: [" + ret.ToString().ToUpper() + "] " + filePath);
            });
            return ret;
        }
        
        async Task IFileService.SaveFileAsync(byte[] byteArray, string fileName, string contentFolder)
        {
            await Task.Run(() =>
            {
                var filePath = GetAndCreatePath(contentFolder, fileName);

                if (string.IsNullOrWhiteSpace(contentFolder) && !DirectoryExists(contentFolder))
                {
                    DirectoryCreateDirectory(contentFolder);
                }

                Trace("SaveFile: " + filePath);

                FileWriteAllBytes(filePath, byteArray);
            });
            
        }

        async Task IFileService.SaveFileTemporalAsync(byte[] byteArray, string fileName)
        {
            await Task.Run(() => 
            {
                var foldertemp = PathGetTempPath();
                var directoryname = Path.Combine(foldertemp, fileName);
                if (!DirectoryExists(directoryname))
                {
                    if (fileName != "")
                    {
                        Trace("SaveFileTemporal:" + directoryname);

                        FileWriteAllBytes(directoryname, byteArray);
                    }
                }
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

        async Task<bool> IFileService.FileExistAsync(string fileName, string contentFolder )
        {
            bool ret = false;

            await Task.Run(() => 
            {
                ret = FileExist(fileName, contentFolder);
            });

            return ret;
        }

        async Task<List<string>> IFileService.GetFilesNamesAsync(string contentFolder )
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

        async Task<string> IFileService.ReadTextFileAsync(string filename, string contentFolder )
        {
            var ret = string.Empty;
            await Task.Run(async ()=>
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
                
        async Task IFileService.SaveTextFileAsync(string filename, string data, string contentFolder )
        {
            await Task.Run(() =>
            {
                var path = GetAndCreatePath(contentFolder, filename);
                Trace("Write: " + path);

                FileWriteAllText(path, data);
            });
        }
              
        private string GetPath(string folder=null, string fileName=null)
        {
            if (string.IsNullOrWhiteSpace(SandboxTag))
                throw new ArgumentNullException("SandboxTag", "Please set Plugin.FileService.CrossFileService.Current.SandboxTag to a value before making any call to FileService");

            var final = Path.Combine(EnvironmentGetFolderPath(), "Plugin.FileService." + SandboxTag);

            final = string.IsNullOrWhiteSpace(folder) ? final : Path.Combine(final, folder);

            final = string.IsNullOrWhiteSpace(fileName) ? final : Path.Combine(final, fileName);

            return final;
        }

        private string GetAndCreatePath(string folder=null, string fileName = null)
        {
            var folderPath = GetPath(folder);

            if (!DirectoryExists(folderPath))
                DirectoryCreateDirectory(folderPath);

            return GetPath(folder, fileName);
        }

        private bool FileExist(string fileName, string contentFolder)
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


        //////           Tracing           //////
        const string TRACETAG = "FileService.";
        /// <summary>
        /// Trace msg using func to format the trace output.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        protected void Trace(string msg, [CallerMemberName] string func = "<Empty>")
        {
            //dbgService.TraceInfo(msg, TraceTag + func);
            Debug.WriteLine($"{func} | {msg}");
        }

        /// <summary>
        /// Trace as verbose
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        protected void TraceVerbose(string msg, [CallerMemberName] string func = "<Empty>")
        {
            //dbgService.TraceVerbose(msg, TraceTag + func);
            Trace(msg, func);
        }

        /// <summary>
        /// Trace as Error
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        protected void TraceError(string msg, [CallerMemberName] string func = "<Empty>")
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
        protected void TraceError(string msg, Exception ex, [CallerMemberName] string func = "<Empty>")
        {
            //dbgService.TraceError(msg, ex, TraceTag + func);
            Trace(msg +"\n\n" + ex.ToString(), func);

        }

        /// <summary>
        /// Starts a TraceCallOut/TraceCallReturn tracing.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        /// <returns>Return the actual TimeDate.</returns>
        /// <remarks>The return is intended to be feeded into TraceCAllReturn to allow calculation of time laps.</remarks>
        //protected DateTime TraceCallOut(string msg, [CallerMemberName] string func = "<Empty>")
        //{
        //    //return dbgService.TraceCallOut(func, TraceTag + func);
        //    return DateTime.Now;
        //}

        /// <summary>
        /// Starts a TraceCallOut/TraceCallReturn tracing.
        /// </summary>
        /// <param name="msg">Recomended it to feed the same string as TraceCallOut for a better read of the log.</param>
        /// <param name="dt">Feed the output of TraceCallOut to allow tracing the time laps between calls.</param>
        /// <param name="func"></param>
        //protected void TraceCallReturn(string msg, DateTime dt, [CallerMemberName] string func = "<Empty>")
        //{
        //    //dbgService.TraceCallReturn(func, TraceTag + func, dt);
        //}

        //////           Abstracts functions           //////              
        /// <summary>
        /// File.GetCreationTime
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected abstract DateTime FileGetCreationTime(string filePath);

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