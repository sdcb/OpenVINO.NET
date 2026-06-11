namespace Sdcb.OpenVINO.NuGetBuilders;

public class CachedHttpGetService : ICachedHttpGetService
{
    private readonly string _cacheFolder;

    public CachedHttpGetService(string cacheFolder)
    {
        if (cacheFolder == null) throw new ArgumentNullException(nameof(cacheFolder));
        Directory.CreateDirectory(cacheFolder);

        _cacheFolder = cacheFolder;
    }

    public async Task<Stream> DownloadAsStream(string url, CancellationToken cancellationToken = default)
    {
        string fileName = url.Split('/').Last();
        string localFilePath = Path.Combine(_cacheFolder, fileName);

        if (!File.Exists(localFilePath) || new FileInfo(localFilePath).Length == 0)
        {
            Console.WriteLine($"Downloading {url} to {localFilePath}");
            string tempFilePath = localFilePath + ".download";
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);

            using HttpClient client = new() { Timeout = Timeout.InfiniteTimeSpan };
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using (FileStream file = File.Create(tempFilePath))
                {
                    await contentStream.CopyToAsync(file, cancellationToken);
                }
                File.Move(tempFilePath, localFilePath, overwrite: true);
            }
            else
            {
                throw new Exception($"Error occurred while getting content from URL: {await response.Content.ReadAsStringAsync(cancellationToken)}");
            }
        }
        else
        {
            Console.WriteLine($"Using cached {localFilePath}");
        }

        // Now we're sure that the file exists already locally, load it from the file
        return File.OpenRead(localFilePath);
    }
}
