using StockAnalyzer.Core;
using StockAnalyzer.Core.Domain;
using StockAnalyzer.Core.Services;
using StockAnalyzer.Windows.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;


namespace StockAnalyzer.Windows;

public partial class MainWindow : Window
{
    private static string API_URL = "https://ps-async.fekberg.com/api/stocks";
    private Stopwatch stopwatch = new Stopwatch();
    CancellationTokenSource? cancellationTokenSource;

    private Random random = new Random();

    public MainWindow()
    {
        InitializeComponent();
    }

    #region Search_Click = version 10
    private async void Search_Click(object sender, RoutedEventArgs e)
    {

    }
    #endregion Search_Click = version 10

    #region Search_Click = version 9 - Parallel Options & exceptions & for/foreach
    //private async void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    BeforeLoadingStockData();

    //    var stocks = new Dictionary<string, IEnumerable<StockPrice>>
    //    {
    //        { "MSFT", Generate("MSFT") },
    //        { "GOOGL", Generate("GOOGL") },
    //        { "AAPL", Generate("AAPL") },
    //        { "CAT", Generate("CAT") }
    //    };

    //    var bag = new ConcurrentBag<StockCalculation>();

    //    await Task.Run(() =>
    //    {
    //        try
    //        {
    //            Parallel.For(0, 10, (i, state) => {
    //                // i == current index
    //            });


    //            var parallelLoopResult = Parallel.ForEach(stocks, 
    //            //new ParallelOptions { MaxDegreeOfParallelism = 1 },
    //            (stock, state) =>
    //            {
    //                if(stock.Key == "MSFT" || state.ShouldExitCurrentIteration)
    //                {
    //                    state.Break();
    //                    return;
    //                }
    //                else
    //                {
    //                    var result = Calculate(stock.Value);
    //                    bag.Add(result);
    //                }


    //            });

    //        }
    //        catch (Exception ex)
    //        {
    //            Notes.Text = ex.Message;
    //        }
    //    });

    //    Stocks.ItemsSource = bag;

    //    AfterLoadingStockData();
    //}

    //private IEnumerable<StockPrice> Generate(string stockIdentifier)
    //{
    //    return Enumerable.Range(1, random.Next(10, 250))
    //        .Select(i => new StockPrice
    //        {
    //            Identifier = stockIdentifier,
    //            Open = random.Next(10, 1024)
    //        });
    //}

    //private StockCalculation Calculate(IEnumerable<StockPrice> prices)
    //{
    //    #region Start stopwatch
    //    var calculation = new StockCalculation();
    //    var watch = new Stopwatch();
    //    watch.Start();
    //    #endregion Start stopwatch

    //    var end = DateTime.UtcNow.AddSeconds(4);

    //    while (DateTime.UtcNow < end) { }


    //    #region Return a result
    //    calculation.Identifier = prices.First().Identifier;
    //    calculation.Result = prices.Average(s => s.Open);

    //    watch.Stop();
    //    calculation.TotalSeconds = watch.Elapsed.Seconds;

    //    return calculation;
    //    #endregion Return a result
    //}
    #endregion Search_Click = version 9 - Parallel Options & exceptions

    #region Search_Click = version 8 - Parallel Invoke & exceptions
    //private void Search_Click(object sender, RoutedEventArgs e)
    //{
    //BeforeLoadingStockData();

    //var stocks = new Dictionary<string, IEnumerable<StockPrice>>
    //    {
    //        { "MSFT", Generate("MSFT") },
    //        { "GOOGL", Generate("GOOGL") },
    //        { "AAPL", Generate("AAPL") },
    //        { "CAT", Generate("CAT") }
    //    };

    //var bag = new ConcurrentBag<StockCalculation>();

