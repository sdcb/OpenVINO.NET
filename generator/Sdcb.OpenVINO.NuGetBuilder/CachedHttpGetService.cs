namespace Sdcb.OpenVINO.NuGetBuilder;

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

        if (!File.Exists(localFilePath))
        {
            // If the file does not exist locally, download it from the url and save it to the cache folder
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);

                using FileStream file = File.Create(localFilePath);
                using MemoryStream memoryStream = new();
                await contentStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.WriteTo(file);
            }
            else
            {
                throw new Exception($"Error occurred while getting content from URL: {await response.Content.ReadAsStringAsync()}");
            }
        }

        // Now we're sure that the file exists already locally, load it from the file
        return File.OpenRead(localFilePath);
    }
}
