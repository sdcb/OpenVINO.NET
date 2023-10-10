<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

#load ".\00-common"

DumpContainer dc = new DumpContainer().Dump();

async Task Main()
{
	await SetupAsync(QueryCancelToken);	
	Refresh();
}

void Refresh()
{
	dc.Content = LoadTable();
}

object LoadTable()
{
	return Projects
		.Select(x => new
		{
			Project = x.name, 
			Version = x.version, 
			Build = new Button("Build", o => Build(x))
		});
}

void Build(ProjectVersion p)
{
	string projPosition = Path.Combine(@"..\" +  SearchProjectPathInSolutionFile(File.ReadAllText(@"..\OpenVINO.NET.sln"), p.name));

	//DotNetRun($@"build ..\src\{project}\{project}.csproj -c Release");
	DotNetRun($@"pack {projPosition} -p:Version={p.version} -c Release -o .\nupkgs");
	Refresh();
}

public static string SearchProjectPathInSolutionFile(string solutionFileContent, string projectName)
{
	//将projectName后面加上".csproj"
	string projectSearchKey = projectName + ".csproj";

	try
	{
		//在solutionFileContent中寻找projectSearchKey
		Match match = Regex.Match(solutionFileContent, string.Format(@"""([^""]*\\{0})""", Regex.Escape(projectSearchKey)));

		if (match.Success)
		{
			// Match.Success will be true if a match was found.
			// match.Groups[1].Value should contain the desired substring.
			return match.Groups[1].Value;
		}
		else
		{
			// If no match was found, throw an exception.
			throw new Exception("Project path could not be found in the solution file.");
		}
	}
	catch (Exception ex)
	{
		// 如果遇到任何错误，则抛一个异常
		throw new Exception("An error occurred while searching the project path in the solution file.", ex);
	}
}
