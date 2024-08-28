using Azure;
using Azure.Data.Tables;

namespace MPL.Logging.Milestone.Infrastructure.Entities
{
  public class DummyEntity : ITableEntity
  {
    public string? PartitionKey { get; set; }
    public string? RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string? Thing { get; set; }

    public DummyEntity() { }

    public DummyEntity(string partitionKey, string rowKey, string thing)
    {
      PartitionKey = partitionKey;
      RowKey = rowKey;
      Thing = thing;
    }

  }
}
