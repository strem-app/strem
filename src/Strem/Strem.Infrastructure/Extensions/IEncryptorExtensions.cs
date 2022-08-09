using System.Text;
using Persistity.Encryption;

namespace Strem.Infrastructure.Extensions;

public static class IEncryptorExtensions
{
    public static string Encrypt(this IEncryptor encryptor, string textToEncrypt)
    {
        var textAsBytes = Encoding.UTF8.GetBytes(textToEncrypt);
        var encryptedData = encryptor.Encrypt(textAsBytes);
        return Convert.ToBase64String(encryptedData);
    }
    
    public static string Decrypt(this IEncryptor encryptor, string encryptedText)
    {
        var textAsBytes = Convert.FromBase64String(encryptedText);
        var encryptedData = encryptor.Decrypt(textAsBytes);
        return Encoding.UTF8.GetString(encryptedData);
    }
}