using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using MPL.Logging.ExceptionHandlers;
using MPL.Logging.Extensions;
using Serilog;
try
{
  var builder = WebApplication.CreateBuilder(args);

  builder.AddDefaultLogStack();

  builder.Services.AddExceptionHandler<LogExceptionHandler>();

  builder.Services.AddHealthChecks();
  builder.Services.AddControllers();
  builder.Services.AddProblemDetails();

  builder.Services.AddAzureClients(clientBuilder =>
  {
    clientBuilder.AddTableServiceClient(builder.Configuration["ConnectionStrings:Storage"] ?? "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://localhost:10000/devstoreaccount1;QueueEndpoint=http://localhost:10001/devstoreaccount1;TableEndpoint=http://localhost:10002/devstoreaccount1;");
  });

  if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName.Equals("Docker"))
  {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
  }
  var app = builder.Build();

  app.UseExceptionHandler();
  app.UseStatusCodePages();

  if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("Docker"))
  {
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
  }

  //app.UseHttpLogging();
  app.UseAuthorization();

  app.UseHealthChecks("/health");
  app.MapControllers();

  ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

  if (logger.IsEnabled(LogLevel.Information))
    logger.LogInformation("Initialisation");

  if (logger.IsEnabled(LogLevel.Debug))
    logger.LogDebug("Ensure Azure Table \"Dummy\" is created");

  TableServiceClient tableServiceClient = app.Services.GetRequiredService<TableServiceClient>();
  Azure.Response tableCreationResponse = (await tableServiceClient.CreateTableIfNotExistsAsync("Dummy")).GetRawResponse();
  if (tableCreationResponse.Status == 204)
  {
    if (logger.IsEnabled(LogLevel.Debug))
    {
      logger.LogDebug("Table \"Dummy\" has been created");
    }
  }
  else if (tableCreationResponse.Status == 409)
  {
    if (logger.IsEnabled(LogLevel.Debug))
    {
      logger.LogDebug("Table \"Dummy\" already exists");
    }
  }
  else
  {
    throw new Exception("There was a problem during creation of the \"Dummy\" Table");
  }

  if (logger.IsEnabled(LogLevel.Information))
    logger.LogInformation("Starting web application");

  await app.RunAsync();
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