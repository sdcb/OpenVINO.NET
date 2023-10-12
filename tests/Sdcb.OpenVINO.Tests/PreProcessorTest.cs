using Sdcb.OpenVINO.Natives;
using Sdcb.OpenVINO.Tests.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests;

public class PreProcessorTest
{
    private readonly ITestOutputHelper _console;
    private readonly string _modelFile;

    public PreProcessorTest(ITestOutputHelper console)
    {
        _console = console;
        _modelFile = OVCoreNativeTest.PrepareModel();
    }

    [Fact]
    public void UpdateElementType()
    {
        using OVCore c = new();
        using Model m1 = c.ReadModel(_modelFile);
        using PrePostProcessor pp = m1.CreatePrePostProcessor();
        using PreProcessInputInfo info = pp.Inputs.Primary;
        info.TensorInfo.ElementType = ov_element_type_e.F16;
        using Model m = pp.BuildModel();
        Assert.Equal(ov_element_type_e.F16, m.Inputs.Primary.ElementType);
    }
}
