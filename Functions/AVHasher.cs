using System;
using System.Security.Cryptography;
using System.Text;

namespace ArnoldVinkCode
{
    class AVHasher
    {
        public static string HMACSHA256(string Message, string Secret)
        {
            Encoding SignatureEncoding = Encoding.UTF8;

            byte[] keyBytes = SignatureEncoding.GetBytes(Secret);
            byte[] messageBytes = SignatureEncoding.GetBytes(Message);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] bytes = hmacsha256.ComputeHash(messageBytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}