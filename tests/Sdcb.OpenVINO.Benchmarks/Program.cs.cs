using BenchmarkDotNet.Running;

namespace Sdcb.OpenVINO.Benchmarks
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
