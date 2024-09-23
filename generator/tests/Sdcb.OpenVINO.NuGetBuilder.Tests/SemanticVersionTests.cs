using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;

namespace Sdcb.OpenVINO.NuGetBuilder.Tests;

public class SemanticVersionTests
{
    [Theory]
    [InlineData("2024.3.0-15549-ae01c973d53", 2024, 3, 0, "15549-ae01c973d53")]
    [InlineData("2022.1", 2022, 1, 0, null)]
    [InlineData("2022.3.1", 2022, 3, 1, null)]
    [InlineData("2024.2.0rc1", 2024, 2, 0, "rc1")]
    [InlineData("2024.2.0-alpha", 2024, 2, 0, "alpha")]
    [InlineData("2024.2.1-beta", 2024, 2, 1, "beta")]
    [InlineData("2024.3.0.dev20240807", 2024, 3, 0, "dev20240807")]
    public void ParseOpenVINOVersion_ValidInput_ShouldReturnCorrectSemanticVersion(
        string input, int expectedMajor, int expectedMinor, int expectedPatch, string? expectedLabel)
    {
        var result = VersionFolder.ParseOpenVINOVersion(input);

        Assert.Equal(expectedMajor, result.Major);
        Assert.Equal(expectedMinor, result.Minor);
        Assert.Equal(expectedPatch, result.Patch);
        Assert.Equal(expectedLabel, result.ReleaseLabels.FirstOrDefault());
    }

    [Theory]
    [InlineData("")]
    [InlineData("2024")]
    public void ParseOpenVINOVersion_InvalidInput_ShouldThrowArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => VersionFolder.ParseOpenVINOVersion(input));
    }

    [Theory]
    [InlineData("abcd.ef.gh")]
    [InlineData("2024.x.0")]
    public void ParseOpenVINOVersion_InvalidInput_ShouldThrowFormatException(string input)
    {
        Assert.Throws<FormatException>(() => VersionFolder.ParseOpenVINOVersion(input));
    }
}
