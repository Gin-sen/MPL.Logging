using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MPL.Logging.Milestone.Infrastructure.Entities;

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
      await table.AddEntityAsync<DummyEntity>(new DummyEntity(partitionKey, rowkey, thing));
      return Created($"/test?partitionKey={partitionKey}&rowKey={rowkey}", new { PartitionKey = partitionKey, RowKey = rowkey, Thing = thing });
    }

    [HttpGet]
    public async Task<IActionResult> GetItem([FromServices] TableServiceClient tableServiceClient, [FromQuery] string partitionKey, [FromQuery] string rowkey, [FromQuery] string thing)
    {
      var table = tableServiceClient.GetTableClient("Dummy");
      var result = await table.GetEntityAsync<DummyEntity>(partitionKey, rowkey);
      return Ok(result.Value);
    }
  }
}
