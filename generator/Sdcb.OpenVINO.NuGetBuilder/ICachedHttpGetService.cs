using System.Text.Json;

namespace Sdcb.OpenVINO.NuGetBuilder
{
    public interface ICachedHttpGetService
    {
        async Task<T> DownloadAsJsonAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            using Stream ms = await DownloadAsStream(url, cancellationToken);
            T result = JsonSerializer.Deserialize<T>(ms) ?? throw new Exception($"Failed to deserialize {url} stream to {typeof(T)}");
            return result;
        }

        Task<Stream> DownloadAsStream(string url, CancellationToken cancellationToken = default);
    }
}