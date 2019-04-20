using BiT21.FileService.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BiT21.FileService.Service
{
    /// <summary>
    /// Class implementation of the <see cref="IFileService"/> abstraction.
    /// </summary>
	public class FileServiceImplementation : IFileService, IFileService4Testing
    {
        private string sandboxTag;
        private readonly string rootFolder;
        const Environment.SpecialFolder DEFAULT_ENVIRONMENT_SPECIALFOLDER = Environment.SpecialFolder.LocalApplicationData;

        /// <summary>
        /// Property to define the sandbox folder where this FileService instance will work on.
        /// </summary>
		public string SandboxTag {
            get
            {
                if (string.IsNullOrWhiteSpace(sandboxTag))
                {
                    throw new ArgumentNullException("SandboxTag");
                }
                return sandboxTag;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(sandboxTag))
                {
                    throw new Exception("SandboxTag already set to a value");
                }
                sandboxTag = value;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="sandboxTag"></param>
        public FileServiceImplementation(string sandboxTag) : 
            this(sandboxTag,DEFAULT_ENVIRONMENT_SPECIALFOLDER)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="sandboxTag"></param>
        /// <param name="specialFolder"></param>
        public FileServiceImplementation(string sandboxTag, Environment.SpecialFolder specialFolder) 
        {
            try
            {
                this.sandboxTag = sandboxTag;

                this.rootFolder = Environment.GetFolderPath(specialFolder, Environment.SpecialFolderOption.Create);
                
                //This will throw if the final folder is not correct.
                var finalRootFolder = GetAndCreatePath();

                this.TraceVerbose($"Created file service for sandbox folder : {finalRootFolder}");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Please check sandboxTag that do not contain invalid characters or that the Environment.SpecialFolder used is supported on your platform.", ex);
            }
        }

        async Task IFileService.SaveObjectFileAsync<T>(T content, string fileName, string contentFolder)
        {
            await Task.Run(() =>
            {
                var filePath = GetAndCreatePath(contentFolder, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                //DateTime dt = TraceCallOut("SaveAsync", "Saving data to : " + filePath);

                string result = JsonConvert.SerializeObject(content, Formatting.Indented);
                File.WriteAllText(filePath, result);

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

                        string result = File.ReadAllText(filePath);

                        if (string.IsNullOrWhiteSpace(result) || result.Equals("[]") || result.Equals("null"))
                        {
                            //TraceCallReturn("Exist. File empty : " + filePath, dt);
                            return default(TResponse);
                        }
                        else
                        {
                            //TraceCallReturn("Exist. Retrieve content: " + filePath, dt);
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

                if (Directory.Exists(documentsPath))
                {
                    string[] files = Directory.GetFiles(documentsPath);
                    foreach (string file in files)
                    {
                        TraceVerbose("Deleting : " + file);

                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                }
            });
        }

        async Task IFileService.DeleteFileAsync(string fileName, string contentFolder)
        {
            await Task.Run(() =>
            {
                var filePath = GetPath(contentFolder, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

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

                if (File.Exists(filePath))
                {
                    creationTime = File.GetLastWriteTime(filePath);

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

                File.WriteAllBytes(filePath, byteArray);
            });

        }

        async Task<byte[]> IFileService.ReadByteFileAsync(string fileName, string contentFolder)
        {
            return await Task.Run<byte[]>(() =>
            {
                string filePath= string.Empty;

                try
                {
                    filePath = GetPath(contentFolder, fileName);

                    byte[] dataBytes = null;

                    if (File.Exists(filePath))
                    {
                        dataBytes = File.ReadAllBytes(filePath);
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

        async Task<bool> IFileService.ExistFolderAsync(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                TraceError("Folder may not be null or empty.");
                return false;
            }

            var ret = false;
            await Task.Run(() =>
            {
                ret = ExistFolder(folder);
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

                if (Directory.Exists(documentsPath))
                {
                    var files = Directory.GetFiles(documentsPath).Select(f => f.Split('/').LastOrDefault());
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

                if (!File.Exists(path))
                {
                    ret = string.Empty;
                }
                else
                {
                    Trace("Load: " + path);

                    using (StreamReader sr = File.OpenText(path))
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

                File.WriteAllText(path, data);
            });
        }

        //////           Privates           //////

        private string GetPath(string folder = null, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(SandboxTag))
                throw new ArgumentNullException("SandboxTag", "Please set Plugin.FileService.CrossFileService.Current.SandboxTag to a value before making any call to FileService");

            var final = Path.Combine(rootFolder, "BiT21.FileService", SandboxTag);

            final = string.IsNullOrWhiteSpace(folder) ? final : Path.Combine(final, folder);

            final = string.IsNullOrWhiteSpace(fileName) ? final : Path.Combine(final, fileName);

            return Path.GetFullPath(final);
        }

        private string GetAndCreatePath(string folder = null, string fileName = null)
        {
            var folderPath = GetPath(folder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return GetPath(folder, fileName);
        }

        private bool ExistsFile(string fileName, string contentFolder)
        {
            bool ret;

            if (string.IsNullOrEmpty(fileName))
            {
                TraceVerbose("NotFound as fileName is null or empty");
                ret = false;
            }
            else
            {
                var filePath = GetPath(contentFolder, fileName);

                ret = File.Exists(filePath);

                TraceVerbose((ret ? "Found: " : "Does Not Exist: ") + filePath);
            }

            return ret;
        }

        private bool ExistFolder(string folderName)
        {
            var path = GetPath(folderName);

            return Directory.Exists(path);
        }

        private void DeleteFolder(string folderPath)
        {
            Trace("DeleteFolder: " + folderPath);

            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                Directory.Delete(folderPath, true);
            }
        }

        //////           Testing           //////

        /// <summary>
        /// For Testing Purposes only. Set the 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        async Task IFileService4Testing.SetCacheCreation(DateTime dateTime, string fileName, string contentFolder)
        {
            await Task.Run(() =>
            {
                var path = GetPath(fileName, contentFolder);
                File.SetLastWriteTime(path, dateTime);
            });
        }

        /// <summary>
        /// Retrieve when the file was last written.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentFolder"></param>
        /// <returns></returns>
        async Task<DateTime> IFileService4Testing.GetCacheCreation(string fileName, string contentFolder)
        {
            DateTime dt = DateTime.MinValue;

            await Task.Run(() =>
            {
                var path = GetPath(fileName, contentFolder);
                dt = File.GetLastWriteTime(path);
            });

            return dt;
        }

        //////           Tracing           //////

        const string TRACETAG = "FileService";

        /// <summary>
        /// Trace msg using func to format the trace output.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        private void Trace(string msg, [CallerMemberName] string func = "<Empty>")
        {
            System.Diagnostics.Trace.TraceInformation($"{func} | {msg}");
        }

        /// <summary>
        /// Trace as verbose
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        private void TraceVerbose(string msg, [CallerMemberName] string func = "<Empty>")
        {
            Trace(msg, func);
        }

        /// <summary>
        /// Trace as Error
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="func"></param>
        private void TraceError(string msg, [CallerMemberName] string func = "<Empty>")
        {
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
            Trace(msg + "\n\n" + ex.ToString(), func);
        }
    }
}
