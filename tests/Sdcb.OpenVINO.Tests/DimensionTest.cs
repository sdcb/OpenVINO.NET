using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO.Tests;

public class DimensionTest
{
    [Fact]
    public void DimensionToStringTest()
    {
        Dimension d = new(1, 10);
        Assert.Equal("1..10", d.ToString());
    }

    [Fact]
    public void RankToStringTest()
    {
        Rank d = new(1, 10);
        Assert.Equal("?", d.ToString());
    }

    [Fact]
    public void DimensionFromNative()
    {
        ov_dimension dn = new() { min = 1, max = 10 };
        Dimension d = dn;
        Assert.Equal(1, d.Min);
        Assert.Equal(10, d.Max);
    }

    [Fact]
    public void RankFromNative()
    {
        ov_dimension dn = new() { min = 1, max = 10 };
        Rank d = dn;
        Assert.Equal(1, d.Min);
        Assert.Equal(10, d.Max);
    }

    [Fact]
    public void DimensionToNative()
    {
        Dimension d = new(1, 10);
        ov_dimension dn = d;
        Assert.Equal(1, dn.min);
        Assert.Equal(10, dn.max);
    }

    [Fact]
    public void RankToNative()
    {
        Rank d = new(1, 10);
        ov_dimension dn = d;
        Assert.Equal(1, dn.min);
        Assert.Equal(10, dn.max);
    }
}
