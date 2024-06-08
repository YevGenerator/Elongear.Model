using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Encryption;

public class ActivationToken : BaseToken
{
    public string ConfirmationNumber { get; set; } = "";
    public override void SetFromValues(string[] values)
    {
        UserId = values[0];
        ConfirmationNumber = values[1];
        SetDate(values[6], values[5], values[4], values[3], values[2]);
    }

    public override string ToString() =>
        string.Join(TokenSeparator, [UserId, ConfirmationNumber, Date.Second, Date.Minute, Date.Hour, Date.Day, Date.Month]);
}
