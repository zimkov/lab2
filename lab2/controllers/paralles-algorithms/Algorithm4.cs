using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace lab2
{
      //Параллельный алгоритм #4: последовательный перебор простых чисел
    class Algorithm4 : IPrime
    {
        private static void RunThreadFunc(object array)
        {
            if (array is object[] obj)
            {
                ConcurrentDictionary<double, double> baseNumbers = (ConcurrentDictionary<double, double>)obj[0];
                ConcurrentDictionary<double, double> arrayNumbers = (ConcurrentDictionary<double, double>)obj[1];

                foreach (double baseN in baseNumbers.Values)
                {
                    foreach (double number in arrayNumbers.Values)
                    {
                        if (number % baseN == 0)
                        {
                            arrayNumbers.TryRemove(number, out _);
                        }
                    }
                    baseNumbers.TryRemove(baseN, out _);
                }

            }
        }

        public ConcurrentDictionary<double, double> FindPrime(double[] arrayNumbers, double[] basenumbers, ConcurrentDictionary<double, double> PrimeNumbers, int countThreads)
        {
            foreach (double number in arrayNumbers)
            {
                PrimeNumbers.TryAdd(number, number);
            }

            ConcurrentDictionary<double, double> basePrime = new ConcurrentDictionary<double, double>();

            foreach (double number in basenumbers)
            {
                basePrime.TryAdd(number, number);
            }

            // Создаем массив потоков
            Thread[] threads = new Thread[countThreads];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(RunThreadFunc);
                threads[i].Name = $"Поток {i}";
            }

            // Запускаем все потоки
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start(new object[] { basePrime, PrimeNumbers });
            }

            // Ожидаем завершения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }

            return PrimeNumbers;
        }
    }
}