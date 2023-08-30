using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests.Natives;

public class OVCoreTest
{
    private readonly ITestOutputHelper _console;

    public OVCoreTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public unsafe void OVCoreCanCreated()
    {
        ov_core* core = null;
        try
        {
            CheckResult(NativeMethods.ov_core_create(&core));
        }
        finally
        {
            NativeMethods.ov_core_free(core);
        }
    }

    static void CheckResult(ov_status_e e)
    {
        Assert.Equal(ov_status_e.OK, e);
    }
}
