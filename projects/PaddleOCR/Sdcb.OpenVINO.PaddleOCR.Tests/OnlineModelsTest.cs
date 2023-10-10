using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class OnlineModelsTest
{
    private readonly ITestOutputHelper _console;

    public OnlineModelsTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task FastCheckOCR()
    {
        FullOcrModel model = await OnlineFullModels.EnglishV3.DownloadAsync();

        // from: https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png
        byte[] sampleImageData = File.ReadAllBytes(@"./samples/vsext.png");

        using (PaddleOcrAll all = new(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = false,
        })
        {
            // Load local file by following code:
            // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
            using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
            {
                PaddleOcrResult result = all.Run(src);
                _console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    _console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                }
            }
        }
    }
}