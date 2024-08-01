using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using MPL.Logging;
using MPL.Logging.Extensions;
using Serilog;


Log.Logger = CustomLoggerFactory.CreateCustomBootstrapLogger(configuration);

try
{
  Log.Information("Starting web application");

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

  TableServiceClient tableServiceClient = app.Services.GetRequiredService<TableServiceClient>();
  await tableServiceClient.CreateTableIfNotExistsAsync("Dummy");

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}