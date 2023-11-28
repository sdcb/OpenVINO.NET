<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

async Task Main()
{
	await SetupAsync(QueryCancelToken);
}

async Task SetupAsync(CancellationToken cancellationToken = default)
{
	Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
	Environment.CurrentDirectory = Util.CurrentQuery.Location;
	await EnsureNugetExe(cancellationToken);
}

static void NuGetRun(string args) => Run(@".\nuget.exe", args, Encoding.GetEncoding("gb2312"));
static void DotNetRun(string args) => Run("dotnet", args.Dump(), Encoding.GetEncoding("utf-8"));
static void Run(string exe, string args, Encoding encoding) => Util.Cmd(exe, args, encoding);
static ProjectVersion[] Projects = new[]
{
	new ProjectVersion("Sdcb.OpenVINO", "0.6.1"),
	new ProjectVersion("Sdcb.OpenVINO.Extensions.OpenCvSharp4", "0.6.1"),
	new ProjectVersion("Sdcb.OpenVINO.PaddleOCR", "0.6.1"),
	new ProjectVersion("Sdcb.OpenVINO.PaddleOCR.Models.Online", "0.2.1"),
};

static async Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken = default)
{
	if (uri.Scheme == "https" || uri.Scheme == "http")
	{
		using HttpClient http = new();

		HttpResponseMessage resp = await http.GetAsync(uri, cancellationToken);
		if (!resp.IsSuccessStatusCode)
		{
			throw new Exception($"Failed to download: {uri}, status code: {(int)resp.StatusCode}({resp.StatusCode})");
		}

		using (FileStream file = File.OpenWrite(localFile))
		{
			await resp.Content.CopyToAsync(file, cancellationToken);
		}
	}
	else if (uri.Scheme == "file")
	{
		File.Copy(uri.ToString()[8..], localFile, overwrite: true);
	}
	else
	{
		throw new Exception($"Uri scheme: {uri.Scheme} not supported.");
	}
}

static async Task<string> EnsureNugetExe(CancellationToken cancellationToken = default)
{
	Uri uri = new Uri(@"https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
	string localPath = @".\nuget.exe";
	if (!File.Exists(localPath))
	{
		await DownloadFile(uri, localPath, cancellationToken);
	}
	return localPath;
}

record ProjectVersion(string name, string version);
