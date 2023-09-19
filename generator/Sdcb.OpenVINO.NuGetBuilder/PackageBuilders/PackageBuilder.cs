using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilders.Extractors;
using Sdcb.OpenVINO.NuGetBuilders.Utils;

namespace Sdcb.OpenVINO.NuGetBuilders.PackageBuilder;

public sealed class PackageBuilder
{
    public static void BuildNuGet(ExtractedInfo local, ArtifactInfo artifactInfo, string? suffix, string outputDir)
    {
        NuGetPackageInfo pkgInfo = NuGetPackageInfo.FromArtifact(artifactInfo);
        PrepairPropsFile(local, pkgInfo);
        PrepairIconFile(local.Directory);
        string nuspecFilePath = PrepairNuspecFile(local, pkgInfo);
        string cmd = suffix != null
            ? $"pack {nuspecFilePath} -Version {artifactInfo.Version} -Suffix {suffix} -OutputDirectory {outputDir}"
            : $"pack {nuspecFilePath} -Version {artifactInfo.Version} -OutputDirectory {outputDir}";
        using Process ps = Process.Start(new ProcessStartInfo("nuget", cmd)
        {
            WorkingDirectory = local.Directory,
        })!;
        ps.WaitForExit();
        if (ps.ExitCode != 0)
        {
            throw new Exception("NuGet generation failed.");
        }
    }

    private static void PrepairIconFile(string destinationFolder)
    {
        const string iconFileName = "icon.png";
        string destinationFile = Path.Combine(destinationFolder, iconFileName);
        if (Path.Exists(destinationFile))
        {
            Console.WriteLine($"{destinationFile} already exists, skip.");
        }
        else
        {
            string iconFile = DirectoryUtils.SearchFileInCurrentAndParentDirectories(new DirectoryInfo(destinationFolder), iconFileName).FullName;
            Console.WriteLine($"Copy {iconFile} to {destinationFile}.");
            File.Copy(iconFile, destinationFile);
        }
    }

    private static string PrepairPropsFile(ExtractedInfo local, NuGetPackageInfo pkgInfo)
    {
        string normalizedName = pkgInfo.NamePrefix.Replace(".", "") + "Dlls";
        XDocument props = XDocument.Parse($"""
        <Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
            <PropertyGroup>
                <{normalizedName}>$(MSBuildThisFileDirectory)..\..\runtimes</{normalizedName}>
            </PropertyGroup>
            <ItemGroup Condition="$(TargetFrameworkVersion.StartsWith('v4')) Or $(TargetFramework.StartsWith('net4'))">
            </ItemGroup>
        </Project>
        """);
        string ns = props.Root!.GetDefaultNamespace().NamespaceName;
        XmlNamespaceManager nsr = new(new NameTable());
        nsr.AddNamespace("p", ns);

        XElement itemGroup = props.XPathSelectElement("/p:Project/p:ItemGroup", nsr)!;
        itemGroup.Add(local.DynamicLibs
            .Select(x => Path.GetFileName(x))
            .Select(x => new XElement(XName.Get("Content", ns), new XAttribute("Include", $@"$({normalizedName})\{pkgInfo.Rid}\native\{x}"),
                new XElement(XName.Get("Link", ns), @$"dll\{pkgInfo.Rid}\{x}"),
                new XElement(XName.Get("CopyToOutputDirectory", ns), "PreserveNewest")))
                );

        string propsFile = Path.Combine(local.Directory, $"{pkgInfo.NamePrefix}.runtime.{pkgInfo.Rid}.props");
        props.Save(propsFile);
        return propsFile;
    }

    private static string PrepairNuspecFile(ExtractedInfo local, NuGetPackageInfo pkgInfo)
    {
        XDocument nuspec = XDocument.Parse($"""
        <?xml version="1.0" encoding="utf-8"?>
        <package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
            <metadata>
            <id>{pkgInfo.Name}</id>
            <version>0.1</version>
            <title>{pkgInfo.NamePrefix} native bindings for {pkgInfo.Rid}</title>
            <authors>sdcb</authors>
            <requireLicenseAcceptance>true</requireLicenseAcceptance>
            <description>Native binding for {pkgInfo.NamePrefix} to work on {pkgInfo.Rid}.</description>
            <summary>Native binding for {pkgInfo.NamePrefix} to work on {pkgInfo.Rid}.</summary>
            <releaseNotes></releaseNotes>
            <copyright>Copyright {DateTime.Now.Year}</copyright>

            <icon>images\icon.png</icon>
            <license type="expression">Apache-2.0</license>
            <projectUrl>https://github.com/sdcb/OpenVINO.NET</projectUrl>
            <tags>Sdcb OpenVINO AI linqpad-samples</tags>
            <repository type="git" url="https://github.com/sdcb/OpenVINO.NET.git" />
            <dependencies />
            <frameworkAssemblies>
            </frameworkAssemblies>
            </metadata>
            <files>
            <file src="icon.png" target="images\" />
            </files>
        </package>
        """);
        string ns = nuspec.Root!.GetDefaultNamespace().NamespaceName;
        XmlNamespaceManager nsr = new(new NameTable());
        nsr.AddNamespace("p", ns);
        XElement files = nuspec.XPathSelectElement("/p:package/p:files", nsr)!;
        XElement deps = nuspec.XPathSelectElement("/p:package/p:metadata/p:dependencies", nsr)!;

        File.WriteAllBytes(Path.Combine(local.Directory, "_._"), Array.Empty<byte>());
        foreach (string dep in new[] { "netstandard2.0", "net45" })
        {
            files.Add(new XElement(XName.Get("file", ns), new XAttribute("src", "_._"), new XAttribute("target", @$"lib\{dep}\_._")));
            deps.Add(new XElement(XName.Get("group", ns), new XAttribute("targetFramework", dep)));
        }

        files.Add(local.DynamicLibs.Select(x => new XElement(
            XName.Get("file", ns),
            new XAttribute("src", x.Replace($@"{pkgInfo.NamePrefix}.{pkgInfo.Rid}/", "")),
            new XAttribute("target", @$"runtimes\{pkgInfo.Rid}\native"))));
        files.Add(new[] { "net", /*"netstandard", "netcoreapp"*/ }.Select(x => new XElement(
            XName.Get("file", ns),
            new XAttribute("src", $"{pkgInfo.Name}.props"),
            new XAttribute("target", @$"build\{x}\{pkgInfo.Name}.props"))));
        string nuspecFilePath = Path.Combine(local.Directory, $"{pkgInfo.Name}.nuspec");
        nuspec.Save(nuspecFilePath);
        return nuspecFilePath;
    }
}
