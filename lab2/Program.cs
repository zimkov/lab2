using DotNetEnv;
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
            Env.Load(@"C:\Users\Admin\Desktop\Alexei\C# проекты\lab2\lab2\.env");

            string path = Environment.GetEnvironmentVariable("pathfile");
            int N = Int32.Parse(Environment.GetEnvironmentVariable("N"));
            int M = Int32.Parse(Environment.GetEnvironmentVariable("M"));

            PrimeNumbersAlgorithm algorithm;

            switch (Int32.Parse(Environment.GetEnvironmentVariable("Algorithm")))
            {
                case 1: algorithm = new PrimeNumbersAlgorithm("Алгоритм 1", new Algorithm1()); break;
                case 2: algorithm = new PrimeNumbersAlgorithm("Алгоритм 2", new Algorithm2()); break;
                case 3: algorithm = new PrimeNumbersAlgorithm("Алгоритм 3", new Algorithm3()); break;
                case 4: algorithm = new PrimeNumbersAlgorithm("Алгоритм 4", new Algorithm4()); break;

                default: algorithm = new PrimeNumbersAlgorithm("Алгоритм 1", new Algorithm1()); break;
            }

            NumberStorage numberStorage = new NumberStorage(path, N);

            Console.WriteLine(numberStorage.PrintPrimeNumbers(algorithm, M));
        }
    }
}
