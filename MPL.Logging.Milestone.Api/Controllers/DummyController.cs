using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MPL.Logging.Milestone.Infrastructure.Entities;
using Serilog.Context;
using SharpCompress.Common;
using System.Net;

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
        return Created($"/test?partitionKey={partitionKey}&rowKey={rowkey}", new { PartitionKey = partitionKey, RowKey = rowkey, Thing = thing });
      }
      catch (RequestFailedException ex)
      {
        if (_logger.IsEnabled(LogLevel.Error))
        {
          _logger.LogError("Exception : {@Exception}", ex);
        }
        return Problem("Something went wrong");
      }
    }

    [HttpGet("GetItem")]
    public async Task<IActionResult> GetItemAsync(
      [FromServices] TableServiceClient tableServiceClient,
      [FromQuery] string partitionKey,
      [FromQuery] string rowkey,
      CancellationToken cancellationToken)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      try
      {
        Azure.Response<DummyEntity> result = await table.GetEntityAsync<DummyEntity>(partitionKey, rowkey, default, cancellationToken);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogDebug("Dummy : {@Dummy}", result);
        }
        return Ok(result.Value);
      }
      catch (RequestFailedException ex) when (ex.Status.Equals((int)HttpStatusCode.NotFound))
      {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogDebug("Dummy not found");
        }
        return NotFound();
      }
      catch (RequestFailedException ex)
      {
        if (_logger.IsEnabled(LogLevel.Error))
        {
          _logger.LogError("Something wrong happened while getting the entity in Azure Table :\n{@Exception}", ex);
        }
        return Problem("Something went wrong");
      }
    }

    [HttpDelete("DeleteItem")]
    public async Task<IActionResult> DeleteItemAsync(
      [FromServices] TableServiceClient tableServiceClient,
      [FromQuery] string partitionKey,
      [FromQuery] string rowkey,
      CancellationToken cancellationToken)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      try
      {
        Azure.Response<DummyEntity> result = await table.GetEntityAsync<DummyEntity>(partitionKey, rowkey, default, cancellationToken);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogDebug("Dummy : {@Dummy}", result);
        }
        try
        {
          await table.DeleteEntityAsync(partitionKey, rowkey, default, cancellationToken);
          return Ok(result.Value);
        }
        catch (RequestFailedException ex)
        {
          if (_logger.IsEnabled(LogLevel.Error))
          {
            _logger.LogError("Something wrong happened while deleting the entity in Azure Table :\n{@Exception}", ex);
          }
          return Problem("Something went wrong");
        }
      }
      catch (RequestFailedException ex) when(ex.Status.Equals((int) HttpStatusCode.NotFound))
      {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
          _logger.LogInformation("Dummy not found");
        }
        return NotFound();
      }
      catch (RequestFailedException ex)
      {
        if (_logger.IsEnabled(LogLevel.Error))
        {
          _logger.LogError("Something wrong happened while getting the entity in Azure Table :\n{@Exception}", ex);
        }
        return Problem("Something went wrong");
      }
    }
  }
}
