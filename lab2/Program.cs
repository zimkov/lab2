using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace lab2
{
    internal class Program
    {
        
        static double[] SieveEratosthenes(double[] numbers)
        {
            List<double> baseNumbers = numbers.ToList();
            for (int i = 0; i < baseNumbers.Count(); i++)
            {
                for (int j = 0; j < baseNumbers.Count(); j++)
                {
                    if (baseNumbers[j] == 1)
                    {
                        baseNumbers.Remove(baseNumbers[j]);
                    }
                    if (baseNumbers[j] % baseNumbers[i] == 0 && baseNumbers[i] != baseNumbers[j])
                    {
                        baseNumbers.Remove(baseNumbers[j]);
                    }
                }
            }
            return baseNumbers.ToArray();
        }

        public static string GetPrimeNumbers(double[] array, PrimeNumbersClass algorithm, int countThreads)
        {
            int countBaseNumbers = (int)Math.Sqrt(array.Length);
            double[] basenumbers = new double[countBaseNumbers];

            for (int i = 0; i < countBaseNumbers; i++)
            {
                basenumbers[i] = array[i];
            }

            int countN = array.Length - countBaseNumbers;
            double[] arrayNumbers = new double[countN];

            for (int i = 0; i < arrayNumbers.Length; i++)
            {
                arrayNumbers[i] = array[i + countBaseNumbers];
            }
            basenumbers = SieveEratosthenes(basenumbers);

            ConcurrentDictionary<double, double> PrimeNumbers = new ConcurrentDictionary<double, double>();

            string timeResult = "";
            // Запускаем таймер
            Stopwatch timer = new Stopwatch();
            timer.Start();

            PrimeNumbers = algorithm.FindPrime(arrayNumbers, basenumbers, PrimeNumbers, countThreads);

            // Останавливаем таймер и выводим время выполнения всех потоков
            timer.Stop();

            timeResult = "Время выполнения всех потоков: " + timer.Elapsed.ToString();

            double[] resultArray = new double[PrimeNumbers.Count];

            int countValue = 0;
            foreach (double value in PrimeNumbers.Values)
            {
                resultArray[countValue] = value;
                countValue++;
            }

            Array.Sort(resultArray);

            string result = "";

            foreach (double number in basenumbers)
            {
                result += number + " ";
            }

            foreach (double number in resultArray)
            {
                result += number + " ";
            }

            result = timeResult + "\n\n" + result;

            return result;

        }


        public static void GenerateFile(string path, int N)
        {
            File.WriteAllText(path, "");
            string buffer = "";
            for (int i = 1; i < N; i += 100)
            {
                for (int j = i; j < i + 100; j++)
                {
                    if (j != N) buffer += j + " ";
                    if (j == N) buffer += j;
                }
                File.AppendAllText(path, buffer);
                buffer = "";
            }
            //File.AppendAllText(path, N + "");
            Console.WriteLine("Файл заполнен числами от 1 до " + N);
        }


        public static List<double>? ReadFile(string path)
        {
            List<double> listNumbers = new List<double>();

            foreach (var line in File.ReadLines(path))
            {
                string[] parts = line.Split(' ');
                foreach (var part in parts)
                {
                    if (double.TryParse(part, out double number))
                    {
                        listNumbers.Add(number);
                        //Console.Write(number + " ");
                    }
                    else
                    {
                        Console.WriteLine($"Не удалось преобразовать строку: {part}");
                        return null;
                    }
                }
            }
            return listNumbers;
        }


        static void Main(string[] args)
        {

            string path = @"C:\Users\Admin\Desktop\Alexei\numbers.txt";
            int N = 10000;
            int M = 2;
            PrimeNumbersClass algorithm = new PrimeNumbersClass("Алгоритм1", new Algorithm4());

            GenerateFile(path, N);
            List<double> listNumbers = ReadFile(path);





            Console.WriteLine(GetPrimeNumbers(listNumbers.ToArray(), algorithm, M));
        }
    }
}
