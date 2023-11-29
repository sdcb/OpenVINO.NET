using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using OpenCvSharp;
using OpenCvSharp.Text;
using Sdcb.OpenVINO.Extensions.OpenCvSharp4;

namespace Sdcb.OpenVINO.Benchmarks
{
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net70, launchCount: 1, warmupCount: 1, iterationCount: 10)]
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net80, launchCount: 1, warmupCount: 1, iterationCount: 10)]
    [MemoryDiagnoser]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    public class StackingVertically
    {
        private Mat[] _srcs = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _srcs = new[] { "test_1.jpg", "test_2.jpg", "test_3.jpg", "test_4.jpg" }
                .Select(x => Cv2.ImRead(Path.Combine("images", x)))
                .ToArray();
            int maxWidth = _srcs.Max(x => x.Width);
            foreach (Mat src in _srcs)
            {
                src.CopyMakeBorder(0, 0, 0, maxWidth - src.Width, BorderTypes.Constant, Scalar.Black);
            }
        }

        [Benchmark]
        [Obsolete]
        public void StackingVerticallyBySdcb()
        {
            int maxWidth = _srcs.Max(x => x.Width);
            int height = _srcs[0].Height;
            using Mat r = _srcs.StackingVertically(height, maxWidth);
        }

        [Benchmark]
        public void StackingVerticallyByAven()
        {
            using Mat r = _srcs.StackingVertically();
        }
    }
}

