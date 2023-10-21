using OpenCvSharp;
using Sdcb.OpenVINO.Extensions.OpenCvSharp4;

namespace Sdcb.OpenVINO.Tests.OpenCvSharp4;

public class MatExtensionsTest
{
    [Fact]
    public unsafe void WeakRefedMatDataPtrIsSame()
    {
        byte* dataPtr = stackalloc byte[10];
        IntPtr data = (IntPtr)dataPtr;
        using Mat src = new(1, 10, MatType.CV_8SC1, data);
        using Mat src2 = src.WeakRef();
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
        using Mat src = new Mat(1, 10, MatType.CV_8SC1, data);
        using (Mat src2 = src.WeakRef())
        {
        }

        Assert.False(src.IsDisposed);
        Span<byte> srcSpan = src.AsSpan<byte>();
        Assert.Equal(1, srcSpan[0]);
        Assert.Equal(2, srcSpan[1]);
        Assert.Equal(10, srcSpan[9]);
    }
}
