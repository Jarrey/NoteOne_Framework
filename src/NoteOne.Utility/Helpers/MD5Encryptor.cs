using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WEB_SERVICE
using System.Security.Cryptography;

namespace Chame.WebService.Helper
{
#else
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace NoteOne_Utility.Helpers
{
#endif
    public static class MD5Encryptor
    {
        public static string GetMD5(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                return GetMD5(br.ReadBytes((int)stream.Length));
            }
        }

        public static string GetMD5(byte[] bytes)
        {
            return GetMD5(Convert.ToBase64String(bytes));
        }

        public static string GetMD5(string str)
        {
#if WEB_SERVICE
            using (MD5 md5 = MD5.Create())
            {
                byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
#else
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
#endif
        }
    }
}
