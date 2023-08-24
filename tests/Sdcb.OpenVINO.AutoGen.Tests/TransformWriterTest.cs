using Sdcb.OpenVINO.AutoGen.Headers;

namespace Sdcb.OpenVINO.AutoGen.Tests;

public class TransformWriterTest
{
    [Fact]
    public void DestinationFolderShouldBeValid()
    {
        Assert.True(File.Exists(TransformWriter.DestinationFolder));
    }
}