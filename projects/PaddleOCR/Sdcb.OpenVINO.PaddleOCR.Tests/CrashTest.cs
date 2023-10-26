using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class CrashTest
{
    private readonly ITestOutputHelper _console;

    public CrashTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task DetMultiInstanceCrashTest()
    {
        using Mat src = Cv2.ImRead("./samples/vsext.png");
        DetectionModel model = await OnlineDetectionModel.ChineseV4.DownloadAsync();
        await Task.WhenAll(Enumerable.Range(0, Environment.ProcessorCount).Select(i => Task.Run(() =>
        {
            using PaddleOcrDetector det = new(model, new DeviceOptions("CPU"), new Size(256, 256));
            for (int r = 0; r < 100; ++r)
            {
                using Mat result = det.RunRaw(src, out Size resizedSize);
            }
        })));
    }

    [Fact]
    public async Task ClsMultiInstanceCrashTest()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        ClassificationModel model = await OnlineClassificationModel.ChineseMobileV2.DownloadAsync();
        await Task.WhenAll(Enumerable.Range(0, 16).Select(i => Task.Run(() =>
        {
            Stopwatch sw = Stopwatch.StartNew();
            using PaddleOcrClassifier cls = new(model, new DeviceOptions("CPU"));
            for (int r = 0; r < 100; ++r)
            {
                cls.ShouldRotate180(src);
            }
            _console.WriteLine($"thread {i} done after: {sw.Elapsed.TotalMilliseconds}ms");
        })));
    }

    [Fact]
    public async Task RecMultiInstanceCrashTest()
    {
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");
        RecognizationModel model = await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync();
        int threadCount = 16;
        using CountdownEvent countdownEvent = new(threadCount);
        await Task.WhenAll(Enumerable.Range(0, threadCount).Select(i => Task.Run(() =>
        {
            using Mat cloned = src.Clone();
            using PaddleOcrRecognizer rec = new(model, new DeviceOptions("CPU"), staticShapeWidth: 512);

            countdownEvent.Signal();
            countdownEvent.Wait();

            Stopwatch sw = Stopwatch.StartNew();
            for (int r = 0; r < 100; ++r)
            {
                rec.Run(cloned);
            }
            _console.WriteLine($"thread {i} done after: {sw.Elapsed.TotalMilliseconds}ms");
        })));
    }

    [Fact]
    public async Task QueuedTest()
    {
        //using Mat src = Cv2.ImRead("./samples/vsext.png"); 
        using Mat src = Cv2.ImDecode(await new HttpClient().GetByteArrayAsync("https://io.starworks.cc:88/paddlesharp/ocr/samples/xdr5450.webp"), ImreadModes.Color);

        FullOcrModel model = await OnlineFullModels.ChineseV4.DownloadAsync();
        using QueuedPaddleOcrAll queued = new(() => new PaddleOcrAll(model, new PaddleOcrOptions(new DeviceOptions("CPU"))
        {
            DetectionStaticSize = new Size(1024, 1024),
            RecognitionStaticWidth = 512,
        }), consumerCount: 8, boundedCapacity: 100);

        await Task.WhenAll(Enumerable.Range(0, 100).Select(i => queued.Run(src)));
    }
}
