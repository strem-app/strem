using Persistity.Encryption;
using System.Security.Cryptography;

namespace Strem.Infrastructure.Services;

public class CustomEncryptor : IEncryptor
{
    public byte[] Key { get; }
    public byte[] IV { get; }

    public CustomEncryptor(byte[] key, byte[]? iv = null)
    {
        IV = iv ?? new byte[key.Length/2];
        Key = key;
    }

    public byte[] Encrypt(byte[] data)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        return aes.EncryptCbc(data, IV);
    }

    public byte[] Decrypt(byte[] data)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        return aes.DecryptCbc(data, IV);
    }
}