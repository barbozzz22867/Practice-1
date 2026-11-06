using System.Diagnostics;

namespace ParallelComputingLab;

public static class BenchmarkRunner
{
    // TODO 3: количество измерений увеличено с 5 до 10.
    private const int MeasurementCount = 10;

    public static void Run(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        Console.WriteLine();
        Console.WriteLine($"Размер массива: {numbers.Length:N0}");

        // Прогрев.
        Calculator.CalculateSequential(numbers);
        Calculator.CalculateParallel(numbers);

        var sequentialTimes = new List<double>();
        var parallelTimes = new List<double>();

        CalculationResult? expectedResult = null;

        for (int i = 0; i < MeasurementCount; i++)
        {
            var sequential = Measure(
                () => Calculator.CalculateSequential(numbers));

            var parallel = Measure(
                () => Calculator.CalculateParallel(numbers));

            expectedResult ??= sequential.Result;

            if (sequential.Result != parallel.Result)
            {
                throw new InvalidOperationException(
                    "Последовательный и параллельный результаты не совпадают.");
            }

            sequentialTimes.Add(sequential.ElapsedMilliseconds);
            parallelTimes.Add(parallel.ElapsedMilliseconds);
        }

        Console.WriteLine($"Сумма:           {expectedResult!.Sum:N0}");
        Console.WriteLine($"Количество:      {expectedResult.Count:N0}");
        Console.WriteLine($"Последовательно: {Median(sequentialTimes):F3} мс");
        Console.WriteLine($"Параллельно:     {Median(parallelTimes):F3} мс");
    }

    // TODO 2: добавлена степень 3.
    // TODO 4: добавлена колонка ускорения.
    // TODO 9: проверяется совпадение результата для каждого варианта.
    public static void CompareParallelism(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        int[] degrees = { 1, 2, 3, 4, -1 };

        Console.WriteLine();
        Console.WriteLine($"Размер массива: {numbers.Length:N0}");
        Console.WriteLine(
            "Степень | Время, мс | Ускорение");
        Console.WriteLine(
            "------------------------------------");

        // Прогрев.
        Calculator.CalculateSequential(numbers);
        foreach (int degree in degrees)
        {
            Calculator.CalculateParallel(numbers, degree);
        }

        // Контрольный результат (TODO 9).
        var expected = Calculator.CalculateSequential(numbers);

        // Медиана последовательной обработки.
        var sequentialTimes = new List<double>();
        for (int i = 0; i < MeasurementCount; i++)
        {
            var m = Measure(
                () => Calculator.CalculateSequential(numbers));
            sequentialTimes.Add(m.ElapsedMilliseconds);
        }
        double sequentialMedian = Median(sequentialTimes);

        Console.WriteLine(
            $"{"seq",7} | {sequentialMedian,9:F3} | x1.00");

        foreach (int degree in degrees)
        {
            var times = new List<double>();

            for (int i = 0; i < MeasurementCount; i++)
            {
                var measurement = Measure(
                    () => Calculator.CalculateParallel(numbers, degree));

                // TODO 9: контроль совпадения результата.
                if (measurement.Result != expected)
                {
                    throw new InvalidOperationException(
                        $"Результат не совпадает при Degree = {degree}");
                }

                times.Add(measurement.ElapsedMilliseconds);
            }

            double median = Median(times);
            double speedup = median > 0
                ? sequentialMedian / median
                : 0;

            Console.WriteLine(
                $"{degree,7} | {median,9:F3} | x{speedup:F2}");
        }
    }

    public static (CalculationResult Result, double ElapsedMilliseconds)
        Measure(Func<CalculationResult> action)
    {
        var stopwatch = Stopwatch.StartNew();
        CalculationResult result = action();
        stopwatch.Stop();
        return (result, stopwatch.Elapsed.TotalMilliseconds);
    }

    private static double Median(List<double> values)
    {
        double[] sorted = values.OrderBy(x => x).ToArray();
        int middle = sorted.Length / 2;

        if (sorted.Length % 2 == 1)
        {
            return sorted[middle];
        }

        return (sorted[middle - 1] + sorted[middle]) / 2;
    }
}