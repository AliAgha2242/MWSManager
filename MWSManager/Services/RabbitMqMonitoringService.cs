using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace MWSManager.Services;

public sealed class RabbitMqMonitoringOptions
{
    public string ManagementUrl { get; set; } = "http://Rabbit-srv:15672/";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string SqlConnectionStringName { get; set; } = "Monitoring";
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

public sealed class RabbitMqLogSummary
{
    public DateTime LogDate { get; init; }
    public string SenderName { get; init; } = "";
    public string ReceiverName { get; init; } = "";
    public long TotalLogs { get; init; }
    public long ErrorCount { get; init; }
    public long ErrorMessageCount { get; init; }
}

public sealed class RabbitMqLogEventSummary
{
    public DateTime LogDate { get; init; }
    public int MessageId { get; init; }
    public string MessageDescription { get; init; } = "";
    public string LogLevel { get; init; } = "";
    public string CurrentOperation { get; init; } = "";
    public long EventCount { get; init; }
    public long StopProgramCount { get; init; }
    public long TotalFinishCount { get; init; }
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
    Task<IReadOnlyList<RabbitMqLogSummary>> GetLogSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RabbitMqLogEventSummary>> GetLogEventsAsync(CancellationToken cancellationToken = default);
}

public sealed class RabbitMqMonitoringService(
    HttpClient httpClient,
    IOptions<RabbitMqMonitoringOptions> options,
    IConfiguration configuration) : IRabbitMqMonitoringService
{
    private readonly RabbitMqMonitoringOptions _options = options.Value;

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

    public async Task<IReadOnlyList<RabbitMqLogSummary>> GetLogSummaryAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CAST(CreateDate AS date) AS LogDate,
                COALESCE(SenderName, '') AS SenderName,
                COALESCE(ReceiverName, '') AS ReceiverName,
                COUNT_BIG(*) AS TotalLogs,
                SUM(CASE WHEN LogLevel = 'Error' THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS ErrorCount,
                SUM(CASE WHEN ErrorMessageId IS NOT NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS ErrorMessageCount
            FROM dbo.RabbitConsumerLog
            WHERE CreateDate >= DATEADD(day, -7, GETDATE())
            GROUP BY CAST(CreateDate AS date), SenderName, ReceiverName
            ORDER BY LogDate DESC, TotalLogs DESC;
            """;

        var result = new List<RabbitMqLogSummary>();
        await using var connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 15 };
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new RabbitMqLogSummary
            {
                LogDate = reader.GetDateTime(0),
                SenderName = reader.GetString(1),
                ReceiverName = reader.GetString(2),
                TotalLogs = reader.GetInt64(3),
                ErrorCount = reader.GetInt64(4),
                ErrorMessageCount = reader.GetInt64(5)
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<RabbitMqLogEventSummary>> GetLogEventsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                CAST(l.CreateDate AS date) AS LogDate,
                d.MessageId,
                COALESCE(m.MessageDescription, N'شرح ثبت نشده') AS MessageDescription,
                COALESCE(d.Loglevel, '') AS Loglevel,
                COALESCE(d.CurrentOperation, '') AS CurrentOperation,
                COUNT_BIG(*) AS EventCount,
                SUM(CASE WHEN d.IsStopProgram = 1 THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS StopProgramCount,
                SUM(CASE WHEN d.IsTotalFinish = 1 THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS TotalFinishCount
            FROM dbo.RabbitConsumerLogDetail AS d
            INNER JOIN dbo.RabbitConsumerLog AS l ON l.ID = d.RabbitConsumerLogID
            LEFT JOIN dbo.RabbitMessage AS m ON m.MessageId = d.MessageId
            WHERE l.CreateDate >= DATEADD(day, -7, GETDATE())
            GROUP BY CAST(l.CreateDate AS date), d.MessageId, m.MessageDescription, d.Loglevel, d.CurrentOperation
            ORDER BY LogDate DESC, EventCount DESC;
            """;

        var result = new List<RabbitMqLogEventSummary>();
        await using var connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 15 };
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new RabbitMqLogEventSummary
            {
                LogDate = reader.GetDateTime(0),
                MessageId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                MessageDescription = reader.GetString(2),
                LogLevel = reader.GetString(3),
                CurrentOperation = reader.GetString(4),
                EventCount = reader.GetInt64(5),
                StopProgramCount = reader.GetInt64(6),
                TotalFinishCount = reader.GetInt64(7)
            });
        }

        return result;
    }

    private SqlConnection CreateSqlConnection()
    {
        var connectionStringName = string.IsNullOrWhiteSpace(_options.SqlConnectionStringName)
            ? "Monitoring"
            : _options.SqlConnectionStringName;
        var connectionString = configuration.GetConnectionString(connectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' is not configured. Configure ConnectionStrings:{connectionStringName} for the database containing RabbitConsumerLog tables.");
        }

        return new SqlConnection(connectionString);
    }
}
