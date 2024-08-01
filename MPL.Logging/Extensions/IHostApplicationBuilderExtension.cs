using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MPL.Logging.Extensions
{
  public static class IHostApplicationBuilderExtension
  {
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
