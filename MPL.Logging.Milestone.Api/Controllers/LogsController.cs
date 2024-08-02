using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace MPL.Logging.Milestone.Api.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class LogsController : ControllerBase
  {
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogger<LogsController> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
    }

    [HttpGet(Name = "GetOK")]
    public IActionResult GetOK()
    {
      if (_logger.IsEnabled(LogLevel.Information))
      {
        using (LogContext.PushProperty("LogType", "metier"))
        {
          _logger.LogInformation("Getting OK");
        }
      }


      _logger.LogTrace("Trace Log Message");
      _logger.LogDebug("Debug Log Message");
      _logger.LogInformation("Information Log Message");
      _logger.LogWarning("Warning Log Message");
      _logger.LogError("Error Log Message");
      _logger.LogCritical("Critical Log Message");
      return Ok();
    }

    [HttpGet(Name = "Throw")]
    public async Task<IActionResult> ThrowAsync([FromQuery]int delayInSec = 1)
    {
      try
      {
        await Task.Delay(delayInSec * 1_000);
        throw new Exception("DummyException");
      }
      catch (Exception ex)
      {
        if (_logger.IsEnabled(LogLevel.Error))
        {
          _logger.LogError("Exception : {@Exception}", ex);
        }
      }
      return Ok();
    }
  }
}
