using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransactionManager;
using System;
using CryptoApi;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            AesFileEncryptionPrivider.Create("hamed");
            var key1 = new KeyContainer();
            var key2 = new KeyContainer();
            key1.ExportPrivateKey("key.dat");
            var t1 = new Transaction(1, key1.PublicKeyS, key2.PublicKeyS, 15.5, 0, Guid.NewGuid().ToString().GetHashString());
            t1.GenerateSignture("key.dat");
            Assert.IsTrue(t1.IsSignatureVerified);
            key2.ExportPrivateKey("key.dat");
            t1.GenerateSignture("key.dat");
            Assert.IsFalse(t1.IsSignatureVerified);
            key1.Dispose();
            key2.Dispose();
        }
    }
}
