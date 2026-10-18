using System.Text;

namespace MWSMan.Core.Infrastructure.Extenssion;

public static class SafeFileWriter
{
    public static async Task<bool> ReplaceContentAsync(
        string filePath,
        string newContent,
        int maxRetries = 10,
        int retryDelayMs = 500,
        CancellationToken cancellationToken = default)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await using var stream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 4096,
                    options: FileOptions.Asynchronous);

                stream.SetLength(0);

                await using var writer = new StreamWriter(
                    stream,
                    new UTF8Encoding(true),
                    bufferSize: 4096,
                    leaveOpen:true);

                await writer.WriteAsync(newContent.AsMemory(), cancellationToken);
                await writer.FlushAsync(cancellationToken);

                return true;
            }
            catch (IOException ex) when (attempt < maxRetries - 1)
            {
                await Task.Delay(retryDelayMs, cancellationToken);
            }
        }

        return false;
    }
}