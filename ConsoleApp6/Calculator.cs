namespace ParallelComputingLab;

public static class Calculator
{
    // TODO 1: условие отбора — деление на 13.
    private const int Divider = 13;

    public static CalculationResult CalculateSequential(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        long sum = 0;
        int count = 0;

        foreach (int number in numbers)
        {
            if (number % Divider == 0)
            {
                sum += number;
                count++;
            }
        }

        return new CalculationResult(sum, count);
    }

    public static CalculationResult CalculateParallel(
        int[] numbers,
        int maxDegreeOfParallelism = -1)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        if (maxDegreeOfParallelism != -1 &&
            maxDegreeOfParallelism < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxDegreeOfParallelism));
        }

        long sum = 0;
        int count = 0;

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };

        Parallel.For(
            0,
            numbers.Length,
            options,
            () => (LocalSum: 0L, LocalCount: 0),
            (i, state, local) =>
            {
                int number = numbers[i];

                if (number % Divider == 0)
                {
                    local.LocalSum += number;
                    local.LocalCount++;
                }

                return local;
            },
            local =>
            {
                Interlocked.Add(ref sum, local.LocalSum);
                Interlocked.Add(ref count, local.LocalCount);
            });

        return new CalculationResult(sum, count);
    }

    // TODO 6: неверная реализация — Lost Update.
    // Запускать на нескольких миллионах элементов,
    // чтобы увидеть расхождение с правильной реализацией.
    public static CalculationResult CalculateParallelWrong(int[] numbers)
    {
        long sum = 0;
        int count = 0;

        Parallel.For(0, numbers.Length, i =>
        {
            if (numbers[i] % Divider == 0)
            {
                // ОШИБКА: общие переменные изменяются
                // несколькими потоками без синхронизации.
                sum += numbers[i];
                count++;
            }
        });

        return new CalculationResult(sum, count);
    }

    // TODO 7: исправленная версия — то же имя, но с локальным состоянием.
    public static CalculationResult CalculateParallelWrongFixed(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        long sum = 0;
        int count = 0;

        Parallel.For(
            0,
            numbers.Length,
            () => (LocalSum: 0L, LocalCount: 0),
            (i, state, local) =>
            {
                int number = numbers[i];

                if (number % Divider == 0)
                {
                    local.LocalSum += number;
                    local.LocalCount++;
                }

                return local;
            },
            local =>
            {
                Interlocked.Add(ref sum, local.LocalSum);
                Interlocked.Add(ref count, local.LocalCount);
            });

        return new CalculationResult(sum, count);
    }
}