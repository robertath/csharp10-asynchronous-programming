using System;
using System.Diagnostics;

namespace StockAnalyzer.AdvancedTopics;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        decimal total = Examples.ParrallelLinqSum();
        Console.WriteLine(total);

        Examples.ParrallelLinqTake();

        Console.WriteLine($"It took: {stopwatch.ElapsedMilliseconds}ms to run");
        Console.ReadLine();
    }
}

static class Examples
{
    static Random random = new();
    static decimal Compute(int value)
    {
        var randomMilliseconds = random.Next(10, 50);
        var end = DateTime.Now + TimeSpan.FromMicroseconds(randomMilliseconds);

        while (DateTime.Now < end) { }

        return value + 0.5m;
    }
    static object syncRoot = new();


    static object lock1 = new();
    static object lock2 = new();
    static ThreadLocal<decimal?> threadLocal = new();
    static AsyncLocal<decimal?> asyncLocal = new();


    /// <summary>
    /// Use Parrallel.For to iterate in a loop faster
    /// </summary>
    /// <returns>decimal</returns>
    public static decimal ParallelFor()
    {
        decimal total = 0;
        //for (int i = 0; i < 100; i++)
        //{
        //    total += Compute(i);
        //}

        Parallel.For(0, 100, (i) =>
        {
            total += Compute(i);
        });

        return total;
    }

    /// <summary>
    /// Use lock to increase security to the object in a threading in safe maner, freeze the object to avoid other update requests
    /// </summary>
    /// <returns>decimal</returns>
    public static decimal ParallelForAndLock()
    {
        decimal total = 0;

        Parallel.For(0, 100, (i) =>
        {
            var result = Compute(i);
            lock (syncRoot)
            {
                total += result;
            }
        });

        return total;
    }

    /// <summary>
    /// Use Interlocked to provide atomic operations for variables that shared by multiple threads
    /// </summary>
    /// <returns>int</returns>
    public static int ParallelForAndInterlock()
    {
        int total = 0;

        Parallel.For(0, 100, (i) =>
        {
            var result = Compute(i);
            Interlocked.Add(ref total, (int)result);
        });

        return total;
    }

    /// <summary>
    /// Avoiding a DeadLock
    /// Don't share lock objects for multiple shared resources
    /// Use one lock object for each shared resource
    /// Give you lock object a meaningful name
    /// Don't use a string as lock
    /// Don't use a type instance from typeOf() as a lock
    /// Don't use "this" as a lock
    /// </summary>
    /// <returns></returns>
    public static async Task AvoidingDeadLock()
    {
        int total = 0;

        var t1 = Task.Run(() =>
        {
            lock (lock1)
            {
                Thread.Sleep(1);
                lock (lock2)
                {
                    Console.WriteLine("Hello!");
                }
            }
        });
        var t2 = Task.Run(() =>
        {
            lock (lock2)
            {
                Thread.Sleep(1);
                lock (lock1)
                {
                    Console.WriteLine("Hello..?");
                }
            }
        });

        await Task.WhenAll(t1, t2);
    }

    /// <summary>
    /// Use CancellationToken 
    /// </summary>
    /// <returns></returns>
    public static int CancellationToken()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(2000);

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationTokenSource.Token,
            MaxDegreeOfParallelism = 1
        };
        int total = 0;
        try
        {
            Parallel.For(0, 100, parallelOptions, (i) =>
            {
                Interlocked.Add(ref total, (int)Compute(i));
            });
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine("Cancellation Requested!");
        }

        return total;
    }

    /// <summary>
    /// ThreadLocal<T> provides storage that is local to a thread
    /// ThreadLocal/Static data may be shared between multiple tasks that uses the same thread
    /// </summary>
    public static void StorageWithThreadLocal()
    {
        var options = new ParallelOptions { MaxDegreeOfParallelism = 2 };
        Parallel.For(0, 100, options, (i) =>
        {
            var currentValue = threadLocal.Value;
            threadLocal.Value = Compute(i); 
        });
    }


    /// <summary>
    /// AsyncLocal<T> represents ambient data (container) that is local to a given asynchronous control flow. such as an asynchronous method
    /// When you write to AsyncLocal, a local copy will be created
    /// The outer contexts value will not be overwritten
    /// </summary>
    public static void StorageWithAsyncLocal()
    {
        asyncLocal.Value = 200;

        var options = new ParallelOptions { MaxDegreeOfParallelism = 1 };
        Parallel.For(0, 100, options, async (i) =>
        {
            var currentValue = threadLocal.Value;
            asyncLocal.Value = Compute(i);
        });

        var currentValue = asyncLocal.Value;
    }

    /// <summary>
    /// PLINQ can achieve significant performance, but sometimes slows down certain queries    
    /// https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq    
    /// Sequenctial when queries contain:
    ///     -Select, indexed Where SelectMany, or ElementAt clause after an ordering or filtering operator that has removed or rearranged original indices
    ///     -Take, TakeWhile, Skip, SkipWhile operator and where indices in the source sequence are not in the original order
    ///     -Zip or SequenceEquals, unless one of the data sources has an originally ordered index and the other data source is indexable
    ///     -Concat, unless it is applied to indexable data sources
    ///     -Reverse, unless applied to an indexable data source
    /// </summary>
    public static decimal ParrallelLinqSum()
    {
        return Enumerable.Range(0, 100)
            .AsParallel()
            .AsOrdered()
            .Select(Compute)
            .Sum();
    }

    public static void ParrallelLinqTake()
    {
        var result = Enumerable.Range(0, 100)
            .AsParallel()
            .AsOrdered()
            //.WithCancellation(new(canceled: true))
            .Select(Compute)
            .Take(10);
        //.ToList();

        result.ForAll(Console.WriteLine);
        
        //result.ForEach(i => { Console.WriteLine(i); });
    }
}