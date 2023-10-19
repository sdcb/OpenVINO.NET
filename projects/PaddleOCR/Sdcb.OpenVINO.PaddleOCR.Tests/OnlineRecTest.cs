using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class OnlineRecTest
{
    [Fact]
    public async Task RecResultIsCorrect()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        using PaddleOcrRecognizer rec = new(await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync());
        PaddleOcrRecognizerResult result = rec.Run(src);
        Assert.Equal("5GHz频段流数多一倍", result.Text);
        Assert.True(result.Score > 0.9);
    }
}
