using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class DetectionStaticTest
{
    private readonly ITestOutputHelper _console;

    public DetectionStaticTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task SmallerImageShouldNotScaleUp()
    {
        using Mat src = Cv2.ImRead("./samples/vsext.png");
        using PaddleOcrDetector det = new (await OnlineDetectionModel.ChineseV4.DownloadAsync(), new DeviceOptions("CPU"), staticShapeSize: src.Size());

        Assert.True(det.StaticShapeSize.HasValue);
        Assert.True(det.StaticShapeSize.Value.Width > src.Width || det.StaticShapeSize.Value.Height > src.Height);

        using Mat detected = det.RunRaw(src, out Size resizedSize);
        Assert.Equal(src.Size(), resizedSize);
    }

    [Fact]
    public async Task LargerImageShouldScaleDown()
    {
        using Mat src = Cv2.ImRead("./samples/vsext.png");
        Size srcSize = src.Size();
        Size smallerSize = new(srcSize.Width - 32, srcSize.Height - 32);
        using PaddleOcrDetector det = new(await OnlineDetectionModel.ChineseV4.DownloadAsync(), new DeviceOptions("CPU"), staticShapeSize: smallerSize);

        using Mat detected = det.RunRaw(src, out Size resizedSize);
        Size targetSize = det.StaticShapeSize!.Value;
        Assert.True(
            (resizedSize.Width == targetSize.Width && resizedSize.Height < targetSize.Height) ^ 
            (resizedSize.Height == targetSize.Height && resizedSize.Width < targetSize.Width));
    }
}
