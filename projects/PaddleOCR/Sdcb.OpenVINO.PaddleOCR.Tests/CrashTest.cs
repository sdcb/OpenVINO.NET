using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class CrashTest
{
    private readonly ITestOutputHelper _console;

    public CrashTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact(Skip = "Just test")]
    public async Task DetMultiInstanceCrashTest()
    {
        using Mat src = Cv2.ImRead("./samples/vsext.png");
        await Task.WhenAll(Enumerable.Range(0, 16).Select(i => Task.Run(() =>
        {
            PaddleOcrDetector det = new PaddleOcrDetector(
                OnlineDetectionModel.ChineseV4.DownloadAsync().Result
                , new DeviceOptions("CPU"),
                new Size(256, 256));
            for (int r = 0; r < 100; ++r)
            {
                using Mat result = det.RunRaw(src, out Size resizedSize);
            }
        })));
    }

    [Fact(Skip = "Just test")]
    public async Task ClsMultiInstanceCrashTest()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        await Task.WhenAll(Enumerable.Range(0, 16).Select(i => Task.Run(() =>
        {
            Stopwatch sw = Stopwatch.StartNew();
            PaddleOcrClassifier cls = new PaddleOcrClassifier(
                OnlineClassificationModel.ChineseMobileV2.DownloadAsync().Result
                , new DeviceOptions("CPU"));
            for (int r = 0; r < 100; ++r)
            {
                cls.ShouldRotate180(src);
            }
            _console.WriteLine($"thread {i} done after: {sw.Elapsed.TotalMilliseconds}ms");
        })));
    }

    [Fact(Skip = "Just test")]
    public async Task RecMultiInstanceCrashTest()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        await Task.WhenAll(Enumerable.Range(0, 16).Select(i => Task.Run(() =>
        {
            Stopwatch sw = Stopwatch.StartNew();
            PaddleOcrRecognizer rec = new PaddleOcrRecognizer(
                LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync().Result
                , new DeviceOptions("CPU"));
            for (int r = 0; r < 100; ++r)
            {
                rec.Run(src);
            }
            _console.WriteLine($"thread {i} done after: {sw.Elapsed.TotalMilliseconds}ms");
        })));
    }

    [Fact(Skip = "Just test")]
    public async Task QueuedTest()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        FullOcrModel model = await OnlineFullModels.ChineseV4.DownloadAsync();
        QueuedPaddleOcrAll queued = new(() => new PaddleOcrAll(model),
            consumerCount: 32,
            boundedCapacity: 100);

        await Task.WhenAll(Enumerable.Range(0, 200).Select(i => queued.Run(src)));
    }
}
