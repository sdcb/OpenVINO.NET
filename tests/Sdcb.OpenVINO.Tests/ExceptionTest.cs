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
    public void Exception2320()
    {
        if (OpenVINOLibraryLoader.Is202302OrGreater())
        {
            OpenVINOException ex = Assert.Throws<OpenVINOException>(() =>
            {
                using OVCore c = new();
                using CompiledModel m = c.CompileModel("INVALID-MODEL");
            });
            _console.WriteLine(ex.Message);
            Assert.Contains("INVALID-MODEL", ex.Message);
        }
        else
        {
            _console.WriteLine($"Case {nameof(Exception2320)} invalid in version {OpenVINOLibraryLoader.VersionAbbr}");
        }
    }

    [Fact]
    public void Exception2310()
    {
        if (!OpenVINOLibraryLoader.Is202302OrGreater())
        {
            _console.WriteLine($"Invalid test in version {OpenVINOLibraryLoader.VersionAbbr}");
            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(() =>
            {
                using OVCore c = new();
                using CompiledModel m = c.CompileModel("INVALID-MODEL");
            });
            _console.WriteLine(ex.Message);
            Assert.Contains("Model path not found", ex.Message);
        }
        else
        {
            _console.WriteLine($"Case {nameof(Exception2310)} invalid in version {OpenVINOLibraryLoader.VersionAbbr}");
        }
    }
}
