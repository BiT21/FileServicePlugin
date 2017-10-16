using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Plugin.EncryptDecrypt;

namespace Plugin.FileService.NET.Test
{
    [TestClass]
    public class EncryptDecryptTest
    {
        IEncryptDecrypt encryptDecrypt;

        [TestInitialize]
        public void Setup()
        {
            encryptDecrypt = EncryptDecrypt.CrossEncryptDecrypt.Current;
        }

        [TestMethod]
        public async Task EncryptDecrypt_Test()
        {
            string pwd = "password1234";
            Trace.WriteLine($"Password : {pwd}");


            string data = Guid.NewGuid().ToString();
            Trace.WriteLine($"Data : {data}");

            string e = await encryptDecrypt.EncryptStringAsync(pwd, data);
            Trace.WriteLine($"Encripted : {e}");

            string d = await encryptDecrypt.DecryptStringAsync(pwd, e);
            Trace.WriteLine("Decripted : " + d);

            Assert.AreEqual(data, d);
        }

        [TestMethod]
        public async Task EncryptDecrypt_pwdEmptyTest()
        {
            string pwd = string.Empty;
            Trace.WriteLine($"Password : {pwd}");

            string data = Guid.NewGuid().ToString();
            Trace.WriteLine($"Data : {data}");

            string e = await encryptDecrypt.EncryptStringAsync(pwd, data);
            Trace.WriteLine($"Encripted : {e}");

            string d = await encryptDecrypt.DecryptStringAsync(pwd, e);
            Trace.WriteLine("Decripted : " + d);

            Assert.AreEqual(data, d);

            try
            {
                string d2 = await encryptDecrypt.DecryptStringAsync("kk", e);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "WrongPassword");
            }
            
        }

        //[ExpectedException(typeof(CryptographicException))]
        [TestMethod]
        public async Task EncryptDecrypt_BadPassword_Test()
        {
            string pwd = "password1234";
            Trace.WriteLine($"Password : {pwd}");

            string data = Guid.NewGuid().ToString();
            Trace.WriteLine($"Data : {data}");

            string e = await encryptDecrypt.EncryptStringAsync(pwd, data);
            Trace.WriteLine($"Encripted : {e}");
            
            string d = await encryptDecrypt.DecryptStringAsync(pwd, e);
            Trace.WriteLine("Decripted : " + d);

            Assert.AreEqual(data, d);

            try
            {
                string d2 = await encryptDecrypt.DecryptStringAsync(pwd + "1", e);
                Trace.WriteLine("Decripted2 : " + d);
                Assert.AreNotEqual(data, d2);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                Assert.AreEqual("WrongPassword", ex.Message);
                Assert.IsTrue(ex.InnerException is CryptographicException);
                Assert.IsTrue(ex.InnerException.Message.StartsWith("Bad Data."));
            }
        }

        //[ExpectedException(typeof(CryptographicException))]
        [TestMethod]
        public async Task EncryptDecrypt_DumpingData_Test()
        {
            string starting = "StartingPadding";

            string pwd = "password1234";
            Trace.WriteLine($"Password : {pwd}");

            string data = starting + Guid.NewGuid().ToString();
            Trace.WriteLine($"Data : {data}");

            string e = await encryptDecrypt.EncryptStringAsync(pwd, data);

            Trace.WriteLine($"Encripted : {e}");

            string d = await encryptDecrypt.DecryptStringAsync(pwd, e);

            Assert.AreEqual(data, d);

            //modify a char from encrypted data.
            var fs = Convert.FromBase64String(e);
            var ca = fs[5] = (byte)'k';
            var e2 = Convert.ToBase64String(fs);

            try
            {
                string d2 = await encryptDecrypt.DecryptStringAsync(pwd, e2);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("DataCorruption", ex.Message);
            }

        }
    }
}
