using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using OpenCvSharp;

namespace OpenVINO.Net.Benchmark
{
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net70, launchCount: 1, warmupCount: 1, iterationCount: 10)]
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net80, launchCount: 1, warmupCount: 1, iterationCount: 10)]
    [MemoryDiagnoser]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    public class StackingVertically
    {
        static readonly string baseDir = AppContext.BaseDirectory;

        public Mat Mat;

        public List<Mat> Mats;


        [GlobalSetup]
        public void GlobalSetup()
        {
            var imageList = new List<string>() { "test_1.jpg", "test_2.jpg", "test_3.jpg", "test_4.jpg" };
            this.Mats = new List<Mat>(imageList.Count); 
            
            for (int i = 0; i < imageList.Count; i++)
            {
                this.Mat = new Mat(Path.Combine(baseDir, "images", imageList[i]));
                this.Mats.Add(this.Mat);
            }
        }

        [Benchmark(Description = "StackingVerticallyBySdcb", Baseline = true)]
        [Arguments(48, 320)]
        [Arguments(48, 512)]
        public void StackingVerticallyBySdcb(int modelHeight, int maxWidth)
        {
            var paddingMats = new List<Mat>(this.Mats.Count);

            for (var i = 0; i < this.Mats.Count; i++)
            {
                paddingMats.Add(ResizePaddingSdcb(this.Mats[i], modelHeight, maxWidth));
            }

            MatType matType = paddingMats[0].Type();
            using Mat combinedMat = new(modelHeight * this.Mats.Count, maxWidth, matType, Scalar.Black);
            for (var i = 0; i < this.Mats.Count; i++)
            {
                Mat src = paddingMats[i];
                using Mat dest = combinedMat[i * modelHeight, (i + 1) * modelHeight, 0, src.Width];
                src.CopyTo(dest);
            }
        }


        [Benchmark(Description = "StackingVerticallyByAven")]
        [Arguments(48, 320)]
        [Arguments(48, 512)]
        public void StackingVerticallyByAven(int modelHeight, int maxWidth)
        {
            var paddingMats = new List<Mat>(this.Mats.Count);

            for (var i = 0; i < this.Mats.Count; i++)
            {
                paddingMats.Add(ResizePaddingAven(this.Mats[i], modelHeight, maxWidth));
            }
            MatType matType = paddingMats[0].Type();
            using Mat combinedMat = new(modelHeight * this.Mats.Count, maxWidth, matType, Scalar.Black);
            Cv2.VConcat(paddingMats, combinedMat);
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
        private static Mat ResizePaddingSdcb(Mat src, int modelHeight, int targetWidth)
        {
            // Calculate scaling factor
            double scale = Math.Min((double)modelHeight / src.Height, (double)targetWidth / src.Width);

            // Resize image
            Mat resized = new();
            Cv2.Resize(src, resized, new Size(), scale, scale);

            // Compute padding for height and width
            int padTop = Math.Max(0, (modelHeight - resized.Height) / 2);

            if (padTop > 0)
            {
                // Add padding. If padding needs to be added to top and bottom we divide it equally,
                // but if there is an odd number we add the extra pixel to the bottom.
                Mat result = new();
                int remainder = (modelHeight - resized.Height) % 2;
                Cv2.CopyMakeBorder(resized, result, padTop, padTop + remainder, 0, 0, BorderTypes.Constant, Scalar.Black);
                resized.Dispose();
                return result;
            }
            else
            {
                return resized;
            }
        }

        private static Mat ResizePaddingAven(Mat src, int modelHeight, int targetWidth)
        {
            //final image size
            var dstSize = new Size(targetWidth, modelHeight);
            var result = new Mat(dstSize, src.Type(), Scalar.Black);

            // Calculate scaling factor
            double scale = Math.Min((double)modelHeight / src.Height, (double)targetWidth / src.Width);

            // Resize image
            Mat resized = new();
            Cv2.Resize(src, resized, new Size(), scale, scale);

            // Compute padding for height and width
            int padTop = Math.Max(0, (modelHeight - resized.Height) / 2);

            if (padTop > 0)
            {
                // Add padding. If padding needs to be added to top and bottom we divide it equally,
                // but if there is an odd number we add the extra pixel to the bottom.                
                int remainder = (modelHeight - resized.Height) % 2;
                Cv2.CopyMakeBorder(resized, result, padTop, padTop + remainder, 0, dstSize.Width - resized.Width, BorderTypes.Constant, Scalar.Black);
                resized.Dispose();
                return result;
            }
            else
            {
                result[0, resized.Height, 0, resized.Width] = resized;
                resized.Dispose();
                return result;
            }
        }
    }
}

