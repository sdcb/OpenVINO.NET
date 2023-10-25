using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO.Tests;

public class PartialShapeTest
{
    [Fact]
    public void CanCreateStatic()
    {
        PartialShape s = new(1, 2, 3, 4);
        Assert.Equal("{1,2,3,4}", s.ToString());
    }

    [Fact]
    public void CanCreateDynamicDimension()
    {
        PartialShape s = new(new Dimension(1), Dimension.Dynamic, new Dimension(64, 128), new Dimension(1024, 2048));
        Assert.Equal("{1,?,64..128,1024..2048}", s.ToString());
    }

    [Fact]
    public void CanCreateDynamicRank()
    {
        PartialShape s = PartialShape.DynamicRank;
        Assert.Equal("?", s.ToString());
    }

    [Fact]
    public void FromNativeDynamic()
    {
        ov_partial_shape s = default;
        s.rank = new ov_dimension { min = -1, max = -1 };

        PartialShape ps = new(s);

        Assert.True(ps.Rank.IsDynamic);
    }

    [Fact]
    public unsafe void LockTest()
    {
        PartialShape ps = new(1, 3, 224, new Dimension(512, 1024));

        using NativePartialShapeWrapper l = ps.Lock();

        Assert.Equal(4, l.PartialShape.rank.min);
        Assert.Equal(4, l.PartialShape.rank.max);

        Assert.Equal(1, l.PartialShape.dims[0].min);
        Assert.Equal(1, l.PartialShape.dims[0].max);

        Assert.Equal(3, l.PartialShape.dims[1].min);
        Assert.Equal(3, l.PartialShape.dims[1].max);

        Assert.Equal(224, l.PartialShape.dims[2].min);
        Assert.Equal(224, l.PartialShape.dims[2].max);

        Assert.Equal(512, l.PartialShape.dims[3].min);
        Assert.Equal(1024, l.PartialShape.dims[3].max);
    }

    [Fact]
    public unsafe void FromNativeStatic()
    {
        ov_partial_shape s = default;
        s.rank = new ov_dimension { min = 4, max = 4 };
        ov_dimension* dims = stackalloc ov_dimension[4];
        dims[0] = new ov_dimension { min = 1, max = 10 };
        dims[1] = new ov_dimension { min = 3, max = 3 };
        dims[2] = new ov_dimension { min = 640, max = 640 };
        dims[3] = new ov_dimension { min = 480, max = 480 };
        s.dims = dims;

        PartialShape ps = new(s);

        Assert.False(ps.Rank.IsDynamic);
        Assert.Equal(4, ps.Rank.Max);
        Assert.Equal("{1..10,3,640,480}", ps.ToString());
    }

    [Fact]
    public void ConvertFromShapeTest()
    {
        Shape s = new(1, 2, 3, 4);
        PartialShape ps = s;
        Assert.False(ps.Rank.IsDynamic);
        Assert.Equal(4, ps.Rank.Max);
        Assert.Equal(1, ps.Dimensions[0].Max);
        Assert.Equal(2, ps.Dimensions[1].Max);
        Assert.Equal(3, ps.Dimensions[2].Max);
        Assert.Equal(4, ps.Dimensions[3].Max);
    }

    [Fact]
    public void ToStringTest()
    {
        PartialShape ps = new (new Dimension(0, -1), 3, new Dimension(0, -1), new Dimension(0, -1));
        Assert.Equal("{?,3,?,?}", ps.ToString());
    }

    [Fact]
    public void TestIsCompatible_WithSameRankDimensions_ShouldReturnTrue()
    {
        var ps1 = new PartialShape(new Rank(4), new Dimension(1), new Dimension(2), new Dimension(3), new Dimension(4));
        var ps2 = new PartialShape(new Rank(4), new Dimension(1), new Dimension(2), new Dimension(3), new Dimension(4));

        Assert.True(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_WithDifferentRankDimensions_ShouldReturnFalse()
    {
        var ps1 = new PartialShape(new Rank(4), new Dimension(1), new Dimension(2), new Dimension(3), new Dimension(4));
        var ps2 = new PartialShape(new Rank(3), new Dimension(1), new Dimension(2), new Dimension(3));

        Assert.False(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_WithDifferentDimensions_ShouldReturnFalse()
    {
        var ps1 = new PartialShape(new Rank(4), new Dimension(1), new Dimension(2), new Dimension(3), new Dimension(4));
        var ps2 = new PartialShape(new Rank(4), new Dimension(1), new Dimension(2), new Dimension(3), new Dimension(5));

        Assert.False(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_WithDynamicRank_ShouldReturnTrue()
    {
        var ps1 = new PartialShape(Rank.Dynamic);
        var ps2 = new PartialShape(new Rank(4), new Dimension(1), new Dimension(2), new Dimension(3), new Dimension(4));

        Assert.True(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_WithDynamicThisAndStaticOther_ShouldReturnTrue()
    {
        var ps1 = new PartialShape(Rank.Dynamic);
        var ps2 = new PartialShape(new Rank(3), new Dimension(1), new Dimension(2), new Dimension(3));

        Assert.True(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_WithStaticThisAndDynamicOther_ShouldReturnTrue()
    {
        var ps1 = new PartialShape(new Rank(3), new Dimension(1), new Dimension(2), new Dimension(3));
        var ps2 = new PartialShape(Rank.Dynamic);

        Assert.True(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_BothDynamic_ShouldReturnTrue()
    {
        var ps1 = new PartialShape(Rank.Dynamic);
        var ps2 = new PartialShape(Rank.Dynamic);

        Assert.True(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_WithOneDynamicDimension_ShouldReturnTrue()
    {
        var ps1 = new PartialShape(new Rank(3), new Dimension(1), Dimension.Dynamic, new Dimension(3));
        var ps2 = new PartialShape(new Rank(3), new Dimension(1), new Dimension(2), new Dimension(3));

        Assert.True(ps1.IsCompatible(ps2));
    }

    [Fact]
    public void TestIsCompatible_WithAllDynamicDimensions_ShouldReturnTrue()
    {
        var ps1 = new PartialShape(new Rank(3), Dimension.Dynamic, Dimension.Dynamic, Dimension.Dynamic);
        var ps2 = new PartialShape(new Rank(3), new Dimension(1), new Dimension(2), new Dimension(3));

        Assert.True(ps1.IsCompatible(ps2));
    }
}
