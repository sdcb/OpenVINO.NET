using CppSharp;
using CppSharp.AST;
using Csp = CppSharp.Parser;
using Vanara.PInvoke;
using System.Diagnostics;
using Sdcb.OpenVINO.NuGetBuilders.Extractors;
using Sdcb.OpenVINO.AutoGen.Headers;

namespace Sdcb.OpenVINO.AutoGen.Parsers;

public class HeadersParser
{
    public static ParsedInfo Parse(ExtractedInfo info)
    {
        DirectoryInfo rootFolder = new(Path.Combine(info.Directory, info.RootFolderName));
        if (!rootFolder.Exists)
        {
            throw new DirectoryNotFoundException($"Directory {rootFolder.FullName} not found");
        }

        HashSet<string> symbols = LoadSymbols(rootFolder.FullName);
        return new (symbols, ParsingHeaders(rootFolder.FullName));
    }

    private static HashSet<string> LoadSymbols(string extractedFolder)
    {
        string[] dlls = Directory
                    .EnumerateFiles(extractedFolder, "*.dll", SearchOption.AllDirectories)
                    .ToArray();
        string cDll = dlls.Single(x => x.EndsWith(@"Release\openvino_c.dll"));
        HashSet<string> symbols = GetExports(cDll);
        Console.WriteLine($"symbols count in openvino_c.dll: {symbols.Count}");
        return symbols;
    }

    private static TranslationUnit[] ParsingHeaders(string extractedFolder)
    {
        string headersDir = new FileInfo(Path.Combine(extractedFolder, @"runtime\include")).FullName;
        string cHeadersDir = new FileInfo(Path.Combine(extractedFolder, @"runtime\include\openvino\c")).FullName;
        Console.WriteLine($"Parsing {cHeadersDir}...");
        Csp.ParserOptions options = new()
        {
            Verbose = false,
            ASTContext = new Csp.AST.ASTContext(),
            LanguageVersion = Csp.LanguageVersion.C99,
        };
        options.Setup(TargetPlatform.Windows);
        options.AddIncludeDirs(headersDir);
        string[] headersToParse = Directory.EnumerateFiles(cHeadersDir, "*.h", SearchOption.AllDirectories).ToArray();
        Csp.ParserResult r = ClangParser.ParseSourceFiles(headersToParse, options);
        ASTContext ctx = ClangParser.ConvertASTContext(options.ASTContext);
        TranslationUnit[] units = ctx.TranslationUnits.Where(x => !x.IsSystemHeader).ToArray();
        return units;
    }

    private static HashSet<string> GetExports(string library)
    {
        nint hCurrentProcess = Process.GetCurrentProcess().Handle;

        if (!DbgHelp.SymInitialize(hCurrentProcess, null, false)) throw new Exception("SymInitialize failed.");

        try
        {
            ulong baseOfDll = DbgHelp.SymLoadModuleEx(hCurrentProcess, IntPtr.Zero, library, null, 0, 0, IntPtr.Zero, 0);
            if (baseOfDll == 0) throw new Exception($"SymLoadModuleEx failed for {library}.");

            return DbgHelp.SymEnumSymbolsEx(hCurrentProcess, baseOfDll)
                .Where(x => x.Flags.HasFlag(DbgHelp.SYMFLAG.SYMFLAG_EXPORT))
                .Select(x => x.Name)
                .ToHashSet();
        }
        finally
        {
            DbgHelp.SymCleanup(hCurrentProcess);
        }
    }
}
