using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests.Natives;

public class VersionTest
{
    private readonly ITestOutputHelper _console;

    public VersionTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public unsafe void PrintVersion()
    {
        ov_version v = new();
        NativeMethods.ov_get_openvino_version(&v);
    }
}
