namespace Sdcb.OpenVINO.Tests;

public class DeviceOptionsTest
{
    [Fact]
    public void CanUseProperties()
    {
        DeviceOptions options = new()
        {
            InferenceNumThreads = 1,
            PerformanceMode = PerformanceMode.Throughput
        };

        Assert.Equal("1", options.Properties[PropertyKeys.InferenceNumThreads]);
        Assert.Equal("THROUGHPUT", options.Properties[PropertyKeys.HintPerformanceMode]);
    }

    [Fact]
    public void CanDeleteProperty()
    {
        DeviceOptions options = new()
        {
            InferenceNumThreads = 1,
            PerformanceMode = PerformanceMode.Throughput
        };
        options.InferenceNumThreads = null;

        Assert.False(options.Properties.ContainsKey(PropertyKeys.InferenceNumThreads));
    }
}
