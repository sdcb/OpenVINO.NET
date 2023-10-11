using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class OnlineDetectionTest
{
    [Fact]
    public async Task DetectSimple()
    {
        using Mat src = Cv2.ImRead("./samples/vsext.png");
        using PaddleOcrDetector det = new(await OnlineDetectionModel.ChineseV4.DownloadAsync());
        RotatedRect[] results =  det.Run(src);
        Assert.NotEmpty(results);
    }

    [Fact]
    public async Task DetectSizeNull()
    {
        using Mat src = Cv2.ImRead("./samples/xdr5450.webp");
        using PaddleOcrDetector det = new(await OnlineDetectionModel.ChineseV4.DownloadAsync())
        {
            MaxSize = null
        };
        RotatedRect[] results = det.Run(src);
        //PaddleOcrDetector.Visualize(src, results, Scalar.Red, 2).ImWrite("test.jpg");
        Assert.NotEmpty(results);
    }

    [Fact]
    public async Task SpecifySize()
    {
        using Mat src = Cv2.ImRead("./samples/xdr5450.webp");
        using PaddleOcrDetector det = new(await OnlineDetectionModel.ChineseV4.DownloadAsync());
        det.MaxSize = null;
        RotatedRect[] results = det.Run(src);
        //PaddleOcrDetector.Visualize(src, results, Scalar.Red, 2).ImWrite("test.jpg");
        Assert.NotEmpty(results);
    }

    [Fact(Skip = "Too slow")]
    public async Task DetectGPU()
    {
        using Mat src = Cv2.ImRead("./samples/vsext.png");
        using PaddleOcrDetector det = new(
            await OnlineDetectionModel.ChineseV3.DownloadAsync(), 
            new DeviceOptions("GPU"))
        {
            MaxSize = 1536
        };
        RotatedRect[] results = det.Run(src);
        PaddleOcrDetector.Visualize(src, results, Scalar.Red, 2).ImWrite("test.jpg");
        Assert.NotEmpty(results);
    }
}
