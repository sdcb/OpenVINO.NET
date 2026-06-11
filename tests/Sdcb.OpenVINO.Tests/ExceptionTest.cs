using Sdcb.OpenVINO.Natives;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests;

public class ExceptionTest
{
    private readonly ITestOutputHelper _console;

    public ExceptionTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public void InvalidModelPathThrowsOpenVINOException()
    {
        OpenVINOException ex = Assert.Throws<OpenVINOException>(() =>
        {
            using OVCore c = new();
            using CompiledModel m = c.CompileModel("INVALID-MODEL");
        });
        _console.WriteLine(ex.Message);
        Assert.Contains("INVALID-MODEL", ex.Message);
    }
}
