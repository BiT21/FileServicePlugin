using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Plugin.FileService.Abstractions;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Plugin.FileService.NET.Test
{
    [TestClass]
    public class FileServiceTest
    {
        const string SANDBOX_TAG = "TestingSandBox";
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
            await fileService.DeleteSandbox();
        }        

        [TestMethod]
        public async Task Save_Read_TextFileAsync_Test()
        {
            var filename = "Save_Read_TextFileAsync_Test";
            var content = "Testing text used to confirm that FileService works saving and reading text files. \n" + Guid.NewGuid().ToString();

            await fileService.SaveTextFileAsync(filename, content);

            var ret2 = fileService.GetFullPath("sample");

            Trace.WriteLine(ret2);

            var ret = await fileService.ReadTextFileAsync(filename);

            Assert.AreEqual(content, ret);
        }

        [TestMethod]
        public async Task Save_Read_TextFileAsync_WithFolder_Test()
        {
            var filename = "Save_Read_TextFileAsync_Test";
            var content = "Testing text used to confirm that FileService works saving and reading text files. \n" + Guid.NewGuid().ToString();
            var folderName = "Save_Read_FolderName";

            await fileService.SaveTextFileAsync(filename, content,folderName);

            var ret2 = fileService.GetFullPath("sample");

            Trace.WriteLine(ret2);

            var ret = await fileService.ReadTextFileAsync(filename,folderName);

            Assert.AreEqual(content, ret);
        }

        [TestMethod]
        public async Task Save_Read_ObjectFileAsync_Object_Test()
        {
            var o = SimpleObject.GetObject();
            var filename = "SaveObjectFileAsync_Test";

            await fileService.SaveObjectFileAsync(filename,o);

            var retO = await fileService.ReadObjectFileAsync<SimpleObject>(filename);
                       
            Assert.AreEqual(o, retO);
        }

        [TestMethod]
        public async Task Save_Read_ObjectFileAsync_ObjectList_Test()
        {
            var list = SimpleObject.GetObjectList();
            var filename = "SaveObjectFileAsync_Test2";

            await fileService.SaveObjectFileAsync(filename, list);

            var retList = await fileService.ReadObjectFileAsync<List<SimpleObject>>(filename);

            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i], retList[i]);
            }
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void GetFullPathEmpty_Test()
        {
            fileService.SandboxTag = string.Empty;
            var ret = CrossFileService.Current.GetFullPath();
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
