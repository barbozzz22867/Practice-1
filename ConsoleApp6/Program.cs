namespace ParallelComputingLab;

internal class Program
{
    static void Main()
    {
        Console.WriteLine("=== Parallel Computing Lab (П1) ===");

        // --- Контрольные сценарии (TODO 3, TODO 4, TODO 5) ---
        Console.WriteLine();
        Console.WriteLine("=== Контрольные сценарии ===");

        CheckScenario("7, 14, 20, 21", new[] { 7 * 2, 13 * 2, 20, 13 * 3 });
        // Намеренно используем числа, кратные 13, т.к. условие изменено.
        CheckScenario("кратные 13", new[] { 13, 26, 39, 20 });
        CheckScenario("нет подходящих", new[] { 1, 2, 3, 4 });
        CheckScenario("пустой массив", Array.Empty<int>());

        // TODO 3: массив из одного элемента.
        CheckScenario("один элемент", new[] { 13 });

        // TODO 5: null.
        try
        {
            Calculator.CalculateParallel(null!);
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("null -> ArgumentNullException (ожидаемо)");
        }

        // --- Основной бенчмарк ---
        int[] numbers = DataGenerator.Generate(
            size: 1_000_000,
            seed: 12345);

        BenchmarkRunner.Run(numbers);

        // --- TODO 6: Lost Update ---
        Console.WriteLine();
        Console.WriteLine("=== Lost Update Demo (TODO 6) ===");

        int[] big = DataGenerator.Generate(5_000_000, 12345);

        var correct = Calculator.CalculateParallel(big);
        var wrong = Calculator.CalculateParallelWrong(big);
        var wrongFixed = Calculator.CalculateParallelWrongFixed(big);

        Console.WriteLine(
            $"Правильная (Parallel):       Sum = {correct.Sum:N0}, Count = {correct.Count:N0}");
        Console.WriteLine(
            $"Неправильная (Wrong):        Sum = {wrong.Sum:N0}, Count = {wrong.Count:N0}");
        Console.WriteLine(
            $"Исправленная (WrongFixed):   Sum = {wrongFixed.Sum:N0}, Count = {wrongFixed.Count:N0}");

        Console.WriteLine(
            correct == wrong
                ? "Wrong совпала с правильной — гонка не проявилась, повторите запуск."
                : "Wrong расходится с правильной — Lost Update!");

        // TODO 7: проверка исправленной версии.
        Console.WriteLine(
            correct == wrongFixed
                ? "WrongFixed совпадает с правильной."
                : "WrongFixed всё ещё расходится — ошибка в коде.");

        Console.WriteLine();
        Console.WriteLine("Работа завершена.");
    }

    private static void CheckScenario(string name, int[] data)
    {
        var seq = Calculator.CalculateSequential(data);
        var par = Calculator.CalculateParallel(data);

        bool match = seq == par;
        Console.WriteLine(
            $"{name,-22} Sum = {seq.Sum,8}, Count = {seq.Count,3} | " +
            $"Parallel {(match ? "OK" : "MISMATCH")}");
    }
}