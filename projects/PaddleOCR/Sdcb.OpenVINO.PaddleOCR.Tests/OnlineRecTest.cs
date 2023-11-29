using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class OnlineRecTest
{
    private readonly ITestOutputHelper _console;

    public OnlineRecTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task RecCPU()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync());
        RecAsserts(rec, src);
    }

    [Fact]
    public async Task RecCPUStaticShape()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        int staticWidth = (int)(1.0 * src.Width / src.Height * 48);
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync(), staticShapeWidth: staticWidth);
        Assert.Equal(416, rec.StaticShapeWidth);
        RecAsserts(rec, src);
    }

    [Fact]
    public async Task RecCPULargerShape()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        int staticWidth = (int)(1.0 * src.Width / src.Height * 48);
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync(), staticShapeWidth: staticWidth + 32);
        RecAsserts(rec, src);
    }

    [Fact]
    public async Task RecCPULargerBatchShape()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        using Mat src2 = src.CopyMakeBorder(0, 0, 0, 100, BorderTypes.Constant);
        int staticWidth = (int)(1.0 * src.Width / src.Height * 48);
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync(), staticShapeWidth: staticWidth + 32);
        RecAsserts(rec, src, src2);
    }

    [Fact]
    public async Task RecCPUSmallerShape()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        int staticWidth = (int)(1.0 * src.Width / src.Height * 48);
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync(), staticShapeWidth: staticWidth - 32);
        RecAsserts(rec, src);
    }

    [Fact(Skip = "GPU too slow")]
    public async Task RecGpu()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        int staticWidth = (int)(1.0 * src.Width / src.Height * 48);
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync(), new DeviceOptions("GPU"), staticShapeWidth: staticWidth);
        RecAsserts(rec, src);
    }

    [Fact(Skip = "GPU too slow")]
    public async Task RecGpuLargerBatchShape()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        using Mat src2 = src.CopyMakeBorder(0, 0, 0, 100, BorderTypes.Constant);
        int staticWidth = (int)(1.0 * src.Width / src.Height * 48);
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync(), new DeviceOptions("GPU"), staticShapeWidth: staticWidth);
        RecAsserts(rec, src, src2);
    }

    private void RecAsserts(PaddleOcrRecognizer rec, params Mat[] srcs)
    {
        Stopwatch sw = Stopwatch.StartNew();
        PaddleOcrRecognizerResult[] results = rec.Run(srcs);
        _console.WriteLine($"elapsed={sw.Elapsed.TotalMilliseconds}ms");
        foreach (PaddleOcrRecognizerResult result in results)
        {
            _console.WriteLine($"score: {result.Score}, text: {result.Text}");
            Assert.Equal("5GHz频段流数多一倍", result.Text);
            Assert.True(result.Score > 0.9);
        }
    }

    [Theory]
    [InlineData(27, 294)]
    [InlineData(48, 1024)]
    [InlineData(30, 512)]
    public void ResizePaddingTests(int srcHeight, int srcWidth)
    {
        int modelHeight = 48;
        int targetWidth = 512;
        using Mat src = new(srcHeight, srcWidth, MatType.CV_8UC1);
        using Mat res = PaddleOcrRecognizer.ResizePadding(src, modelHeight, targetWidth);
        Assert.Equal(targetWidth, res.Width);
        Assert.Equal(modelHeight, res.Height);
    }
}
