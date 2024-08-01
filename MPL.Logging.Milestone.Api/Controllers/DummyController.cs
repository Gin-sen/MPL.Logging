using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MPL.Logging.Milestone.Infrastructure.Entities;
using Serilog.Context;
using SharpCompress.Common;

namespace MPL.Logging.Milestone.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DummyController : ControllerBase
  {
    private readonly ILogger<DummyController> _logger;
    public DummyController(ILogger<DummyController> logger, TableServiceClient tableServiceClient)
    {
      _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PostItem([FromServices] TableServiceClient tableServiceClient, [FromQuery] string partitionKey, [FromQuery] string rowkey, [FromQuery] string thing)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      DummyEntity entity = new DummyEntity(partitionKey, rowkey, thing);
      await table.AddEntityAsync<DummyEntity>(entity);
      if (_logger.IsEnabled(LogLevel.Information))
      {
        using (LogContext.PushProperty("LogType", "metier"))
        {
          _logger.LogInformation("1 Dummy created");
        }
      }
      if (_logger.IsEnabled(LogLevel.Debug))
      {
        _logger.LogDebug("Dummy : {@Dummy}", entity);
      }
      return Created($"/test?partitionKey={partitionKey}&rowKey={rowkey}", new { PartitionKey = partitionKey, RowKey = rowkey, Thing = thing });
    }

    [HttpGet]
    public async Task<IActionResult> GetItem([FromServices] TableServiceClient tableServiceClient, [FromQuery] string partitionKey, [FromQuery] string rowkey)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      var result = await table.GetEntityAsync<DummyEntity>(partitionKey, rowkey);
      if (_logger.IsEnabled(LogLevel.Debug))
      {
        _logger.LogDebug("Dummy : {@Dummy}", result);
      }
      return Ok(result.Value);
    }
  }
}
