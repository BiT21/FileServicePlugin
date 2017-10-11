using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Plugin.FileService.Abstractions;
using System.Diagnostics;

namespace Plugin.FileService.NET.Test
{
    [TestClass]
    public class FileServiceTest
    {
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void GetFullPathEmpty_Test()
        {
            var ret = CrossFileService.Current.GetFullPath();
        }

        [TestMethod]
        public async Task SaveTextFileAsync_Test()
        {
            IFileService fileService = Plugin.FileService.CrossFileService.Current;
            fileService.InstanceTag = "Testingfolder";

            var filename = "filename";
            var content = "this is padding and a face set of string to test";

            await fileService.SaveTextFileAsync(filename, content);

            var ret2 = fileService.GetFullPath("sample");

            Debug.WriteLine(ret2);

            var ret = await fileService.ReadTextFileAsync(filename);

            Assert.AreEqual(content, ret);
        }

    }
}
