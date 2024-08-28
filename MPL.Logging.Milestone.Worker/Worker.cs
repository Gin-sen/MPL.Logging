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
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
        }
        await Task.Delay(1_000, stoppingToken);
      }
    }
  }
}
