using System;
using System.Security.Cryptography;
using System.Text;

namespace Kandu.Utility
{
    public class Encryption
    {
        private Util Util;
        public Encryption(Util util)
        {
            Util = util;
        }

        public string GetMD5Hash(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            byte[] hash = md5.ComputeHash(Util.Str.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < hash.Length; x++)
            {
                sb.Append(hash[x].ToString("X2"));
            }
            return sb.ToString();
        }

        public string getNewCSPRNG()
        {
            var rng = RandomNumberGenerator.Create();
            byte[] data = new byte[32];
            rng.GetBytes(data);
            return Convert.ToBase64String(data);
        }
    }
}
