using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace lab2
{
    internal class Program
    {
        
        
        static void Main(string[] args)
        {
            string path = @"C:\Users\Admin\Desktop\Alexei\numbers.txt";
            int N = 10000;
            int M = 2;

            PrimeNumbersClass algorithm = new PrimeNumbersClass("Алгоритм2", new Algorithm4());

            NumberStorage numberStorage = new NumberStorage(path, N);

            Console.WriteLine(numberStorage.PrintPrimeNumbers(algorithm, M));
        }
    }
}
