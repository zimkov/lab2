using DotNetEnv;

namespace lab2
{
    public class ConsoleView
    {
        public static void MainConsole(){
            Console.WriteLine("\nДля выполнения программы введите любой символ");
            Console.WriteLine("\nДля выхода введите 0");
            int value = Convert.ToInt32(Console.ReadLine());
            if(value == 0) {
                return;
            }
            Console.Clear();
            Start();
            MainConsole();

        }

        static void Start()
        {
            Env.Load("/Users/zimkov/Desktop/lab2/lab2/.env");

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

            NumberController numberController = new NumberController();
            NumberStorage numberStorage = numberController.CreateNumberStorage(path, N);

            Console.WriteLine(numberController.PrintPrimeNumbers(numberStorage, algorithm, M));
        }
    }
}