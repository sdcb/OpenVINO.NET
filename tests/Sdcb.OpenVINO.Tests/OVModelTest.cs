using Sdcb.OpenVINO.Tests.Natives;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests;

public class OVModelTest
{
    private readonly ITestOutputHelper _console;
    private readonly string _modelFile;

    public OVModelTest(ITestOutputHelper console)
    {
        _console = console;
        _modelFile = OVCoreNativeTest.PrepareModel();
    }

    [Fact]
    public void CanRead()
    {
        using OVCore c = new();
        using Model m = c.ReadModel(_modelFile);
        Assert.NotNull(m);
        Assert.NotEmpty(m.Inputs);
        Assert.NotNull(m.Inputs.Primary);
        Assert.NotEmpty(m.Outputs);
        Assert.NotNull(m.Outputs.Primary);
        Assert.NotNull(m.FriendlyName);
    }

    [Fact]
    public void ReshapeByTensorNames()
    {
        using OVCore c = new();
        using Model m = c.ReadModel(_modelFile);
        using IOPort inputPort = m.Inputs.Primary;
        Assert.Equal("x", inputPort.Name);
        Assert.Equal("{?,3,?,?}", inputPort.PartialShape.ToString());

        m.ReshapeByTensorNames(("x", new PartialShape(1, 3, 256, 320)));
        Assert.Equal("{1,3,256,320}", inputPort.PartialShape.ToString());
    }

    [Fact]
    public void ReshapeByTensorName()
    {
        using OVCore c = new();
        using Model m = c.ReadModel(_modelFile);
        using IOPort inputPort = m.Inputs.Primary;
        Assert.Equal("x", inputPort.Name);
        Assert.Equal("{?,3,?,?}", inputPort.PartialShape.ToString());

        m.ReshapeByTensorName("x", new PartialShape(1, 3, new Dimension(32, 320), 320));
        Assert.Equal("{1,3,32..320,320}", inputPort.PartialShape.ToString());
    }

    [Fact]
    public void ReshapeByPorts()
    {
        using OVCore c = new();
        using Model m = c.ReadModel(_modelFile);
        using IOPort inputPort = m.Inputs.Primary;
        Assert.Equal("x", inputPort.Name);
        Assert.Equal("{?,3,?,?}", inputPort.PartialShape.ToString());

        m.ReshapeByPorts((inputPort, new PartialShape(new Dimension(1, 10), 3, new (32, 320), 320)));
        Assert.Equal("{1..10,3,32..320,320}", inputPort.PartialShape.ToString());
    }

    [Fact]
    public void ReshapeByPortIndexes()
    {
        using OVCore c = new();
        using Model m = c.ReadModel(_modelFile);
        using IOPort inputPort = m.Inputs.Primary;
        Assert.Equal("x", inputPort.Name);
        Assert.Equal("{?,3,?,?}", inputPort.PartialShape.ToString());

        m.ReshapeByPortIndexes(new()
        {
            [0] = new PartialShape(new Dimension(1, 10), 3, new(32, 320), 320),
        });
        Assert.Equal("{1..10,3,32..320,320}", inputPort.PartialShape.ToString());
    }

    [Fact]
    public void ReshapePrimaryInput()
    {
        using OVCore c = new();
        using Model m = c.ReadModel(_modelFile);
        using IOPort inputPort = m.Inputs.Primary;
        Assert.Equal("x", inputPort.Name);
        Assert.Equal("{?,3,?,?}", inputPort.PartialShape.ToString());

        m.ReshapePrimaryInput(new PartialShape(new Dimension(1, 10), 3, new(32, 320), 320));
        Assert.Equal("{1..10,3,32..320,320}", inputPort.PartialShape.ToString());
    }
}
