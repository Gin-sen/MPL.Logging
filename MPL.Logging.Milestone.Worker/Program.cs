using MPL.Logging.Extensions;
using MPL.Logging.Milestone.Worker;
using Serilog;

try
{
  var builder = Host.CreateApplicationBuilder(args);
  builder.AddDefaultLogStack();

  //builder.Services.AddHealthChecks();
  builder.Services.AddHostedService<Worker>();

  using var host = builder.Build();
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