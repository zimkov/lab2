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
    //Параллельный алгоритм #2: декомпозиция набора простых чисел
    class Algorithm2 : IPrime
    {
        private static void RunThreadFunc(object array)
        {
            if (array is object[] threadClass)
            {
                double[] baseNumbers = (double[])threadClass[1];
                ConcurrentDictionary<double, double> arrayNumbers = (ConcurrentDictionary<double, double>)threadClass[2];


                foreach (double baseN in baseNumbers)
                {
                    foreach (var number in arrayNumbers.Values)
                    {
                        if (number % baseN == 0)
                        {
                            arrayNumbers.TryRemove(number, out _);
                        }
                    }
                }

            }
        }

        private static List<List<double>> CircularDecomposition(double[] array, int k)
        {
            int N = array.Length;
            List<List<double>> subarrays = new List<List<double>>();

            // Инициализация подмассивов
            for (int i = 0; i < k; i++)
            {
                subarrays.Add(new List<double>());
            }

            // Распределение элементов по подмассивам
            for (int i = 0; i < N; i++)
            {
                subarrays[i % k].Add(array[i]);
            }

            return subarrays;
        }

        public ConcurrentDictionary<double, double> FindPrime(double[] arrayNumbers, double[] basenumbers, ConcurrentDictionary<double, double> PrimeNumbers, int countThreads)
        {
            List<List<double>> decompositionNumbers = CircularDecomposition(basenumbers, countThreads);

            ConcurrentDictionary<double, double> result = PrimeNumbers;

            foreach (double number in arrayNumbers)
            {
                result.TryAdd(number, number);
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
                threads[i].Start(new object[] { arrayNumbers, decompositionNumbers[i].ToArray(), PrimeNumbers });
            }

            // Ожидаем завершения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }
    }

}