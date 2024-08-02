using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using MPL.Logging.Extensions;
using Serilog;
try
{
  var builder = WebApplication.CreateBuilder(args);

  builder.AddCustomLogger();

  builder.Services.AddHealthChecks();
  builder.Services.AddControllers();

  builder.Services.AddAzureClients(clientBuilder =>
  {
    clientBuilder.AddTableServiceClient(builder.Configuration["ConnectionStrings:Storage"]);
  });

  if (builder.Environment.IsDevelopment())
  {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
  }
  var app = builder.Build();

  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }

  app.UseAuthorization();

  app.UseHealthChecks("/health");
  app.MapControllers();

  ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

  if (logger.IsEnabled(LogLevel.Information))
    logger.LogInformation("Initialisation");

  if (logger.IsEnabled(LogLevel.Debug))
    logger.LogDebug("Ensure Azure Table \"Dummy\" is created");

  TableServiceClient tableServiceClient = app.Services.GetRequiredService<TableServiceClient>();
  await tableServiceClient.CreateTableIfNotExistsAsync("Dummy");
  
  if (logger.IsEnabled(LogLevel.Debug))
    logger.LogDebug("Table \"Dummy\" exists or has been created");

  if (logger.IsEnabled(LogLevel.Information))
    logger.LogInformation("Starting web application");

  app.Run();
}
catch (Exception ex)
{
  if (Log.IsEnabled(Serilog.Events.LogEventLevel.Fatal))
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}