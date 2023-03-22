using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LMFS.Data.Utilities
{
    public static class HashHelper
    {
        public static byte[] HashBytes(this byte[] data)
        {

            using (SHA256 _sha = SHA256.Create())
            {
                var hash = _sha.ComputeHash(data);
                return hash;
            }
        }
        public static string HashString(this byte[] data)
        {

            using (SHA256 _sha = SHA256.Create())
            {
                var hash = _sha.ComputeHash(data);
                var hash_str = Convert.ToBase64String(hash);
                return hash_str;
            }
        }
        public static string AddSalt(this string content)
        {
            var now=DateTime.UtcNow;
            return content + $"{now.Year}/{now.Month}/{now.Day}/{now.Hour}:{now.Minute % 5}";
        }
        public static byte[] HashBytes(this string content)
        {
            using (SHA256 _sha = SHA256.Create())
            {
                var hash = _sha.ComputeHash(Encoding.UTF8.GetBytes(content));
                return hash;
            }
        }
        public static string HashString(this string content)
        {
            using (SHA256 _sha = SHA256.Create())
            {
                var hash = _sha.ComputeHash(Encoding.UTF8.GetBytes(content));
                var hash_str = Convert.ToBase64String(hash);
                return hash_str;
            }
        }
    }
}
