using Elongear.Server.MailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Encryption;

public class TokenOperator(MailClient mailClient)
{
    private byte[] ActivationKey = RandomKey.GetRandomBytes(32);
    private byte[] LoginKey = RandomKey.GetRandomBytes(32);
    private byte[] UploadKey = RandomKey.GetRandomBytes(32);
    private byte[] Iv = RandomKey.GetRandomBytes(16);
    public MailClient Mail { get; set; } = mailClient;

    private ActivationDict ActivationDict { get; set; } = new();

    public async Task AddActivationRecordAsync(string userId, string email)
    {
        var digits = RandomNumber.GetRandomDigits(5);
        var token = new ActivationToken
        {
            UserId = userId,
            Date = DateTime.Now.AddDays(2),
            ConfirmationNumber = RandomNumber.DigitsToString(digits)
        };
        ActivationDict.AddToken(token);
        var encryptor = new Encryptor(ActivationKey, Iv);
        var cryptoToken = encryptor.EncryptTextAsync(token.ToString());
        await Mail.SendActivationLetter(digits, cryptoToken, email);
    }

    public bool ConfirmSignUp(string token)
    {
        var decryptor = new Encryptor(ActivationKey, Iv);
        try
        {
            var decrypted = decryptor.DecryptTextAsync(token);
            var activationToken = new ActivationToken();
            activationToken.SetFromString(decrypted);
            return ActivationDict.IsValid(token);
        }
        catch
        {
            return false;
        }
    }

    public bool ConfirmSignUp(string userId, string number)
    {
        return ActivationDict.IsValid(userId, number);
    }
    public string[] GetUserAndDigitsFromActivationTokenAsync(string token)
    {
        var decryptor = new Encryptor(ActivationKey, Iv);
        try
        {
            var decrypted = decryptor.DecryptTextAsync(token);
            var decryptedActivationToken = new ActivationToken();
            decryptedActivationToken.SetFromString(decrypted);
            if (decryptedActivationToken.IsExpired()) return [];
            return [decryptedActivationToken.UserId, decryptedActivationToken.ConfirmationNumber];
        }
        catch
        {
            return [];
        }
    }
    public string? GetUserFromLoginTokenAsync(string token)
    {
        var decryptor = new Encryptor(LoginKey, Iv);
        try
        {
            var decrypted = decryptor.DecryptTextAsync(token);
            var loginToken = new LoginToken();
            loginToken.SetFromString(decrypted);
            if (loginToken.IsExpired() || !loginToken.IsActive) return null;
            return loginToken.UserId;
        }
        catch
        {
            return null;
        }
    }
    public string GetUploadToken(string userId, string podcastId)
    {
        var token = new UploadToken()
        {
            UserId = userId,
            Date = DateTime.Now.AddDays(2),
            PodcastId = podcastId
        };
        var encryptor = new Encryptor(UploadKey, Iv);
        var cryptoToken = encryptor.EncryptTextAsync(token.ToString());
        return cryptoToken;
    }
    public string[] GetUserAndPodcastFromUploadToken(string uploadToken)
    {
        var decryptor = new Encryptor(UploadKey, Iv);
        try
        {
            var decrypted = decryptor.DecryptTextAsync(uploadToken);
            var decryptedUploadToken = new UploadToken();
            decryptedUploadToken.SetFromString(decrypted);
            if (decryptedUploadToken.IsExpired()) return [];
            return [decryptedUploadToken.UserId, decryptedUploadToken.PodcastId];
        }
        catch
        {
            return [];
        }
    }
    public string[] GetUserAndPodcastFromUploadToken(string loginToken, string uploadToken)
    {
        var user = GetUserFromLoginTokenAsync(loginToken);
        if (user is null) return [];
        var uploadResults = GetUserAndPodcastFromUploadToken(uploadToken);
        if (uploadResults.Length == 0 || uploadResults[0] != user) return [];
        return uploadResults;
    }
    public string? GetUserFromShortLoginTokenAsync(string token)
    {
        var decryptor = new Encryptor(LoginKey, Iv);
        try
        {
            var decrypted = decryptor.DecryptTextAsync(token);
            var loginToken = new LoginToken();
            loginToken.SetFromString(decrypted);
            if (loginToken.IsExpired()) return null;
            return loginToken.UserId;
        }
        catch
        {
            return null;
        }
    }

    private string GetLoginToken(string userId, DateTime finishTime)
    {
        var encryptor = new Encryptor(LoginKey, Iv);
        var loginToken = new LoginToken()
        {
            UserId = userId,
            Date = finishTime,
            IsActive = true
        };
        var cryptoToken = encryptor.EncryptTextAsync(loginToken.ToString());
        return cryptoToken;
    }

    public string GetShortLoginToken(string userId)
    {
        var encryptor = new Encryptor(LoginKey, Iv);
        var loginToken = new LoginToken()
        {
            UserId = userId,
            Date = DateTime.Now.AddMinutes(5),
            IsActive = false
        };
        var cryptoToken = encryptor.EncryptTextAsync(loginToken.ToString());
        return cryptoToken;
    }

    public string GetDefaultLoginToken(string userId) =>
        GetLoginToken(userId, DateTime.Now.AddHours(6));

}
