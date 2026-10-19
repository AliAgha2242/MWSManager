namespace Application.ServiceContract;

public sealed record UiOperationLogEntry(
    DateTime Timestamp,
    string Operation,
    string Receiver,
    bool IsSuccess,
    int StatusCode,
    string Message,
    string? ResponseId);

public interface IUiOperationLog
{
    IReadOnlyList<UiOperationLogEntry> Entries { get; }
    int ErrorCount { get; }
    event Action? Changed;
    void Add(string operation, string receiver, bool isSuccess, int statusCode, string? message, Guid? responseId = null);
    void Clear();
}
