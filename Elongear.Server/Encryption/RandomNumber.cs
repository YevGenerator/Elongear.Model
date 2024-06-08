
using System.Security.Cryptography;
namespace Elongear.Server.Encryption;

public static class RandomNumber
{
    public static byte GetRandomByte() => RandomNumberGenerator.GetBytes(1)[1];
    public static short GetRandomShort() => BitConverter.ToInt16(RandomNumberGenerator.GetBytes(2));
    public static int GetRandomInt() => BitConverter.ToInt32(RandomNumberGenerator.GetBytes(4));
    public static long GetRandomLong() => BitConverter.ToInt64(RandomNumberGenerator.GetBytes(8));

    //random digit is not bigger than 7
    public static int GetDigit(byte number) => number % 8;
    public static byte GetDigitAsByte(byte number) => (byte)(number % 8);

    //random digit is not bigger than 7
    public static byte[] GetRandomDigits(int numberOfDigits)
    {
        var bytes = RandomNumberGenerator.GetBytes(numberOfDigits);
        for (int i = 0; i < numberOfDigits; i++)
        {
            bytes[i] = GetDigitAsByte(bytes[i]);
        }
        return bytes;
    }

    public static string DigitsToString(byte[] bytes) => string.Concat(bytes);
}
