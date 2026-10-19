using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace MWSManager.Services;

public sealed class RabbitMqMonitoringOptions
{
    public string ManagementUrl { get; set; } = "http://Rabbit-srv:15672/";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}

public sealed class RabbitMqOverview
{
    [JsonPropertyName("rabbitmq_version")] public string? RabbitMqVersion { get; set; }
    [JsonPropertyName("cluster_name")] public string? ClusterName { get; set; }
    [JsonPropertyName("queue_totals")] public RabbitMqQueueTotals? QueueTotals { get; set; }
    [JsonPropertyName("object_totals")] public RabbitMqObjectTotals? ObjectTotals { get; set; }
}

public sealed class RabbitMqQueueTotals
{
    [JsonPropertyName("messages")] public long Messages { get; set; }
    [JsonPropertyName("messages_ready")] public long MessagesReady { get; set; }
    [JsonPropertyName("messages_unacknowledged")] public long MessagesUnacknowledged { get; set; }
}

public sealed class RabbitMqObjectTotals
{
    [JsonPropertyName("queues")] public int Queues { get; set; }
    [JsonPropertyName("consumers")] public int Consumers { get; set; }
    [JsonPropertyName("exchanges")] public int Exchanges { get; set; }
    [JsonPropertyName("connections")] public int Connections { get; set; }
    [JsonPropertyName("channels")] public int Channels { get; set; }
}

public sealed class RabbitMqQueue
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("vhost")] public string Vhost { get; set; } = "/";
    [JsonPropertyName("state")] public string State { get; set; } = "";
    [JsonPropertyName("messages")] public long Messages { get; set; }
    [JsonPropertyName("messages_ready")] public long MessagesReady { get; set; }
    [JsonPropertyName("messages_unacknowledged")] public long MessagesUnacknowledged { get; set; }
    [JsonPropertyName("consumers")] public int Consumers { get; set; }
    [JsonPropertyName("message_stats")] public RabbitMqMessageStats? MessageStats { get; set; }
}

public sealed class RabbitMqMessageStats
{
    [JsonPropertyName("publish_details")] public RabbitMqRate? PublishDetails { get; set; }
    [JsonPropertyName("deliver_get_details")] public RabbitMqRate? DeliverGetDetails { get; set; }
    [JsonPropertyName("ack_details")] public RabbitMqRate? AckDetails { get; set; }
}

public sealed class RabbitMqRate
{
    [JsonPropertyName("rate")] public double Rate { get; set; }
}

public sealed class RabbitMqMonitoringSnapshot
{
    public DateTimeOffset CollectedAt { get; init; } = DateTimeOffset.Now;
    public RabbitMqOverview Overview { get; init; } = new();
    public IReadOnlyList<RabbitMqQueue> Queues { get; init; } = Array.Empty<RabbitMqQueue>();
}

public interface IRabbitMqMonitoringService
{
    Task<RabbitMqMonitoringSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);
}

public sealed class RabbitMqMonitoringService(HttpClient httpClient, IOptions<RabbitMqMonitoringOptions> options)
    : IRabbitMqMonitoringService
{
    public async Task<RabbitMqMonitoringSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var overviewTask = httpClient.GetFromJsonAsync<RabbitMqOverview>("api/overview", cancellationToken);
        var queuesTask = httpClient.GetFromJsonAsync<List<RabbitMqQueue>>("api/queues", cancellationToken);
        await Task.WhenAll(overviewTask, queuesTask);

        return new RabbitMqMonitoringSnapshot
        {
            Overview = await overviewTask ?? new RabbitMqOverview(),
            Queues = (await queuesTask ?? []).OrderByDescending(q => q.Messages).ToArray(),
            CollectedAt = DateTimeOffset.Now
        };
    }
}
