using Newtonsoft.Json;
using System;
using TransactionManager;
using CryptoApi;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            AesEncryptionPrivider.Create("hamed");
            RsaSignatureProvider p = new RsaSignatureProvider();
            p.ExportPrivateKey("an");
            Transaction t = new Transaction("an", 1, p.PublicKeyS, "samin", 12, 1, new System.Collections.Generic.List<TransactionInput>{
                new TransactionInput("hi"),
                new TransactionInput("bye") });
            t.GenerateSignture("an");
            var json = JsonConvert.SerializeObject(t);
            Console.WriteLine(json);
            var t2 = new Transaction(json);
            Console.WriteLine(t2.IsSignatureVerified);
            while (true)
            {

            }
        }
    }
}
