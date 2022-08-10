using System.Text;
using Persistity.Extensions;
using Strem.Infrastructure.Services;

namespace Strem.UnitTests.SanityTests;

public class EncryptionTests
{
    public static byte[] Key = Encoding.UTF8.GetBytes("UxRBN8hfjzTG86d6SkSSNzyUhERGu5Zj");
    public static byte[] IV = Encoding.UTF8.GetBytes("7cA8jkRMJGZ8iMeJ");
    
    [Fact]
    public void should_encrypt_and_decrypt_bytes()
    {
        var encryptor = new CustomEncryptor(Key, IV);
        var dummyString = "12345";
        var expectedBytes = Encoding.UTF8.GetBytes(dummyString);
        var encryptedData = encryptor.Encrypt(expectedBytes);
        var actualBytes = encryptor.Decrypt(encryptedData);
        
        Assert.Equal(expectedBytes, actualBytes);
    }
    
    [Fact]
    public void should_encrypt_and_decrypt_with_extensions()
    {
        var encryptor = new CustomEncryptor(Key, IV);
        var expected = "12345";
        var encryptedData = encryptor.Encrypt(expected);
        var actual = encryptor.Decrypt(encryptedData);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void should_encrypt_and_decrypt_bytes_with_different_instances()
    {
        var Key1 = Encoding.UTF8.GetBytes("UxRBN8hfjzTG86d6SkSSNzyUhERGu5Zj");
        var IV1 = Encoding.UTF8.GetBytes("7cA8jkRMJGZ8iMeJ");
        var encryptor = new CustomEncryptor(Key1, IV1);
        
        var Key2 = Encoding.UTF8.GetBytes("UxRBN8hfjzTG86d6SkSSNzyUhERGu5Zj");
        var IV2 = Encoding.UTF8.GetBytes("7cA8jkRMJGZ8iMeJ");
        var decryptor = new CustomEncryptor(Key2, IV2);
        
        var dummyString = "12345";
        var expectedBytes = Encoding.UTF8.GetBytes(dummyString);
        var encryptedData = encryptor.Encrypt(expectedBytes);
        var actualBytes = decryptor.Decrypt(encryptedData);
        
        Assert.Equal(expectedBytes, actualBytes);
    }
}