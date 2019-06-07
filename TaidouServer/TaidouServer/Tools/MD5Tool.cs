using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaidouServer.Tools
{
    class MD5Tool
    {
        public static string GetMD5(string str)
        {
            byte[] strByte = Encoding.UTF8.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output= md5.ComputeHash(strByte);
            string res = BitConverter.ToString(output).Replace("-", "");
            return res;
        }
    }
}
