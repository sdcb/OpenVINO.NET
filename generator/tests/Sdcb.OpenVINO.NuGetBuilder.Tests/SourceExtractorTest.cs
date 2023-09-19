using Moq;
using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilders.Extractors;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using System.Security.Cryptography;
using System.Text;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.NuGetBuilders.Tests;

public class SourceExtractorTest
{
    private readonly ITestOutputHelper _console;

    public SourceExtractorTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task CheckSha256()
    {
        // prepair
        Mock<ICachedHttpGetService> mock = new();
        mock.Setup(x => x.DownloadAsStream(It.IsAny<string>(), default))
            .Returns(Task.FromResult((Stream)new MemoryStream(Encoding.UTF8.GetBytes(
                """
                00d4934e8228d0bceba8006e5433864f683b4a9ca517d25f0fca074b2add19fa  w_openvino_toolkit_windows_2023.1.0.dev20230728_x86_64.zip
                """))));

        // act
        byte[] sha256 = await ArtifactDownloader.ReadSha256(TestCommon.Root.LatestStableVersion.Artifacts.Single(v => v.OS == KnownOS.Windows).Sha256Url, mock.Object);

        // assert
        _console.WriteLine(HexUtils.ByteArrayToHexString(sha256));
        Assert.Equal("00d4934e8228d0bceba8006e5433864f683b4a9ca517d25f0fca074b2add19fa", HexUtils.ByteArrayToHexString(sha256));
    }

    [Fact]
    public async Task CheckDownload()
    {
        // prepair
        Mock<ICachedHttpGetService> mock = new();
        byte[] zipArray = MockKeysFileAsZipByteArray(TestCommon.WindowsKeysFile);
        ArtifactInfo artifactInfo = TestCommon.Root.LatestStableVersion.Artifacts.Single(v => v.OS == KnownOS.Windows);
        mock.Setup(x => x.DownloadAsStream(It.IsAny<string>(), default))
            .Returns<string, CancellationToken>((url, cts) => Task.FromResult<Stream>(url switch
            {
                var x when x == artifactInfo.Sha256Url => new MemoryStream(Encoding.UTF8.GetBytes(
                    $"""
                    {HexUtils.ByteArrayToHexString(SHA256.HashData(zipArray))}  w_openvino_toolkit_windows_2023.1.0.dev20230728_x86_64.zip
                    """)),
                var x when x == artifactInfo.DownloadUrl => new MemoryStream(zipArray),
                 _ => throw new Exception("Unknown"),
            }));

        // act
        ArtifactDownloader e = new(mock.Object);
        await e.Download(artifactInfo);
    }

    [Fact]
    public void ListDllsTest()
    {
        // prepair
        IEnumerable<string> keys = File.ReadLines(TestCommon.WindowsKeysFile);
        string[] dynamicLibs = keys.Where(ArchiveExtractor.FilterWindowsDlls)
            .ToArray();

        Assert.All(dynamicLibs.Where(x => !x.EndsWith("cache.json")), x => Assert.EndsWith(".dll", x, StringComparison.OrdinalIgnoreCase));
        Assert.All(dynamicLibs, x => Assert.DoesNotContain("debug", x, StringComparison.OrdinalIgnoreCase));
    }

    static byte[] MockKeysFileAsZipByteArray(string keyPath)
    {
        using IWritableArchive zip = ArchiveFactory.Create(ArchiveType.Zip);
        foreach (string item in File.ReadLines(keyPath))
        {
            zip.AddEntry(item, new MemoryStream(), true);
        }
        MemoryStream ms = new();
        zip.SaveTo(ms, new WriterOptions(CompressionType.None));
        return ms.ToArray();
    }
}
