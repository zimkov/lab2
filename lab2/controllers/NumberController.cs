using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;


namespace lab2
{
    public class NumberController
    {
        public NumberStorage CreateNumberStorage(string path, int N) 
        {
            GenerateFile(path, N);
            List<double> listNumbers = ReadFile(path);
            double[] array = listNumbers.ToArray();

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

            double[] baseNumbers = SieveEratosthenes(basenumbers);

            return new NumberStorage(listNumbers, baseNumbers, arrayNumbers);
        }

        private static double[] SieveEratosthenes(double[] numbers)
        {
            List<double> baseNumbers = numbers.ToList();
            for (int i = 0; i < baseNumbers.Count; i++)
            {
                for (int j = 0; j < baseNumbers.Count; j++)
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

        public string PrintPrimeNumbers(NumberStorage numberStorage, PrimeNumbersAlgorithm algorithm, int countThreads)
        {
            ConcurrentDictionary<double, double> PrimeNumbers = new ConcurrentDictionary<double, double>();
            string timeResult = "";
            double[] arrayNumbers = numberStorage.GetArrayNumbers();
            double[] baseNumbers = numberStorage.GetBaseNumbers();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            PrimeNumbers = algorithm.FindPrime(arrayNumbers, baseNumbers, PrimeNumbers, countThreads);

            timer.Stop();

            timeResult = algorithm.nameFunc+ ": Время выполнения всех потоков: " + timer.Elapsed.ToString();

            double[] resultArray = new double[PrimeNumbers.Count];

            int countValue = 0;
            foreach (double value in PrimeNumbers.Values)
            {
                resultArray[countValue] = value;
                countValue++;
            }

            return ToString(resultArray, baseNumbers, timeResult);
        }

        public string ToString(double[] resultArray, double[] baseNumbers, string timeResult)
        {
            Array.Sort(resultArray);

            string result = "";

            foreach (double number in baseNumbers)
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

        private static void GenerateFile(string path, int N)
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


        private static List<double> ReadFile(string path)
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
                        return listNumbers;
                    }
                }
            }
            return listNumbers;
        }
    }
}