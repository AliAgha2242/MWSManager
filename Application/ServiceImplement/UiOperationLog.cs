using Application.ServiceContract;
using System.Collections.ObjectModel;

namespace Application.ServiceImplement;

public sealed class UiOperationLog : IUiOperationLog
{
    private const int MaxEntries = 250;
    private readonly List<UiOperationLogEntry> _entries = new();
    private readonly object _sync = new();

    public IReadOnlyList<UiOperationLogEntry> Entries
    {
        get { lock (_sync) return new ReadOnlyCollection<UiOperationLogEntry>(_entries.ToList()); }
    }

    public int ErrorCount
    {
        get { lock (_sync) return _entries.Count(x => !x.IsSuccess); }
    }

    public event Action? Changed;

    public void Add(string operation, string receiver, bool isSuccess, int statusCode, string? message, Guid? responseId = null)
    {
        var entry = new UiOperationLogEntry(
            DateTime.Now,
            operation,
            string.IsNullOrWhiteSpace(receiver) ? "سرور نامشخص" : receiver,
            isSuccess,
            statusCode,
            string.IsNullOrWhiteSpace(message) ? (isSuccess ? "عملیات با موفقیت انجام شد." : "عملیات ناموفق بود؛ جزئیات پاسخ ثبت نشده است.") : message,
            responseId?.ToString());

        lock (_sync)
        {
            _entries.Insert(0, entry);
            if (_entries.Count > MaxEntries)
                _entries.RemoveRange(MaxEntries, _entries.Count - MaxEntries);
        }

        Changed?.Invoke();
    }

    public void Clear()
    {
        lock (_sync) _entries.Clear();
        Changed?.Invoke();
    }
}
