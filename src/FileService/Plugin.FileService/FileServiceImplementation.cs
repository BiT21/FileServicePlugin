using Plugin.FileService.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

//FileServiceImplementation
namespace Plugin.FileService
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class FileServiceImplementation : FileServiceBase, IFileService
    {

#if PORTABLE

        /// <summary>
        /// Directory.CreateDirectory
        /// </summary>
        /// <param name="folder"></param>
        protected override void DirectoryCreateDirectory(string folder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Directory.Delete
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bRecursive"></param>
        protected override void DirectoryDelete(string filePath, bool bRecursive)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Directory.EnumerateFiles
        /// </summary>
        /// <param name="documentsPath"></param>
        /// <returns></returns>
        protected override IEnumerable<string> DirectoryEnumerateFiles(string documentsPath)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Directory.Exists
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        protected override bool DirectoryExists(string folder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Directory.GetFiles
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override string[] DirectoryGetFiles(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encoding.UTF8.GetString
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected override string EncodingUTF8GetString(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Environment.GetFolderPath
        /// </summary>
        /// <returns></returns>
        protected override string EnvironmentGetFolderPath()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.Delete
        /// </summary>
        /// <param name="file"></param>
        protected override void FileDelete(string file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.Exists
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override bool FileExists(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.GetCreationTime
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override DateTime FileGetCreationTime(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.OpenText
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override StreamReader FileOpenText(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.ReadAllBytes
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected override byte[] FileReadAllBytes(string filename)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.ReadAllText
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override string FileReadAllText(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.SetAttributesNormal
        /// </summary>
        /// <param name="file"></param>
        protected override void FileSetAttributesNormal(string file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.WriteAllBytes
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="byteArray"></param>
        protected override void FileWriteAllBytes(string filename, byte[] byteArray)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// File.WriteAllText
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="result"></param>
        protected override void FileWriteAllText(string filePath, string result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Path.GetTempPath
        /// </summary>
        /// <returns></returns>
        protected override string PathGetTempPath()
        {
            throw new NotImplementedException();
        }
#else
        /// <summary>
        /// Return the root folder where the plugin will save files
        /// </summary>
        /// <returns></returns>
        protected override string EnvironmentGetFolderPath()
        {
            string ret;
#if WINDOWS_UWP
            
            ret = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
#else
            ret =  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
            Trace(ret);

            return ret;
        }

        /// <summary>
        /// Directory.Exists
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        protected override bool DirectoryExists(string folder)
        {
            return Directory.Exists(folder);
        }

        /// <summary>
        /// Directory.CreateDirectory
        /// </summary>
        /// <param name="folder"></param>
        protected override void DirectoryCreateDirectory(string folder)
        {
            Directory.CreateDirectory(folder);
        }

        /// <summary>
        /// File.Exists
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        /// <summary>
        /// File.Delete
        /// </summary>
        /// <param name="file"></param>
        protected override void FileDelete(string file)
        {
            System.IO.File.Delete(file);
        }

        /// <summary>
        /// File.WriteAllText
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="result"></param>
        protected override void FileWriteAllText(string filePath, string result)
        {
            System.IO.File.WriteAllText(filePath, result);
        }

        /// <summary>
        /// File.ReadAllText
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override string FileReadAllText(string filePath)
        {
            return System.IO.File.ReadAllText(filePath);
        }
        
        /// <summary>
        /// Directory.GetFiles
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override string[] DirectoryGetFiles(string filePath)
        {
            return Directory.GetFiles(filePath);
        }

        /// <summary>
        /// File.SetAttributesNormal
        /// </summary>
        /// <param name="file"></param>
        protected override void FileSetAttributesNormal(string file)
        {
            System.IO.File.SetAttributes(file, FileAttributes.Normal);
        }

        /// <summary>
        /// Directory.Delete
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bRecursive"></param>
        protected override void DirectoryDelete(string filePath, bool bRecursive)
        {
            Directory.Delete(filePath, bRecursive);
        }

        /// <summary>
        /// File.GetCreationTime
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override DateTime FileGetCreationTime(string filePath)
        {
            return System.IO.File.GetCreationTime(filePath);
        }

        /// <summary>
        /// File.WriteAllBytes
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="byteArray"></param>
        protected override void FileWriteAllBytes(string filename, byte[] byteArray)
        {
            System.IO.File.WriteAllBytes(filename, byteArray);
        }

        /// <summary>
        /// Path.GetTempPath
        /// </summary>
        /// <returns></returns>
        protected override string PathGetTempPath()
        {
            return Path.GetTempPath();
        }

        /// <summary>
        /// File.ReadAllBytes
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected override byte[] FileReadAllBytes(string filename)
        {
            return System.IO.File.ReadAllBytes(filename);
        }

        /// <summary>
        /// Directory.EnumerateFiles
        /// </summary>
        /// <param name="documentsPath"></param>
        /// <returns></returns>
        protected override IEnumerable<string> DirectoryEnumerateFiles(string documentsPath)
        {
            return Directory.EnumerateFiles(documentsPath);
        }

        /// <summary>
        /// Encoding.UTF8.GetString
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected override string EncodingUTF8GetString(byte[] buffer)
        {
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// File.OpenText
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override StreamReader FileOpenText(string path)
        {
            return System.IO.File.OpenText(path);
        }
#endif
        }
    }