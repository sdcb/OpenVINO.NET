namespace Sdcb.OpenVINO.Tests;

public class ShapeTests
{
    [Fact]
    public void CanCreateFromArray()
    {
        using Shape shape = Shape.From(new long[] { 1, 2, 3, 4 });
        Assert.Equal(new long[] { 1, 2, 3, 4 }, shape.ToArray());
    }

    [Fact]
    public void CanCreateFromD4()
    {
        using Shape shape = Shape.From(4, 3, 2, 1);
        Assert.Equal(new long[] { 4, 3, 2, 1 }, shape.ToArray());
    }
}
