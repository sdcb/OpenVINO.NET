using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using OpenCvSharp;

namespace OpenVINO.Net.Benchmark
{
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net70, launchCount: 1, warmupCount: 1, iterationCount: 5)]
    public class StackingVertically
    {
        static readonly string baseDir = AppContext.BaseDirectory;

        public Mat Mat;

        public List<Mat> Mats;

        [Params("test_1.jpg")]
        public string FileName { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.Mat = new Mat(Path.Combine(baseDir, "images", this.FileName));
            var matNum = 4;
            this.Mats = new List<Mat>(matNum);
            for (int i = 0; i < matNum; i++)
            {
                this.Mats.Add(this.Mat);
            }
        }

        [Benchmark(Description = "StackingVerticallyBySdcb", Baseline = true)]
        public void StackingVerticallyBySdcb()
        {
            int height = this.Mat.Height;
            int width = this.Mat.Width;

            MatType matType = this.Mats[0].Type();
            using Mat combinedMat = new(height * this.Mats.Count, width, matType, Scalar.Black);
            for (int i = 0; i < this.Mats.Count; i++)
            {
                Mat src = this.Mats[i];
                using Mat dest = combinedMat[i * height, (i + 1) * height, 0, src.Width];
                src.CopyTo(dest);
            }
        }


        [Benchmark(Description = "StackingVerticallyByAven")]
        public void StackingVerticallyByAven()
        {
            using Mat combinedMat = new();
            Cv2.VConcat(this.Mats, combinedMat);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            this.Mat?.Dispose();
            foreach (Mat mat in this.Mats)
            {
                mat.Dispose();
            }
        }
    }
}

