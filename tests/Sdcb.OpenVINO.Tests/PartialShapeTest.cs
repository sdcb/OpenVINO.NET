using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.Tests;

public class PartialShapeTest
{
    [Fact]
    public void CanCreateStatic()
    {
        using PartialShape s = new(1, 2, 3, 4);
        Assert.Equal("{1,2,3,4}", s.ToString());
    }

    [Fact]
    public void CanCreateDynamicDimension()
    {
        using PartialShape s = new(new Dimension(1), Dimension.Dynamic, new Dimension(64, 128), new Dimension(1024, 2048));
        Assert.Equal("{1,?,64..128,1024..2048}", s.ToString());
    }

    [Fact]
    public void CanCreateDynamicRank()
    {
        using PartialShape s = new(Rank.Dynamic);
        Assert.Equal("?", s.ToString());
    }

    [Fact]
    public void CheckDispose()
    {
        using PartialShape s = new(1, 3, 224, 224);
        Assert.False(s.Disposed);
        s.Dispose();
        Assert.True(s.Disposed);
    }
}
