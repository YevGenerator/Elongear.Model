using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Encryption;

public class LoginToken : BaseToken
{
    public bool IsActive { get; set; } = true;
    public override string ToString() =>
        string.Join(TokenSeparator, [UserId, Date.Second, Date.Minute, Date.Hour, Date.Day, Date.Month, IsActive]);

    public static LoginToken Create(string userId)
        => new() { UserId = userId, Date = DateTime.Now.AddHours(2) };


    public override void SetFromValues(string[] values)
    {
        UserId = values[0];
        SetDate(values[5], values[4], values[3], values[2], values[1]);
        IsActive = bool.Parse(values[6]);
    }
}
