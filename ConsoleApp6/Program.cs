namespace ParallelComputingLab;

internal class Program
{
    static void Main()
    {
        Console.WriteLine("=== Parallel Computing Lab (П2) ===");

        // TODO 1: добавлен размер 10_000_000.
        int[] sizes =
        {
            10_000,
            100_000,
            1_000_000,
            5_000_000,
            10_000_000
        };

        foreach (int size in sizes)
        {
            int[] numbers = DataGenerator.Generate(
                size,
                seed: 12345);

            BenchmarkRunner.Run(numbers);
            BenchmarkRunner.CompareParallelism(numbers);
            Console.WriteLine();
        }

        Console.WriteLine("Работа завершена.");
    }
}