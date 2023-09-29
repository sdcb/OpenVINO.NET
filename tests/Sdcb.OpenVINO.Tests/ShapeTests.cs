using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO.Tests;

public class ShapeTests
{
    [Fact]
    public void CanCreateFromArray()
    {
        Shape shape = new(new long[] { 1, 2, 3, 4 });
        Assert.Equal(new long[] { 1, 2, 3, 4 }, shape.Dimensions);
        Assert.Equal(4, shape.Rank);
    }

    [Fact]
    public void CanCreateFromD4()
    {
        Shape shape = new(3, 640, 480);
        Assert.Equal(new long[] { 3, 640, 480 }, shape.Dimensions);
        Assert.Equal(3, shape.Rank);
    }

    [Fact]
    public void ToStringTest()
    {
        Shape shape = new(4, 3);
        Assert.Equal("{4,3}", shape.ToString());
    }

    [Fact]
    public unsafe void TestLock()
    {
        Shape shape = new(1, 2, 3, 4);
        using NativeShapeWrapper l = shape.Lock();
        Assert.Equal(new long[] { 1, 2, 3, 4 }, new ReadOnlySpan<long>(l.Shape.dims, (int)l.Shape.rank).ToArray());
        Assert.Equal(4, l.Shape.rank);
    }

    [Fact]
    public unsafe void TestInitFromNative()
    {
        // arrange
        ov_shape_t s = default;
        s.rank = 4;
        long* dims = stackalloc long[4] { 1, 2, 3, 4 };
        s.dims = dims;

        // act
        Shape shape = new(s);

        // assert
        Assert.Equal(new long[] { 1, 2, 3, 4 }, shape.Dimensions);
        Assert.Equal(4, shape.Rank);
    }

    [Fact]
    public void ConvertFromPartialShapeTest()
    {
        PartialShape s = new(1, 2, 3, 4);
        Shape ps = (Shape)s;
        Assert.Equal(4, ps.Rank);
        Assert.Equal(1, ps.Dimensions[0]);
        Assert.Equal(2, ps.Dimensions[1]);
        Assert.Equal(3, ps.Dimensions[2]);
        Assert.Equal(4, ps.Dimensions[3]);
    }

    [Fact]
    public void ConvertFromDynamicPartialShapeTest()
    {
        PartialShape s = PartialShape.DynamicRank;
        Assert.Throws<NotSupportedException>(() => (Shape)s);
    }

    [Fact]
    public void ConvertFromDynamicPartialShapeTest2()
    {
        PartialShape s = new(1, 2, 3, new Dimension(4, 5));
        Assert.Throws<NotSupportedException>(() => (Shape)s);
    }
}
