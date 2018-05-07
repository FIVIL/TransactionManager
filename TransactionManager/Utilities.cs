using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TransactionManager
{
    public static class Utilities
    {
        public static string GetHashString(this string data)
        {
            var bytedata = Encoding.UTF8.GetBytes(data);
            byte[] hash;
            using (var hasher = SHA256.Create())
            {
                hash = hasher.ComputeHash(bytedata);
            }
            return Convert.ToBase64String(hash);
        }
        public static double BlockRewardForThisVersion { get; set; }
    }
}
