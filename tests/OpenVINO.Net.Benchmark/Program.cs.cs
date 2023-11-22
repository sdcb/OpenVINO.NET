using BenchmarkDotNet.Running;

namespace OpenVINO.Net.Benchmark
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
