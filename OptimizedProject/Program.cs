using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static Dictionary<string, double> cache = new Dictionary<string, double>();

    static async Task Main()
    {
        Console.WriteLine("Введите числа через пробел (можно с запятой или точкой):");
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Ошибка: входные данные пусты.");
            return;
        }

        // Преобразование в числа с учетом разных разделителей (запятая и точка)
        double[] numbers;
        try
        {
            numbers = input.Split(' ')
                           .Select(s => double.Parse(s.Replace(",", "."), CultureInfo.InvariantCulture))
                           .ToArray();
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка: введите корректные числа.");
            return;
        }

        Stopwatch sw = Stopwatch.StartNew();

        // Асинхронная обработка
        Task<double> sumTask = Task.Run(() => CalculateSum(numbers));
        Task<double> maxTask = Task.Run(() => numbers.Max());
        Task<double> minTask = Task.Run(() => numbers.Min());

        await Task.WhenAll(sumTask, maxTask, minTask);

        sw.Stop();

        Console.WriteLine($"Сумма: {sumTask.Result}");
        Console.WriteLine($"Максимум: {maxTask.Result}");
        Console.WriteLine($"Минимум: {minTask.Result}");
        Console.WriteLine($"Время выполнения: {sw.ElapsedMilliseconds} мс");

        Console.WriteLine("Для выхода нажмите любую клавишу...");
        Console.ReadKey();
    }

    static double CalculateSum(double[] numbers)
    {
        string key = string.Join(",", numbers);
        if (cache.TryGetValue(key, out double cachedSum))
        {
            Console.WriteLine("Результат взят из кэша.");
            return cachedSum;
        }

        double sum = numbers.Sum();
        cache[key] = sum;
        return sum;
    }
}
