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
    public interface IPrime
    {
        ConcurrentDictionary<double, double> FindPrime(double[] arrayNumbers, double[] basenumbers, ConcurrentDictionary<double, double> PrimeNumbers, int countThreads);
    }

    //Параллельный алгоритм #1: декомпозиция по данным
    class Algorithm1 : IPrime
    {
        private static void RunThreadFunc(object array)
        {
            if (array is object[] threadClass)
            {
                double[] decompositionNumbers = (double[])threadClass[0];
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
            List<List<double>> decompositionNumbers = CircularDecomposition(arrayNumbers, countThreads);

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
                threads[i].Start(new object[] { decompositionNumbers[i].ToArray(), basenumbers, PrimeNumbers });
            }

            // Ожидаем завершения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }


            return PrimeNumbers;
        }
    }


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


    //Параллельный алгоритм #3: применение пула потоков
    class Algorithm3 : IPrime
    {
        private static void RunThreadFunc(object array)
        {
            if (array is object[] obj)
            {
                double baseNumbers = (double)obj[0];
                ManualResetEvent ev = ((object[])obj)[1] as ManualResetEvent;
                ConcurrentDictionary<double, double> arrayNumbers = (ConcurrentDictionary<double, double>)obj[2];

                foreach(var number in arrayNumbers.Values)
                {
                    if (number % baseNumbers == 0)
                    {
                        arrayNumbers.TryRemove(number, out _);
                    }
                }

                ev.Set();
            }
        }

        public ConcurrentDictionary<double, double> FindPrime(double[] arrayNumbers, double[] basenumbers, ConcurrentDictionary<double, double> PrimeNumbers, int countThreads)
        {
            foreach (double number in arrayNumbers)
            {
                PrimeNumbers.TryAdd(number, number);
            }
            // Объявляем массив сигнальных сообщений
            ManualResetEvent[] events = new ManualResetEvent[countThreads];

            for(int th = 0; th < countThreads; th++)
            {
                // Добавляем в пул рабочие элементы с параметрами
                for (int i = 0; i < basenumbers.Length; i++)
                {
                    events[th] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(RunThreadFunc, new object[] { basenumbers[i], events[th], PrimeNumbers });
                }
            }
            
            // Дожидаемся завершения
            WaitHandle.WaitAll(events);

            return PrimeNumbers;
        }
    }



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


    public class PrimeNumbersClass
    {
        public string nameFunc;
        public IPrime Algorithm { private get; set; }
        public PrimeNumbersClass(string nameFunc, IPrime algorithm)
        {
            this.nameFunc = nameFunc;
            Algorithm = algorithm;
        }

        public ConcurrentDictionary<double, double> FindPrime(double[] array, double[] arrayPrime, ConcurrentDictionary<double, double> PrimeNumbers, int countThreads)
        {
            return Algorithm.FindPrime(array, arrayPrime, PrimeNumbers, countThreads);
        }
        public override string ToString() => $"{nameFunc}";
    }
}
