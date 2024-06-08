namespace RandomNumber.View;

using Elongear.Server.Encryption;
internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        for(int i =0; i< 100; i++)
        {
            for(int j =1; j <=5;j++)
            {
                var digits = RandomNumber.GetRandomDigits(j);
                Console.Write($"Цифер {j}, число: ");
                foreach(var digit in digits)
                {
                    Console.Write(digit.ToString());
                }

                Console.WriteLine("\tАбо\t" + string.Concat(digits));
            }
        }
    }
}
