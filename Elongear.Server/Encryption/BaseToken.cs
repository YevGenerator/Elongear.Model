namespace Elongear.Server.Encryption;

public abstract class BaseToken
{
    public string UserId { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Now;

    public static bool IsExpired(BaseToken currentToken) => currentToken.Date < DateTime.Now;
    public string TokenSeparator { get; set; } = @"||";
    public bool IsExpired() => IsExpired(this);

    public abstract void SetFromValues(string[] values);
    
    public void SetFromString(string tokenString, string seperator = @"||")
    {
        var splitted = tokenString.Split(seperator);
        SetFromValues(splitted);
    }

    public void SetDate(string month, string day, string hour, string minute, string second)
    {
        var now = DateTime.Now;
        Date = new(year: now.Year,
            month: int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second));
    }
}
