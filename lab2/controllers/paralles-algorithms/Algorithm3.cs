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

}