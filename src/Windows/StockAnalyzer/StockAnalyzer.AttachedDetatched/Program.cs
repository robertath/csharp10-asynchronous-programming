

Console.WriteLine("Working with Attached and Detached tasks and nested classes!");



Console.WriteLine("Starting");

var task = Task.Factory.StartNew(async () => {
    await Task.Delay(2000);

    return "Baby";
}).Unwrap();

var result = await task;

Console.WriteLine("Completed");

Console.ReadLine();