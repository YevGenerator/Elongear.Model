
using System.Collections.Concurrent;

using Timer = System.Timers.Timer;
namespace Elongear.Server.Encryption;

public class ActivationDict : IDisposable
{
    Timer timer;
    public int MinutesToWait { get; set; } = 3;
    public ActivationDict()
    {
        timer = new(TimeSpan.FromMinutes(MinutesToWait));
        timer.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        CheckAndRemoveExpired();
    }

    ConcurrentDictionary<string, ActivationToken> Tokens { get; set; } = [];
    ConcurrentBag<ActivationToken> ExpiredTokens { get; set; } = [];
    public void AddToken(ActivationToken token)
    {
        Tokens.AddOrUpdate(token.UserId, token, (x, value) => token);
    }
    public void ExpireToken(ActivationToken token)
    {
        ExpiredTokens.Add(token);
    }

    public void RemoveExpiredTokens()
    {
        foreach (var token in ExpiredTokens)
        {
            Tokens.TryRemove(token.UserId, out _);
        }
    }

    public bool IsValid(ActivationToken token)
    {
        var exists = Tokens.TryGetValue(token.UserId, out var savedToken);
        if (!exists || savedToken is null) return false;
        return token.ConfirmationNumber == savedToken.ConfirmationNumber;
    }
    public bool IsValid(string userId) => Tokens.ContainsKey(userId);
    public bool IsValid(string userId, string digits)
    {
        var exists = Tokens.TryGetValue(userId, out var token);
        if (!exists || token is null) return false;
        return token.ConfirmationNumber == digits;
    }
    public void CheckAndRemoveExpired()
    {
        CheckExpiredTokens();
        RemoveExpiredTokens();
    }
    public void CheckExpiredTokens()
    {
        foreach (var pair in Tokens)
        {
            if (pair.Value.IsExpired())
            {
                ExpireToken(pair.Value);
            }
        }
    }

    public void Dispose()
    {
        timer.Dispose();
    }
}
