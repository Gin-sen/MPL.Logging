using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Microsoft.Extensions.Configuration;
using MPL.Logging.Enrichers;
using Serilog;

namespace MPL.Logging
{
  public static class CustomLoggerFactory
  {
    public static Serilog.Core.Logger CreateCustomLogger()
    {
      return new LoggerConfiguration()
        .Enrich.WithElasticApmCorrelationInfo()
        .Enrich.FromLogContext()
        .Enrich.With(new TechnicalEnricher())
        .WriteTo.Console(new EcsTextFormatter(new EcsTextFormatterConfiguration
        {
          IncludeHost = false,
          IncludeProcess = false,
          IncludeUser = false,
        }))
        .CreateLogger();
    }



    public static Serilog.Extensions.Hosting.ReloadableLogger CreateCustomBootstrapLogger()
    {
      return new LoggerConfiguration()
        .Enrich.WithElasticApmCorrelationInfo()
        .Enrich.FromLogContext()
        .Enrich.With(new TechnicalEnricher())
        .WriteTo.Console(new EcsTextFormatter(new EcsTextFormatterConfiguration
        {
          IncludeHost = false,
          IncludeProcess = false,
          IncludeUser = false,
        }))
        .CreateBootstrapLogger();
    }

    public static Serilog.Extensions.Hosting.ReloadableLogger CreateCustomBootstrapLogger(IConfiguration configuration)
    {
      return new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.WithElasticApmCorrelationInfo()
        .Enrich.FromLogContext()
        .Enrich.With(new TechnicalEnricher())
        .WriteTo.Console(new EcsTextFormatter(new EcsTextFormatterConfiguration
        {
          IncludeHost = false,
          IncludeProcess = false,
          IncludeUser = false,
        }))
        .CreateBootstrapLogger();
    }
  }
}
