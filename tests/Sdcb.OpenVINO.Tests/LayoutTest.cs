namespace Sdcb.OpenVINO.Tests;

public class LayoutTest
{
    [Fact]
    public void BasicTest()
    {
        using Layout layout = new("NCHW");
        Assert.Equal("[N,C,H,W]", layout.ToString());
    }

    [Fact]
    public void SharedNCHWTest()
    {
        using Layout layout = Layout.NCHW;
        Assert.Equal("[N,C,H,W]", layout.ToString());
    }

    [Fact]
    public void SharedNCHWTest2()
    {
        {
            using Layout layout = Layout.NCHW;
            Assert.Equal("[N,C,H,W]", layout.ToString());
        }
        {
            using Layout layout = Layout.NCHW;
            Assert.Equal("[N,C,H,W]", layout.ToString());
        }
    }

    [Fact]
    public void SharedNHWCTest()
    {
        using Layout layout = Layout.NHWC;
        Assert.Equal("[N,H,W,C]", layout.ToString());
    }
}
