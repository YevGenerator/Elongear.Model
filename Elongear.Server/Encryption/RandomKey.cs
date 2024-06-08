using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Encryption;

public static class RandomKey
{
    public static byte[] GetRandomBytes(int amount) => RandomNumberGenerator.GetBytes(amount);

    public static string GetRandomBitSizedString(int bitAmount) => GetRandomByteSizedString(bitAmount / 8); 
    public static string GetRandomByteSizedString(int byteAmount)=> Encoding.UTF8.GetString(GetRandomBytes(byteAmount));
}
