using Sdcb.OpenVINO.AutoGen.Headers;
using Sdcb.OpenVINO.NuGetBuilder.Utils;
using System.CodeDom.Compiler;

namespace Sdcb.OpenVINO.AutoGen.Writers;

public static class TransformWriter
{
    public static string DestinationFolder { get; } = Path.Combine(
        DirectoryUtils.SearchFileInCurrentAndParentDirectories(new DirectoryInfo(Environment.CurrentDirectory), "OpenVINO.NET.sln").DirectoryName!,
        "src", $"{nameof(Sdcb)}.{nameof(OpenVINO)}", "Natives");

    public static void WriteAll(GeneratedAll all, string destinationFolder, string ns)
    {
        Directory.CreateDirectory(destinationFolder);
        WriteFunctions(all.Functions, Path.Combine(destinationFolder, $"NativeMethods.g.cs"), ns);
        WriteUnits(all.Structs, Path.Combine(destinationFolder, $"Structs.g.cs"), ns);
        WriteUnits(all.Enums, Path.Combine(destinationFolder, $"Enums.g.cs"), ns);
    }

    private static void WriteUnits(GeneratedUnits enums, string filePath, string ns)
    {
        using StreamWriter sw = new(filePath);
        using IndentedTextWriter w = new(sw, "    ");
        w.WriteLine("#pragma warning disable CS1591"); // 缺少对公共可见类型或成员的 XML 注释
        w.WriteLine("using System.Runtime.InteropServices;");
        w.WriteLine();
        w.WriteLine($"namespace {ns};");
        w.WriteLine();
        w.WriteLines(enums.Lines);
    }

    private static void WriteFunctions(GeneratedUnits funcs, string filePath, string ns)
    {
        using StreamWriter sw = new(filePath);
        using IndentedTextWriter w = new(sw, "    ");
        w.WriteLine("#pragma warning disable CS1591"); // 缺少对公共可见类型或成员的 XML 注释
        w.WriteLine("#pragma warning disable CS1573"); // 参数在 XML 注释中没有匹配的 param 标记(但其他参数有)
        w.WriteLine("using System;");
        w.WriteLine("using System.Runtime.InteropServices;");
        w.WriteLine();
        w.WriteLine($"namespace {ns};");
        w.WriteLine();
        w.WriteLine($"public static unsafe partial class NativeMethods");
        w.BeginIdent(() =>
        {
            w.WriteLines(funcs.Lines);
        });
    }
}
