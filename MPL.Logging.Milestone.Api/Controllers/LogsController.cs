﻿using Microsoft.AspNetCore.Http;
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
      _logger = logger;
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
      return Ok();
    }
  }
}
