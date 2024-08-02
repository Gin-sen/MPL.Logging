using Azure;
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
    public DummyController(
      ILogger<DummyController> logger,
      TableServiceClient tableServiceClient)
    {
      _logger = logger;
    }

    [HttpPost("PostItem")]
    public async Task<IActionResult> PostItemAsync(
      [FromServices] TableServiceClient tableServiceClient, 
      [FromQuery] string partitionKey,
      [FromQuery] string rowkey,
      [FromQuery] string thing,
      CancellationToken cancellationToken)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      DummyEntity entity = new DummyEntity(partitionKey, rowkey, thing);
      try
      {
        await table.AddEntityAsync<DummyEntity>(entity, cancellationToken);
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
      }
      catch (RequestFailedException ex)
      {
        if (_logger.IsEnabled(LogLevel.Error))
        {
          _logger.LogError("Exception : {@Exception}", ex);
        }
        throw;
      }
      return Created($"/test?partitionKey={partitionKey}&rowKey={rowkey}", new { PartitionKey = partitionKey, RowKey = rowkey, Thing = thing });
    }

    [HttpGet("GetItem")]
    public async Task<IActionResult> GetItemAsync(
      [FromServices] TableServiceClient tableServiceClient,
      [FromQuery] string partitionKey,
      [FromQuery] string rowkey,
      CancellationToken cancellationToken)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      var result = await table.GetEntityAsync<DummyEntity>(partitionKey, rowkey, default, cancellationToken);
      if (_logger.IsEnabled(LogLevel.Debug))
      {
        _logger.LogDebug("Dummy : {@Dummy}", result);
      }
      return Ok(result.Value);
    }

    [HttpDelete("DeleteItem")]
    public async Task<IActionResult> DeleteItemAsync(
      [FromServices] TableServiceClient tableServiceClient,
      [FromQuery] string partitionKey,
      [FromQuery] string rowkey,
      CancellationToken cancellationToken)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      var result = await table.GetEntityAsync<DummyEntity>(partitionKey, rowkey, default, cancellationToken);
      if (_logger.IsEnabled(LogLevel.Debug))
      {
        _logger.LogDebug("Dummy : {@Dummy}", result);
      }
      await table.DeleteEntityAsync(partitionKey, rowkey, default, cancellationToken);
      return Ok(result.Value);
    }
  }
}
