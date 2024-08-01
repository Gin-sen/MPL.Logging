using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MPL.Logging.Enrichers;
using Serilog;

namespace MPL.Logging.Extensions
{
  public static class IHostApplicationBuilderExtension
  {

    /// <summary>
    /// Ajout d'un logger Serilog et ajout des APM hors developpement
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddCustomLogger(this IHostApplicationBuilder builder)
    {
      builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.WithElasticApmCorrelationInfo()
        .Enrich.FromLogContext()
        .Enrich.With(new TechnicalEnricher())
        .WriteTo.Console(new EcsTextFormatter(new EcsTextFormatterConfiguration
        {
          IncludeHost = false,
          IncludeProcess = false,
          IncludeUser = false,
        })));
      if (!builder.Environment.IsDevelopment())
        builder.Services.AddAllElasticApm();

      return builder;
    }

    /// <summary>
    /// Ajout d'un logger Serilog et ajout des APM hors developpement
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddCustomLogger(this IHostApplicationBuilder builder, ILogger logger)
    {
      builder.Services.AddSerilog(logger);
      if (!builder.Environment.IsDevelopment())
        builder.Services.AddAllElasticApm();

      return builder;
    }


    /// <summary>
    /// Ajout d'un logger Serilog et ajout des APM hors developpement
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureLogger"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddCustomLogger(this IHostApplicationBuilder builder, Action<IServiceProvider, LoggerConfiguration> configureLogger)
    {
      builder.Services.AddSerilog(configureLogger);
      if (!builder.Environment.IsDevelopment())
        builder.Services.AddAllElasticApm();

      return builder;
    }

    /// <summary>
    /// Ajout d'un logger Serilog et ajout des APM hors developpement
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureLogger"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddCustomLogger(this IHostApplicationBuilder builder, Action<LoggerConfiguration> configureLogger)
    {
      builder.Services.AddSerilog(configureLogger);
      if (!builder.Environment.IsDevelopment())
        builder.Services.AddAllElasticApm();

      return builder;
    }
  }
}
