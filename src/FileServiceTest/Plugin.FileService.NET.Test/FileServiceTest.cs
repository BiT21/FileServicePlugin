using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Plugin.FileService.Abstractions;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Plugin.FileService.NET.Test
{
    [TestClass]
    public class FileServiceTest
    {
        const string SANDBOX_TAG = "TestingSandBox";
        const string TEXT_PADDING = "Testing text used to confirm that FileService works saving and reading text files. \naba29448-a930-4df7-9195-a9340b0ce6a0";
        IFileService fileService;

        [TestInitialize]
        public void Setup()
        {
            fileService = FileService.CrossFileService.Current;
            fileService.SandboxTag = SANDBOX_TAG;
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            Trace.WriteLine();
            fileService.SandboxTag = SANDBOX_TAG;
            await fileService.DeleteSandboxAsync();
            Assert.IsFalse(await fileService.ExistSandBoxAsync());
        }

        [TestMethod]
        public async Task Save_Read_TextFileAsync_Test()
        {
            var filename = "Save_Read_TextFileAsync_Test";
            var content = "Testing text used to confirm that FileService works saving and reading text files. \n" + Guid.NewGuid().ToString();

            await fileService.SaveTextFileAsync(content, filename);

            var ret = await fileService.ReadTextFileAsync(filename);

            Assert.AreEqual(content, ret);
        }

        [TestMethod]
        public async Task Save_Read_TextFileAsync_WithFolder_Test()
        {
            var filename = "Save_Read_TextFileAsync_Test";
            var content = "Testing text used to confirm that FileService works saving and reading text files. \n" + Guid.NewGuid().ToString();
            var folderName = "Save_Read_FolderName";

            await fileService.SaveTextFileAsync(content, filename, folderName);

            var ret = await fileService.ReadTextFileAsync(filename, folderName);

            Assert.AreEqual(content, ret);
        }

        [TestMethod]
        public async Task Read_TextFileAsync_FileNotExist_Test()
        {
            Assert.AreEqual(string.Empty, await fileService.ReadTextFileAsync("fakename5634"));
        }

        [TestMethod]
        public async Task Save_Read_ObjectFileAsync_Object_Test()
        {
            var o = SimpleObject.GetObject();
            var filename = "SaveObjectFileAsync_Test";

            await fileService.SaveObjectFileAsync(o, filename);

            var retO = await fileService.ReadObjectFileAsync<SimpleObject>(filename);

            Assert.AreEqual(o, retO);
        }
        [TestMethod]
        public async Task Save_Read_ObjectFileAsync_ObjectAlreadyExists_Test()
        {
            var o = SimpleObject.GetObject();
            var filename = "SaveObjectFileAsync_Test";

            await fileService.SaveObjectFileAsync(o, filename);

            var j = SimpleObject.GetObject();

            await fileService.SaveObjectFileAsync(j, filename);

            var retO = await fileService.ReadObjectFileAsync<SimpleObject>(filename);

            Assert.AreEqual(j, retO);
        }

        [TestMethod]
        public async Task Save_Read_ObjectFileAsync_ObjectList_Test()
        {
            var list = SimpleObject.GetObjectList();
            var filename = "SaveObjectFileAsync_Test2";

            await fileService.SaveObjectFileAsync(list, filename);

            var retList = await fileService.ReadObjectFileAsync<List<SimpleObject>>(filename);

            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i], retList[i]);
            }
        }

        [TestMethod]
        public async Task Read_ObjectFileAsync_Object_FileNotExist_Test()
        {
            Assert.AreEqual(default(SimpleObject), await fileService.ReadObjectFileAsync<SimpleObject>("fakename5634"));
        }

        [TestMethod]
        public async Task Save_Read_ByteFileAsync_Test()
        {
            var filename = "Save_Read_ByteFileAsync_Test";
            var content = Helper.GetByteArray();

            await fileService.SaveByteFileAsync(content, filename);

            var ret = await fileService.ReadByteFileAsync(filename);

            Assert.IsTrue(content.SequenceEqual(ret));
        }

        [TestMethod]
        public async Task Save_Read_ByteFileAsync_WithFolder_Test()
        {
            var filename = "Save_Read_ByteFileAsync_WithFolder_Test";
            var content = Helper.GetByteArray();
            var folderName = "Save_Read_FolderName";

            await fileService.SaveByteFileAsync(content, filename, folderName);

            var ret = await fileService.ReadByteFileAsync(filename, folderName);

            Assert.IsTrue(content.SequenceEqual(ret));
        }

        [TestMethod]
        public async Task Read_ByteFileAsync_Object_FileNotExist_Test()
        {
            Assert.AreEqual(default(byte[]), await fileService.ReadByteFileAsync("fakename5634"));
        }

        [TestMethod]
        public async Task ReadObjectFileAsync_Emptyfile_Test()
        {
            var filename = "ReadObjectFileAsync_filename";

            await fileService.SaveObjectFileAsync<SimpleObject>(null, filename);
            Assert.IsTrue(await fileService.ExistFileAsync(filename));

            var ret = await fileService.ReadObjectFileAsync<SimpleObject>(filename);
            Assert.IsNull(ret);
        }

        [TestMethod]
        public async Task ReadObjectFileAsync_EmptyListOfObject_Test()
        {
            var filename = "ReadObjectFileAsync_filename";

            await fileService.SaveObjectFileAsync<List<SimpleObject>>(null, filename);
            Assert.IsTrue(await fileService.ExistFileAsync(filename));
            var ret1 = await fileService.ReadTextFileAsync(filename);
            Assert.AreEqual("null", ret1);

            var ret = await fileService.ReadObjectFileAsync<List<SimpleObject>>(filename);
            Assert.IsNull(ret);

            await fileService.SaveObjectFileAsync<List<SimpleObject>>(new List<SimpleObject>(), filename);
            Assert.IsTrue(await fileService.ExistFileAsync(filename));

            var ret2 = await fileService.ReadTextFileAsync(filename);
            Assert.AreEqual("[]", ret2);

            var ret3 = await fileService.ReadObjectFileAsync<List<SimpleObject>>(filename);
            Assert.IsNull(ret3);
        }

        [TestMethod]
        public async Task ReadObjectFileAsync_FileDoNotExist_Test()
        {
            var ret = await fileService.ReadObjectFileAsync<List<SimpleObject>>("dasdasd");
            Assert.IsNull(ret);
        }

        [TestMethod]
        public async Task ReadObjectFileAsync_FolderDoNotExist_Test()
        {
            var filename = "ReadObjectFileAsync_filename";
            var foldername = "ReadObjectFileAsync_foldername";
            var o = SimpleObject.GetObject();

            await fileService.SaveObjectFileAsync<SimpleObject>(o, filename);
            Assert.IsTrue(await fileService.ExistFileAsync(filename));

            var ret = await fileService.ReadObjectFileAsync<List<SimpleObject>>(filename, foldername);
            Assert.IsNull(ret);
        }

        [TestMethod]
        public async Task ReadObjectFileAsync_WrongObject_Test()
        {
            var filename = "ReadObjectFileAsync_filename";
            var o = SimpleObject.GetObject();

            await fileService.SaveTextFileAsync(TEXT_PADDING, filename);
            Assert.IsTrue(await fileService.ExistFileAsync(filename));

            var ret = await fileService.ReadObjectFileAsync<SimpleObject>(filename);
            Assert.IsNull(ret);
        }

        [TestMethod]
        public async Task DeleteFileAsync_Test()
        {
            var filename = "DeleteFilesAsync_Filename";

            await fileService.SaveTextFileAsync(TEXT_PADDING, filename);

            Assert.IsTrue(await fileService.ExistFileAsync(filename));

            await fileService.DeleteFileAsync(filename);
            Assert.IsFalse(await fileService.ExistFileAsync(filename));
        }

        [TestMethod]
        public async Task DeleteFileAsyncWithFolder_Test()
        {
            var filename = "DeleteFilesAsync_Filename";
            var folderName = "DeleteFilesAsync_FolderName";

            await fileService.SaveTextFileAsync(TEXT_PADDING, filename, folderName);
            Assert.IsTrue(await fileService.ExistFileAsync(filename, folderName));
            Assert.IsFalse(await fileService.ExistFileAsync(filename));

            await fileService.DeleteFileAsync(filename, folderName);
            Assert.IsFalse(await fileService.ExistFileAsync(filename));
        }

        [TestMethod]
        public async Task DeleteFilesAsync_Test()
        {
            var filename = "DeleteFilesAsync_Filename";

            for (int i = 0; i < 4; i++)
                await fileService.SaveTextFileAsync(TEXT_PADDING, filename + i);

            var fs = await fileService.GetFilesNamesAsync();
            Assert.AreEqual(4, fs.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(fs[i].EndsWith(filename + i));
                Assert.IsTrue(await fileService.ExistFileAsync(filename + i));
            }

            await fileService.DeleteFilesAsync();

            fs = await fileService.GetFilesNamesAsync();
            Assert.AreEqual(0, fs.Count);
            //Assert.IsNull(fs);
            for (int i = 0; i < 4; i++)
                Assert.IsFalse(await fileService.ExistFileAsync(filename + i));
        }

        [TestMethod]
        public async Task DeleteFilesAsyncWithFolder_Test()
        {
            var filename = "DeleteFilesAsync_Filename";
            var folderName = "DeleteFilesAsync_FolderName";


            for (int i = 0; i < 4; i++)
                await fileService.SaveTextFileAsync(TEXT_PADDING, filename + i, folderName);

            var fs = await fileService.GetFilesNamesAsync(folderName);
            Assert.AreEqual(4, fs.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(fs[i].EndsWith(filename + i));
                Assert.IsTrue(await fileService.ExistFileAsync(filename + i, folderName));
            }

            await fileService.DeleteFilesAsync(folderName);

            fs = await fileService.GetFilesNamesAsync(folderName);
            Assert.AreEqual(0, fs.Count);
            //Assert.IsNull(fs);
            for (int i = 0; i < 4; i++)
                Assert.IsFalse(await fileService.ExistFileAsync(filename + i, folderName));
        }

        [TestMethod]
        public async Task DeleteFolderAsync_Test()
        {
            var foldername = "DeleteFolderAsync_Foldername";
            var filename = "DeleteFolderAsync_filename";

            await fileService.SaveTextFileAsync(TEXT_PADDING, filename, foldername);

            Assert.IsTrue(await fileService.ExistFileAsync(filename, foldername));
            Assert.IsTrue(await fileService.ExistFolderAsync(foldername));

            await fileService.DeleteFolderAsync(foldername);
            Assert.IsFalse(await fileService.ExistFileAsync(filename, foldername));
            Assert.IsFalse(await fileService.ExistFolderAsync(foldername));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public async Task DeleteFolderAsync_WithNoFolder_Test()
        {            
            await fileService.DeleteFolderAsync(null);
        }

        [TestMethod]
        public async Task DeleteSandboxAsync_Test()
        {
            var foldername = "DeleteFolderAsync_Foldername";
            var filename = "DeleteFolderAsync_filename";

            await fileService.SaveTextFileAsync(TEXT_PADDING, filename, foldername);

            Assert.IsTrue(await fileService.ExistFileAsync(filename, foldername));
            Assert.IsTrue(await fileService.ExistFolderAsync(foldername));

            await fileService.DeleteSandboxAsync();
            Assert.IsFalse(await fileService.ExistFileAsync(filename, foldername));
            Assert.IsFalse(await fileService.ExistFolderAsync(foldername));
            Assert.IsFalse(await fileService.ExistSandBoxAsync());
        }

        [TestMethod]
        public async Task ExistRecentCacheAsync_Test()
        {
            var fs = (FileServiceImplementation)fileService;
            var filename = "ExistRecentCacheAsync_Filename";
            var safeoffsetSeconds = 2;
            var hoursOffset = 2;
            var offset = new TimeSpan(hoursOffset, 0,0); 

            var now = DateTime.Now;
            await fileService.SaveObjectFileAsync(SimpleObject.GetObject(), filename);

            
            await fs.SetCacheCreation((now - offset).AddSeconds(safeoffsetSeconds),filename);
            Assert.AreEqual(await fs.GetCacheCreation(filename), (now - offset).AddSeconds(safeoffsetSeconds));
            Assert.IsTrue(await fileService.ExistRecentCacheAsync(filename, offset));

            await fs.SetCacheCreation((now -offset).AddSeconds(-safeoffsetSeconds), filename);
            Assert.AreEqual(await fs.GetCacheCreation(filename), (now - offset).AddSeconds(-safeoffsetSeconds));
            Assert.IsFalse(await fileService.ExistRecentCacheAsync(filename, offset));
        }

        [TestMethod]
        public async Task ExistRecentCacheAsync_FileNotExist_Test()
        {
            Assert.IsFalse(await fileService.ExistRecentCacheAsync("fakename21323", new TimeSpan()));
        }

        [TestMethod]
        public async Task GetFilesNamesAsync_FolderNotExist_Test()
        {
            var ret = await fileService.GetFilesNamesAsync("fakefoldername");

            Assert.AreEqual(0, ret.Count);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void GetFullPathEmpty_Test()
        {
            fileService.SandboxTag = string.Empty;
            var ret = CrossFileService.Current.GetFullPath();
        }

        [TestMethod]
        public async Task ExistFile_FileNameNull_Test()
        {
            var ret = await fileService.ExistFileAsync(string.Empty);
            Assert.IsFalse(ret);

            var ret1 = await fileService.ExistFileAsync(null);
            Assert.IsFalse(ret1);
        }
    }

    class Helper
    {
        const string text = "012345678";
        public static  byte[] GetByteArray()
        {
            return text.Select(c=> Convert.ToByte(c)).ToArray();
        }
    }

    class Trace
    {
        public static void WriteLine(string msg = null, [CallerMemberName] string func = "<Empty>")
        {
            System.Diagnostics.Debug.WriteLine($"| {func} | {msg}");
        }
    }
    class SimpleObject
    {
        public int number { get; set; }
        public string name { get; set; }
        public DateTime dateTime { get; set; }

        public static SimpleObject GetObject()
        {
            var o = new SimpleObject();

            o.name = Guid.NewGuid().ToString();
            o.number = Convert.ToInt32(Regex.Replace(o.name, @"[^0-9]+", string.Empty).Substring(0, 5));
            o.dateTime = DateTime.Now.AddMinutes(o.number);

            return o;
        }
        public static List<SimpleObject> GetObjectList()
        {
            var list = new List<SimpleObject>();

            list.Add(GetObject());
            list.Add(GetObject());
            list.Add(GetObject());
            list.Add(GetObject());
            list.Add(GetObject());

            return list;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here
            SimpleObject so = obj as SimpleObject;
            bool ret =
                this.name == so.name &&
                this.number == so.number &&
                this.dateTime == so.dateTime;

            return ret;
            //return base.Equals(obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            //throw new NotImplementedException();
            return base.GetHashCode();
        }

    }
}
