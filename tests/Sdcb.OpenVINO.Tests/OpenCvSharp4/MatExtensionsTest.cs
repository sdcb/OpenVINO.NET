using OpenCvSharp;
using Sdcb.OpenVINO.Extensions.OpenCvSharp4;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests.OpenCvSharp4;

public class MatExtensionsTest
{
    private readonly ITestOutputHelper _console;

    public MatExtensionsTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public unsafe void WeakRefedMatDataPtrIsSame()
    {
        byte* dataPtr = stackalloc byte[10];
        IntPtr data = (IntPtr)dataPtr;
        using Mat src = Mat.FromPixelData(1, 10, MatType.CV_8SC1, data);
        using Mat src2 = src.FastClone();
        Assert.Equal(src.Data, src2.Data);
    }

    [Fact]
    public unsafe void DisposeWeakRefedShouldNotEffectData()
    {
        byte* dataPtr = stackalloc byte[10];
        dataPtr[0] = 1;
        dataPtr[1] = 2;
        dataPtr[9] = 10;
        IntPtr data = (IntPtr)dataPtr;
        using Mat src = Mat.FromPixelData(1, 10, MatType.CV_8SC1, data);
        using (Mat src2 = src.FastClone())
        {
        }

        Assert.False(src.IsDisposed);
        Span<byte> srcSpan = src.AsSpan<byte>();
        Assert.Equal(1, srcSpan[0]);
        Assert.Equal(2, srcSpan[1]);
        Assert.Equal(10, srcSpan[9]);
    }

    [Fact]
    public unsafe void RoiTest()
    {
        byte* dataPtr = stackalloc byte[10];
        dataPtr[0] = 1;
        dataPtr[1] = 2;
        dataPtr[9] = 10;
        IntPtr data = (IntPtr)dataPtr;
        using Mat src = Mat.FromPixelData(2, 5, MatType.CV_8SC1, data);
        using Mat srcRoi = src[0, 2, 4, 5];
        using Mat mat = srcRoi.FastClone();
        Assert.Equal(10, mat.At<byte>(1, 0));
    }
}
