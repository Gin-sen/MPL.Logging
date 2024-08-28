using Elastic.Apm.Config;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPL.Logging.ExceptionHandlers
{
  public class LogExceptionHandler : IExceptionHandler
  {
    private readonly ILogger<LogExceptionHandler> _logger;
    public LogExceptionHandler(ILogger<LogExceptionHandler> logger)
    {
      _logger = logger;
    }
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        _logger.LogInformation(
          "Handling exception");
      }
      var transaction = Elastic.Apm.Agent.Tracer.CurrentTransaction;
      if (transaction != null)
      {
        transaction.CaptureException(exception);
        
        if (_logger.IsEnabled(LogLevel.Error))
        {
          _logger.LogError(
            "HttpContext : {@HttpContext}", httpContext);
          _logger.LogError(
            "Exception : {@exception}", exception);
        }
      }
      else
      {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogError(
            "Exception was not captured by APM Agent");
        }
      }

      // Return false to continue with the default behavior
      // - or - return true to signal that this exception is handled
      return ValueTask.FromResult(false);
    }
  }
}
