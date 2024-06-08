
using System.Security.Cryptography;


namespace Elongear.Server.Encryption;

public class Encryptor
{
    public byte[] Key { get; set; } = [];
    public string KeyFileName { get; set; } = "";
    public byte[] Iv { get; set; } = [];
    public Encryptor()
    {

    }
    public Encryptor(byte[] key, byte[] iv)
    {
        Key = key;
        Iv = iv;
    }
    private Aes GetAes()
    {
        Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;
        aes.Padding = PaddingMode.Zeros;
        return aes;
    }

    public string EncryptTextAsync(string message)
    {

        using Aes aesAlg = GetAes();
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new (msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new (csEncrypt))
        {
            swEncrypt.Write(message);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string DecryptTextAsync(string encryptedMessage)
    {

        using Aes aesAlg = GetAes() ;
        
        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new (Convert.FromBase64String(encryptedMessage));
        using CryptoStream csDecrypt = new (msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new (csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}