    //await Task.Run(() =>
    //    {
    //    try
    //    {
    //        Parallel.Invoke(
    //            //new ParallelOptions{ MaxDegreeOfParallelism = 2},
    //            () =>
    //            {
    //                var msft = Calculate(stocks["MSFT"]);
    //                bag.Add(msft);
    //            },
    //            () =>
    //            {
    //                var googl = Calculate(stocks["GOOGL"]);
    //                bag.Add(googl);
    //            },
    //            () =>
    //            {
    //                var aapl = Calculate(stocks["AAPL"]);
    //                bag.Add(aapl);
    //            },
    //            () =>
    //            {
    //                var cat = Calculate(stocks["CAT"]);
    //                bag.Add(cat);
    //            }
    //        );
    //    }
    //    catch (Exception ex)
    //    {
    //        Notes.Text = ex.Message;
    //    }
    //});

    //    Stocks.ItemsSource = bag;

    //    AfterLoadingStockData();
    //}

    //private IEnumerable<StockPrice> Generate(string stockIdentifier)
    //{
    //    return Enumerable.Range(1, random.Next(10, 250))
    //        .Select(i => new StockPrice
    //        {
    //            Identifier = stockIdentifier,
    //            Open = random.Next(10, 1024)
    //        });
    //}

    //private StockCalculation Calculate(IEnumerable<StockPrice> prices)
    //{
    //    #region Start stopwatch
    //    var calculation = new StockCalculation();
    //    var watch = new Stopwatch();
    //    watch.Start();
    //    #endregion Start stopwatch

    //    var end = DateTime.UtcNow.AddSeconds(4);

    //    while (DateTime.UtcNow < end) { }


    //    #region Return a result
    //    calculation.Identifier = prices.First().Identifier;
    //    calculation.Result = prices.Average(s => s.Open);

    //    watch.Stop();
    //    calculation.TotalSeconds = watch.Elapsed.Seconds;

    //    return calculation;
    //    #endregion Return a result
    //}
    #endregion Search_Click = version 8 - Parallel Options

    private void BeforeLoadingStockData()
    {
        stopwatch.Restart();
        StockProgress.Visibility = Visibility.Visible;
        StockProgress.IsIndeterminate = false;
        StockProgress.Value = 0;
        StockProgress.Maximum = StockIdentifier.Text.Split(' ', ',').Length;

    }

    private void AfterLoadingStockData()
    {
        StocksStatus.Text = $"Loaded stocks for {StockIdentifier.Text} in {stopwatch.ElapsedMilliseconds}ms";
        StockProgress.Visibility = Visibility.Hidden;
    }

