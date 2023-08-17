using System.Diagnostics;
using System.IO.Compression;
using Vanara.PInvoke;
using CppSharp;
using Csa = CppSharp.AST;
using Csp = CppSharp.Parser;
using CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen;

internal class Workflow
{
    private readonly AppSettings _settings;

    public Workflow(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task Run()
    {
        string localFilePath = await DownloadOpenVINOBinaries();
        string extractedFolder = ExtractOpenVINOBinaries(localFilePath);
        HashSet<string> symbols = LoadSymbols(extractedFolder);
        TranslationUnit[] units = ParsingHeaders(extractedFolder);
        WriteAll(units, symbols);
    }

    private void WriteAll(TranslationUnit[] units, HashSet<string> symbols)
    {
        Function[] functions = units
            .SelectMany(x => x.Functions)
            .Where(x => symbols.Contains(x.Name))
            .ToArray();
        Console.WriteLine($"Detected functions: {units.SelectMany(x => x.Functions).Count()}, matched: {functions.Length} in symbols");
        string allFunctions = string.Join(Environment.NewLine + Environment.NewLine, functions.Select(TransformOneFunction));
    }

    private string TransformOneFunction(Function function)
    {
        string paramsText = function.Comment.FullComment.Blocks
            .OfType<ParamCommandComment>()
            .Select(x => $"/// <param name=\"{x.Arguments[0].Text}\">{string.Join(" ", x.ParagraphComment.Content.OfType<TextComment>().Select(x => x.Text)).Trim()}</param>")
            .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");
        paramsText = paramsText == null ? "" : Environment.NewLine + paramsText;
        return $"""
            /// <summary>{function.Comment.BriefText}</summary>{paramsText}
            {TypeTransform(function.ReturnType.Type)} {function.Name}({string.Join(", ", function.Parameters.Select(TransformOneParameter))});
            """;
    }

    private static string TypeTransform(Csa.Type type)
    {
        return type switch
        {
            PointerType p => $"{TypeTransform(p.Pointee)}*",
            BuiltinType bi => PrimitiveTypeToCSharp(bi.Type),
            TypedefType tdef => TypeTransform(tdef.Declaration.Type),
            TagType tag => tag.Declaration.Name,
            _ => type.ToString(),
        };
    }

    private static string PrimitiveTypeToCSharp(PrimitiveType type)
    {
        // help me implement this
        return type switch
        {
            PrimitiveType.Bool => "bool",
            PrimitiveType.Char => "byte",
            PrimitiveType.Double => "double",
            PrimitiveType.Float => "float",
            PrimitiveType.Int => "int",
            PrimitiveType.Long => "int",
            PrimitiveType.LongDouble => "double",
            PrimitiveType.LongLong => "long",
            PrimitiveType.Short => "short",
            PrimitiveType.SChar => "sbyte",
            PrimitiveType.UChar => "byte",
            PrimitiveType.UInt => "uint",
            PrimitiveType.ULong => "uint",
            PrimitiveType.ULongLong => "ulong",
            PrimitiveType.UShort => "ushort",
            PrimitiveType.Void => "void",
            _ => throw new NotImplementedException(),
        };
    }

    private object TransformOneParameter(Parameter p)
    {
        return $"{TypeTransform(p.QualifiedType.Type)} {p.Name}";
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
        //Csp.ParserResult r = ClangParser.ParseSourceFiles(new[] { @"C:\Users\sdfly\source\repos\OpenVINO.NET\src\Sdcb.OpenVINO.AutoGen\bin\Debug\net7.0\cache\extraction\w_openvino_toolkit_windows_2023.0.1.11005.fa1c41994f3_x86_64\runtime\include\openvino\c\ov_core.h" }, options);
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

    private async Task<string> DownloadOpenVINOBinaries()
    {
        Directory.CreateDirectory(_settings.DownloadFolder);
        string localFileName = _settings.DownloadUrl.Split('/').Last();
        string localFilePath = Path.Combine(_settings.DownloadFolder, localFileName);
        if (File.Exists(localFilePath))
        {
            Console.WriteLine($"Using exists {localFilePath}...");
            return localFilePath;
        }

        Console.Write($"Downloading {_settings.DownloadUrl} into {localFilePath}...");
        using HttpClient http = new HttpClient();
        using Stream stream = await http.GetStreamAsync(_settings.DownloadUrl);
        using FileStream localFileStream = File.OpenWrite(localFilePath);
        await stream.CopyToAsync(localFileStream);

        Console.WriteLine("Done");
        return localFilePath;
    }

    private string ExtractOpenVINOBinaries(string localFilePath)
    {
        Directory.CreateDirectory(_settings.ExtractionFolder);
        
        using ZipArchive zip = ZipFile.OpenRead(localFilePath);
        string rootFolderName = zip.Entries[0].FullName.Split('/')[0];
        string destinationFolder = Path.Combine(_settings.ExtractionFolder, rootFolderName);
        if (Directory.Exists(destinationFolder))
        {
            Console.WriteLine($"Using exists {destinationFolder}...");
            return destinationFolder;
        }

        Console.Write($"Extracting {localFilePath} into {_settings.ExtractionFolder}...");
        zip.ExtractToDirectory(_settings.ExtractionFolder, true);
        Console.WriteLine("Done");
        return destinationFolder;
    }
}
