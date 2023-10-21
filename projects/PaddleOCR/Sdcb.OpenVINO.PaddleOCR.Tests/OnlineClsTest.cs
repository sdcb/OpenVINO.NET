using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class OnlineClsTest
{
    private readonly ITestOutputHelper _console;

    public OnlineClsTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task ClsCPU()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        using PaddleOcrClassifier cls = new(await OnlineClassificationModel.ChineseMobileV2.DownloadAsync(), new DeviceOptions("CPU"));

        ClsAsserts(cls, false, src);

        Cv2.Rotate(src, src, RotateFlags.Rotate180);
        ClsAsserts(cls, true, src);
    }

    [Fact]
    public async Task ClsGpu()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        using PaddleOcrClassifier cls = new(await OnlineClassificationModel.ChineseMobileV2.DownloadAsync(), new DeviceOptions("GPU"));
        ClsAsserts(cls, false, src);

        Cv2.Rotate(src, src, RotateFlags.Rotate180);
        ClsAsserts(cls, true, src);
    }

    private void ClsAsserts(PaddleOcrClassifier cls, bool[] rotated, Mat[] srcs)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Ocr180DegreeClsResult[] shouldRotate = cls.ShouldRotate180(srcs);
        double elapsedMs = sw.Elapsed.TotalMilliseconds;
        _console.WriteLine($"elapsed: {elapsedMs}ms, avg: {elapsedMs/srcs.Length}");
        for (int i = 0; i < shouldRotate.Length; i++)
        {
            Ocr180DegreeClsResult res = shouldRotate[i];
            _console.WriteLine($"{i}: result: {res.ShouldRotate180}, score: {res.Confidence}.");
            Assert.Equal(rotated[i], res.ShouldRotate180);
        }
    }

    private void ClsAsserts(PaddleOcrClassifier cls, bool rotated, Mat src) => ClsAsserts(cls, new[] { rotated }, new[] { src });
}
