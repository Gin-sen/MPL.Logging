using Elastic.Apm.Api;

namespace MPL.Logging.Milestone.Worker
{
  public class Worker : BackgroundService
  {
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {

        var transaction = Elastic.Apm.Agent
          .Tracer.StartTransaction("SendMailCommerce", ApiConstants.TypeMessaging);

        try
        {
          if (_logger.IsEnabled(LogLevel.Debug))
          {
            _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
          }
          await Task.Delay(1_000 * 30, stoppingToken);
        }
        catch (Exception ex)
        {
          transaction.CaptureException(ex);
          if (_logger.IsEnabled(LogLevel.Debug))
          {
            _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
            _logger.Log(LogLevel.Information,"Worker running at: {time}", DateTimeOffset.Now);
          }
        }
        finally
        {
          transaction.End();
        }
      }
    }
  }
}
