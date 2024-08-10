using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace DarlingEngine.Engine.Bundle
{
    public class CryptoTool
    {
        private static readonly byte[] defaultSalt = Encoding.UTF8.GetBytes("DarlingEngine");
        private static readonly byte[] defaultPwd = Encoding.UTF8.GetBytes("WhereisMyDarling003");
        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            var memoryStream = new MemoryStream();
            var rijndael = Rijndael.Create();
            rijndael.Key = Key;
            rijndael.IV = IV;
            var cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(clearData, 0, clearData.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        public static byte[] Encrypt(byte[] rawData)
        {
            var passwordDeriveBytes = new PasswordDeriveBytes(defaultPwd, defaultSalt);
            return Encrypt(rawData, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
        }

        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            var memoryStream = new MemoryStream();
            var rijndael = Rijndael.Create();
            rijndael.Key = Key;
            rijndael.IV = IV;
            var cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(cipherData, 0, cipherData.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        public static byte[] Decrypt(byte[] cipherData)
        {
            var passwordDeriveBytes = new PasswordDeriveBytes(defaultPwd, defaultSalt);
            return Decrypt(cipherData, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
        }

    }
}