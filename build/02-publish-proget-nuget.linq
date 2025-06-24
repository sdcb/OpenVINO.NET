<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

LINQPad.DumpContainer dc = new DumpContainer().Dump();

async Task Main()
{
	Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
	await EnsureNugetExe(QueryCancelToken);
	Refresh();
}

void PublishProGet(string path)
{
	QueryCancelToken.ThrowIfCancellationRequested();
	string nugetApiKey = Util.GetPassword("proget-api-key");
	string nugetApiUrl = Util.GetPassword("proget-api-test");
	DotNetRun($@"nuget push {path} -s {nugetApiUrl} -k {nugetApiKey}");
	// dotnet nuget add source -n proget -u zhoujie -p "xxxx"
	//DotNetRun($@"nuget push {path} -s proget");
}

void PublishNuGet(string path)
{
	QueryCancelToken.ThrowIfCancellationRequested();
	string nugetApiUrl = "nuget.org";
	string nugetApiKey = Util.GetPassword("nuget-api-key");
	//DotNetRun($@"nuget push {path} -k {nugetApiKey} -s {nugetApiUrl}");
	DotNetRun($@"nuget push {path} -k {nugetApiKey} -s {nugetApiUrl}");
}

void Refresh()
{
	string dir = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath)!, "nupkgs");
	IEnumerable<string> pkgs = Directory.EnumerateFiles(dir, "*.nupkg")
		.Where(x => !x.Contains("2025.1.0"))
		;
	dc.Content = new
	{
		Functions = Util.HorizontalRun(true,
			new Button("✅Publish All", _ => pkgs.ToList().ForEach(pkg => PublishProGet(pkg))),
			new Button("⚠Publish All to NuGet", _ => pkgs.ToList().ForEach(pkg => PublishNuGet(pkg))),
			new Button("📂Open Folder", _ => Process.Start("explorer", @$"/select, ""{dir}"""))
			),
		Table = BuildTable()
	};

	object BuildTable()
	{
		return pkgs
			.Select(x => new
			{
				Package = Path.GetFileNameWithoutExtension(x),
				Size = $"{new FileInfo(x).Length / 1024.0 / 1024:N2}MB",
				Functions = Util.HorizontalRun(true,
					new Button("✅Publish", o => PublishProGet(x)),
					new Button("⚠Publish NuGet", o => PublishNuGet(x)),
					new Button("📁Open Folder", o => Process.Start("explorer", @$"/select, ""{x}""")),
					new Button("❌Delete", o =>
					{
						File.Delete(x);
						Refresh();
					})),
			});
	}
}

static void NuGetRun(string args) => Run(@".\nuget.exe", args, Encoding.GetEncoding("gb2312"));
static void DotNetRun(string args) => Run("dotnet", args.Dump(), Encoding.GetEncoding("utf-8"));
static void Run(string exe, string args, Encoding encoding) => Util.Cmd(exe, args, encoding);

static async Task<string> EnsureNugetExe(CancellationToken cancellationToken = default)
{
	Uri uri = new Uri(@"https://dist.nuget.org/win-x86-commandline/latest/nuget.exe");
	string localPath = @"C:\_\3rd\bin\nuget.exe";
	if (!File.Exists(localPath))
	{
		await DownloadFile(uri, localPath, cancellationToken);
	}
	return localPath;
}

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