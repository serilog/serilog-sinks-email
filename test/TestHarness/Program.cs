using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Email("from@localhost", "to@localhost", "localhost")
    .CreateLogger();

Log.Information("Hello, world!");

await Log.CloseAndFlushAsync();
