using System;
using BenchmarkDotNet.Running;
using neo_vm.Benchmarks.Types;

namespace neo_vm.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            VMBenchmarkBase.NeoVMCurrent = new NeoVMEngine(@"C:\Sources\Neo\neo-vm\src\neo-vm\bin\Release\net461\");
            VMBenchmarkBase.NeoVMLastRelease = new NeoVMEngine(@"C:\Sources\Neo\neo-vm\tests\neo-vm.Benchmarks\bin\Debug\net461\2.4.0\");

            foreach (var type in new Type[]
            {
                typeof(VMBenchmarkNOP),
                typeof(VMBenchmarkVERIFY),
                typeof(VMBenchmarkSHA1),
                typeof(VMBenchmarkFACTORIAL),
                typeof(VMBenchmarkFB),
                typeof(VMBenchmarkPUSH0)
            })
            {
                var summary = BenchmarkRunner.Run(type, new AllowNonOptimized());

                //PlainExporter.Default.ExportToLog(s, new ConsoleLogger(null));
                //PlainExporter.Default.ExportToFiles(s, new ConsoleLogger(null));

                Console.WriteLine(type.Name);
                Console.ReadLine();
            }
        }
    }
}