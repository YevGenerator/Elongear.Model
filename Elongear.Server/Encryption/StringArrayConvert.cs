using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ljson;
namespace Elongear.Server.Encryption;

public class StringArrayConvert : LjsonConvertArray<string>
{
    public override string[] GetValues(string[] obj)
    {
        return obj;
    }

    public override void SetValues(string[] obj, string[] values)
    {
        Array.Copy(values, obj, obj.Length);
    }
}
