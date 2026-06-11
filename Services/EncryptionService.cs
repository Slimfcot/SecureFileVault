using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecureFileVault.Services
{
    public class EncryptionService
    {
        public void EncryptFile(string inputPath, string outputPath, byte[] key, byte[] iv)
        {
            using FileStream inputFileStream = new FileStream(inputPath, FileMode.Open);
            using FileStream outputFileStream = new FileStream(outputPath, FileMode.Create);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using CryptoStream cryptoStream = new CryptoStream(
                outputFileStream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write);

            inputFileStream.CopyTo(cryptoStream);
        }

        public void DecryptFile(string inputPath, string outputPath, byte[] key, byte[] iv)
        {
            using FileStream inputFileStream = new FileStream(inputPath, FileMode.Open);
            using FileStream outputFileStream = new FileStream(outputPath, FileMode.Create);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using CryptoStream cryptoStream = new CryptoStream(
                inputFileStream,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

            cryptoStream.CopyTo(outputFileStream);
        }

        public byte[] GenerateKey(string password)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public byte[] GenerateIV()
        {
            return new byte[16];
        }
    }
}