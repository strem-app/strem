using System.Security.Cryptography;
using Persistity.Encryption;

namespace Strem.Infrastructure.Services;

public class CustomEncryptor : IEncryptor
    {
        private RandomNumberGenerator _random;
        
        public int KeySize { get; }
        public int Iterations { get; }
        public string Password { get; }

        public CustomEncryptor(string password, int keySize = 128, int iterations = 1000)
        {
            Password = password;
            KeySize = keySize;
            Iterations = iterations;

            _random = RandomNumberGenerator.Create();
        }

        public byte[] Encrypt(byte[] data)
        {
            var saltStringBytes = GetRandomBytes(KeySize);
            var ivStringBytes = GetRandomBytes(KeySize);
            var password = new Rfc2898DeriveBytes(Password, saltStringBytes, Iterations);
            var keyBytes = password.GetBytes(KeySize / 8);

            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = KeySize;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();

                    var cipherTextBytes = saltStringBytes;
                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                    return cipherTextBytes;
                }
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            var derivedKeySize = KeySize / 8;
            var saltStringBytes = data.Take(derivedKeySize).ToArray();
            var ivStringBytes = data.Skip(derivedKeySize).Take(KeySize / 8).ToArray();
            var cipherTextBytes = data.Skip((derivedKeySize) * 2).Take(data.Length - ((derivedKeySize) * 2)).ToArray();

            var password = new Rfc2898DeriveBytes(Password, saltStringBytes, Iterations);
            var keyBytes = password.GetBytes(derivedKeySize);

            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = KeySize;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                using (var memoryStream = new MemoryStream(cipherTextBytes))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var plainTextStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(plainTextStream);
                    return plainTextStream.ToArray();
                }
            }
        }
       
        private byte[] GetRandomBytes(int keySize)
        {
            var actualBytes = new byte[keySize / 8];
            _random.GetBytes(actualBytes);
            return actualBytes;
        }
    }