using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class ClsTest
{
    [Fact]
    public async Task ClsResultIsCorrect()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        using PaddleOcrClassifier cls = new(await OnlineClassificationModel.ChineseMobileV2.DownloadAsync());
        bool shouldRotate = cls.ShouldRotate180(src);
        Assert.False(shouldRotate);

        Cv2.Rotate(src, src, RotateFlags.Rotate180);
        bool shouldRotate2 = cls.ShouldRotate180(src);
        Assert.True(shouldRotate2);
    }
}
