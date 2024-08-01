using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Azure.Data.Tables;
using MPL.Logging;
using MPL.Logging.Extensions;
using MPL.Logging.Milestone.Api.Enrichers;
using Serilog;
using Microsoft.Extensions.Azure;
using Serilog.Context;


Log.Logger = CustomLoggerFactory.CreateLogger();
//using (LogContext.PushProperty("Logtype", "technique"))

try
{
  Log.Information("Starting web application");

  var builder = WebApplication.CreateBuilder(args);


  builder.AddCustomLogger(Log.Logger);


  builder.Services.AddHealthChecks();
  builder.Services.AddControllers();


  //builder.Services.AddAzureClients(clientBuilder =>
  //{
  //  clientBuilder.AddTableServiceClient(builder.Configuration["ConnectionStrings:Storage"]);
  //});


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