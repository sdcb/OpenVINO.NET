using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests;

public class OVCoreTest
{
    private readonly ITestOutputHelper _console;

    public OVCoreTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public void VersionTest()
    {
        OVVersion ver = OVCore.Version;
        _console.WriteLine(ver.ToString());
        Assert.NotNull(ver.BuildNumber);
        Assert.NotNull(ver.Description);
    }

    [Fact]
    public void DeviceVersionTest()
    {
        using OVCore c = new();
        Dictionary<string, OVVersion> versions = c.GetDevicePluginsVersions("HETERO:CPU,GPU");
        foreach (KeyValuePair<string, OVVersion> item in versions)
        {
            _console.WriteLine($"{item.Key}: {item.Value}");
        }
        Assert.NotEmpty(versions);
    }
}
