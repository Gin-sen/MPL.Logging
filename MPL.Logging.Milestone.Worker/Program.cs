using MPL.Logging;
using MPL.Logging.Extensions;
using MPL.Logging.Milestone.Worker;
using Serilog;

Log.Logger = CustomLoggerFactory.CreateCustomBootstrapLogger();

try
{
  Log.Information("Starting web application");
  var builder = Host.CreateApplicationBuilder(args);
  builder.AddCustomLogger(Log.Logger);

  builder.Services.AddHealthChecks();
  builder.Services.AddHostedService<Worker>();

  var host = builder.Build();
  host.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}