    private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });

        e.Handled = true;
    }

    private void Close_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    #region Search_Click = version 7 - Completion source
    //private async void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        BeforeLoadingStockData();

    //        var data = await SearchForStocks();


    //        Stocks.ItemsSource = data.Where(price =>
    //                            price.Identifier == StockIdentifier.Text);
    //    }
    //    catch (Exception ex)
    //    {
    //        Notes.Text = ex.Message;
    //    }
    //    finally
    //    {
    //        AfterLoadingStockData();
    //    }
    //}

    //private Task<IEnumerable<StockPrice>> SearchForStocks()
    //{
    //    var tcs = new TaskCompletionSource<IEnumerable<StockPrice>>();

    //    ThreadPool.QueueUserWorkItem(_ =>
    //    {
    //        var lines = File.ReadAllLines("StockPrices_Small.csv");
    //        var prices = new List<StockPrice>();

    //        foreach (var line in lines.Skip(1))
    //        {
    //            prices.Add(StockPrice.FromCSV(line));
    //        }

    //        tcs.SetResult(prices);
    //    });

    //    return tcs.Task;
    //}

    #endregion Search_Click = version 4

    #region Search_Click = version 6 - Monitoring process 
    //private async void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        BeforeLoadingStockData();
    //        var progress = new Progress<IEnumerable<StockPrice>>();
    //        progress.ProgressChanged += (_, stocks) =>
    //        {
    //            StockProgress.Value += 1;
    //            Notes.Text += $"Loaded {stocks.Count()} for {stocks.First().Identifier}{Environment.NewLine}";
    //        };

    //        await SearchForStocks(progress);
    //    }
    //    catch (Exception ex)
    //    {
    //        Notes.Text = ex.Message;
    //    }
    //    finally
    //    {
    //        AfterLoadingStockData();
    //    }
    //}

    //private async Task SearchForStocks(IProgress<IEnumerable<StockPrice>> progress)
    //{
    //    var service = new StockService();
    //    var loadingTasks = new List<Task<IEnumerable<StockPrice>>>();

    //    foreach (var identifier in StockIdentifier.Text.Split(' ', ','))
    //    {
    //        var loadTask = service.GetStockPricesFor(identifier,
    //            CancellationToken.None);

    //        loadTask = loadTask.ContinueWith(completedTask =>
    //        {
    //            progress?.Report(completedTask.Result);
    //            return completedTask.Result;
    //        });

    //        loadingTasks.Add(loadTask);
    //    }

    //    var data = await Task.WhenAll(loadingTasks);

    //    Stocks.ItemsSource = data.SelectMany(stock => stock);
    //}

    #endregion Search_Click = version 3

    #region Search_Click = version 5 - run all searches in the same time -  unsing cancelation token
    //private async void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    if (cancellationTokenSource is not null)
    //    {
    //        cancellationTokenSource.Cancel();
    //        cancellationTokenSource.Dispose();
    //        cancellationTokenSource = null;

    //        Search.Content = "Search";
    //        return;
    //    }

    //    try
    //    {
    //        cancellationTokenSource = new();

    //        cancellationTokenSource.Token.Register(() =>
    //        {
    //            Notes.Text = "Cancellation requested";
    //        });

    //        Search.Content = "Cancel"; // Button text

    //        BeforeLoadingStockData();

    //        var identifiers = StockIdentifier.Text.Split(',', ' ');

    //        var service = new StockService();
    //        var loadingTasks = new List<Task<IEnumerable<StockPrice>>>();
    //        var stocks = new ConcurrentBag<StockPrice>();

    //        foreach (var identifier in identifiers)
    //        {
    //            var loadTask = service.GetStockPricesFor(identifier, cancellationTokenSource.Token);

    //            loadTask = loadTask.ContinueWith(t =>
    //            {
    //                var aFewStocks = t.Result.Take(5);

    //                foreach (var stock in aFewStocks)
    //                {
    //                    stocks.Add(stock);
    //                }
    //                Dispatcher.Invoke(() =>
    //                {
    //                    Stocks.ItemsSource = stocks.ToArray();
    //                });

    //                return aFewStocks;
    //            });

    //            loadingTasks.Add(loadTask);
    //        }

    //        var timeout = Task.Delay(120000);
    //        var allStocksLoadingTask = Task.WhenAll(loadingTasks);

    //        var completedTask = await Task.WhenAny(timeout, allStocksLoadingTask);

    //        if (completedTask == timeout)
    //        {
    //            cancellationTokenSource.Cancel();
    //            throw new OperationCanceledException("Timeout!");
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        Notes.Text = ex.Message;
    //    }
    //    finally
    //    {
    //        AfterLoadingStockData();
    //        cancellationTokenSource?.Dispose();
    //        cancellationTokenSource = null;

    //        Search.Content = "Search";
    //    }
    //}
    #endregion Search_Click = version 5

    #region Search_Click = version 4 - using ContinueWith
    //private void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        BeforeLoadingStockData();

    //        //var loadLinesTask = Task.Run(() =>
    //        //{
    //        //    var lines = File.ReadAllLines("StockPrices_Small.csv");

    //        //    return lines;
    //        //});

    //        var loadLinesTask = Task.Run(async () =>
    //        {
    //            using var stream = new StreamReader(File.OpenRead("StockPrices_Small.csv"));
    //            var lines = new List<string>();
    //            while (await stream.ReadLineAsync() is string line)
    //            {
    //                lines.Add(line);
    //            }
    //            return lines;
    //        });

    //        loadLinesTask.ContinueWith(task =>
    //        {
    //            Dispatcher.Invoke(() =>
    //            {
    //                Notes.Text = task.Exception?.InnerException?.Message;
    //            });

    //        }, TaskContinuationOptions.OnlyOnFaulted);


    //        var processStocksTask = loadLinesTask.ContinueWith((completedTask) =>
    //        {
    //            return completedTask.Result;

    //            #region Process readALl
    //            //var lines = completedTask.Result;

    //            //var data = new List<StockPrice>();

    //            //foreach (var line in lines.Skip(1))
    //            //{
    //            //    var price = StockPrice.FromCSV(line);
    //            //    data.Add(price);
    //            //}

    //            //Dispatcher.Invoke(() =>
    //            //{
    //            //    Stocks.ItemsSource = data.Where(sp => sp.Identifier == StockIdentifier.Text);
    //            //});
    //            #endregion
    //        }, TaskContinuationOptions.OnlyOnRanToCompletion);

    //        processStocksTask.ContinueWith(completedTask =>
    //        {
    //            Dispatcher.Invoke(() => { AfterLoadingStockData(); });
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        Notes.Text = ex.Message;
    //    }
    //    finally
    //    {
    //        AfterLoadingStockData();
    //    }
    //}
    #endregion Search_Click = version 4

    #region Search_Click = version 1 - async & await deadlocking
    //private void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        // NEVER DO THIS!
    //        Task.Run(SearchForStocks).Wait();
    //    }
    //    catch (Exception ex)
    //    {
    //        Notes.Text = ex.Message;
    //    }
    //}
    #endregion

    #region Search_Click = version 2 - streams and disposable
    //private async void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        BeforeLoadingStockData();

    //        var identifiers = StockIdentifier.Text.Split(' ', ',');

    //        var data = new ObservableCollection<StockPrice>();
    //        Stocks.ItemsSource= data;

    //        var service = new StockDiskStreamService();
    //        var enumerator = service.GetAllStockPrices();

    //        await foreach(var price in enumerator.WithCancellation(CancellationToken.None))
    //        {
    //            if (identifiers.Contains(price.Identifier))
    //                data.Add(price);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Notes.Text = ex.Message;
    //    }
    //}
    #endregion Search_Click = version 6

    #region Search_Click = version 3 - using ConfigureAwait(false);
    //private async void Search_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        var data = await GetStocksFor(StockIdentifier.Text);
    //        Notes.Text = "Stocks Loaded!";
    //        Stocks.ItemsSource= data;
    //    }
    //    catch (Exception ex)
    //    {

    //        throw;
    //    }
    //}

    //private async Task<IEnumerable<StockPrice>> GetStocksFor(string identifier)
    //{
    //    var service = new StockService();
    //    var data = await service.GetStockPricesFor(identifier, CancellationToken.None)
    //                            .ConfigureAwait(false);


    //    return data.Take(5);
    //}
    #endregion Search_Click = version 5 - using ConfigureAwait(false);

    #region SearchForStocks = initial
    //private async Task SearchForStocks()
    //{
    //    var service = new StockService();
    //    var loadingTasks = new List<Task<IEnumerable<StockPrice>>>();

    //    foreach (var identifier in StockIdentifier.Text.Split(' ', ','))
    //    {
    //        var loadTask = service.GetStockPricesFor(identifier,
    //            CancellationToken.None);

    //        loadingTasks.Add(loadTask);
    //    }

    //    var data = await Task.WhenAll(loadingTasks);

    //    Stocks.ItemsSource = data.SelectMany(stock => stock);
    //}

    //private static Task<List<string>> SearchForStocks(CancellationToken cancellationToken)
    //{
    //    return Task.Run(async () =>
    //    {
    //        using var stream = new StreamReader(File.OpenRead("StockPrices_Small.csv"));
    //        var lines = new List<string>();
    //        while (await stream.ReadLineAsync() is string line)
    //        {
    //            if (cancellationToken.IsCancellationRequested)
    //            {
    //                break;
    //            }
    //            lines.Add(line);
    //        }
    //        return lines;
    //    }, cancellationToken);
    //}

    //private async Task GetStocks()
    //{
    //    try
    //    {
    //        var store = new DataStore();

    //        var responseTask = store.GetStockPrices(StockIdentifier.Text);

    //        Stocks.ItemsSource = await responseTask;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;
    //    }
    //}

    #endregion SearchForStocks = initial

}