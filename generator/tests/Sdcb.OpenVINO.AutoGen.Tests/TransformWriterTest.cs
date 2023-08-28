using Sdcb.OpenVINO.AutoGen.Writers;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.AutoGen.Tests;

public class TransformWriterTest
{
    private readonly ITestOutputHelper _console;

    public TransformWriterTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public void DestinationFolderShouldBeValid()
    {
        string dir = TransformWriter.DestinationFolder;
        _console.WriteLine(dir);
        Assert.True(Directory.Exists(dir));
    }
}