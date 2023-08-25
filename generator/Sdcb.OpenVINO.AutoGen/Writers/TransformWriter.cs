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
        //WriteUnits(all.Structs, Path.Combine(destinationFolder, $"{nameof(all.Structs)}.g.cs"), ns);
        WriteEnums(all.Enums, Path.Combine(destinationFolder, $"Enums.g.cs"), ns);
    }

    private static void WriteEnums(GeneratedUnits enums, string filePath, string ns)
    {
        using StreamWriter sw = new(filePath);
        using IndentedTextWriter w = new(sw, "    ");
        w.WriteLine("using System;");
        w.WriteLine("using System.Runtime.InteropServices;");
        w.WriteLine();
        w.WriteLine($"namespace {ns};");
        w.WriteLine();
        w.WriteLine(enums.Text);
    }

    private static void WriteFunctions(GeneratedUnits funcs, string filePath, string ns)
    {
        using StreamWriter sw = new(filePath);
        using IndentedTextWriter w = new(sw, "    ");
        w.WriteLine("using System;");
        w.WriteLine("using System.Runtime.InteropServices;");
        w.WriteLine();
        w.WriteLine($"namespace {ns};");
        w.WriteLine();
        w.WriteLine($"public static unsafe partial class NativeMethods");
        w.BeginIdent(() =>
        {
            w.WriteLines(funcs.Text);
        });
    }
}
