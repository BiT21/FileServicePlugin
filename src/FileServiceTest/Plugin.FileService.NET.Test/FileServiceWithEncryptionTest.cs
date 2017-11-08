using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plugin.EncryptDecrypt.Abstractions;
using Plugin.FileService.Abstractions;
using System.Threading.Tasks;

namespace Plugin.FileService.NET.Test
{
    [TestClass]
    public class FileServiceWithEncryptionTest
    {
        const string SANDBOX_TAG = "TestingSandBox";
        const string TEXT_PADDING = "Testing text used to confirm that FileService works saving and reading text files. \naba29448-a930-4df7-9195-a9340b0ce6a0";
        IFileService fileService;

        const string PASSWORD = "password1234";
        IEncryptDecrypt encryptedDecrypt;

        [TestInitialize]
        public void Setup()
        {
            fileService = FileService.CrossFileService.Current;
            fileService.SandboxTag = SANDBOX_TAG;

            encryptedDecrypt = EncryptDecrypt.CrossEncryptDecrypt.Current;
        }

        [TestMethod]
        public async Task Save_Read_Encrypted_TextFileAsync_Test()
        {
            var filename = "Save_Read_TextFileAsync_Test";
            var content = "Testing text used to confirm that FileService works saving and reading text files. \n" + Guid.NewGuid().ToString();

            var contentEncrypted = await encryptedDecrypt.EncryptStringAsync(PASSWORD, content);

            await fileService.SaveTextFileAsync(contentEncrypted, filename);

            var ret = await fileService.ReadTextFileAsync(filename);

            var retDecrypt = await encryptedDecrypt.DecryptStringAsync(PASSWORD, ret);

            Assert.AreEqual(content, retDecrypt);
        }
    }
}
