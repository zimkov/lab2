using System.Collections.Concurrent;
using System.Dynamic;

namespace lab2
{
    public class NumberStorage
    {
        private List<double> listNumbers;
        private double[] baseNumbers;
        private double[] arrayNumbers;

        public NumberStorage(List<double> listNumbers, double[] baseNumbers, double[] arrayNumbers)
        {
            this.listNumbers = listNumbers;
            this.baseNumbers = baseNumbers;
            this.arrayNumbers = arrayNumbers;
        }

        public double[] GetBaseNumbers()
        {
            return baseNumbers;
        }

        public double[] GetArrayNumbers()
        {
            return arrayNumbers;
        }

        public List<double> GetListNumbers()
        {
            return listNumbers;
        }
        
    }
}
