using System.Collections.Concurrent;

namespace lab2
{
    public interface IPrime
    {
        ConcurrentDictionary<double, double> FindPrime(double[] arrayNumbers, double[] basenumbers, ConcurrentDictionary<double, double> PrimeNumbers, int countThreads);
    }

    
    public class PrimeNumbersAlgorithm
    {
        public string nameFunc;
        public IPrime Algorithm { private get; set; }
        public PrimeNumbersAlgorithm(string nameFunc, IPrime algorithm)
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
