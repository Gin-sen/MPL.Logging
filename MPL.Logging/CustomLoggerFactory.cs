using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using MPL.Logging.Enrichers;
using Serilog;

namespace MPL.Logging
{
  public static class CustomLoggerFactory
  {
    public static Serilog.Core.Logger CreateLogger()
    {
      return new LoggerConfiguration()
        .Enrich.WithElasticApmCorrelationInfo()
        .Enrich.FromLogContext()
        .WriteTo.Console(new EcsTextFormatter(new EcsTextFormatterConfiguration
        {
          IncludeHost = false,
          IncludeProcess = false,
          IncludeUser = false,
        }))
        .CreateLogger();
    }
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
  }
}
