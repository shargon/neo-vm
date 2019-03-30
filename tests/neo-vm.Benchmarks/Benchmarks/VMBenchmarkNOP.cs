using BenchmarkDotNet.Attributes;
using neo_vm.Benchmarks.Types;

namespace neo_vm.Benchmarks
{
    public class VMBenchmarkNOP : VMBenchmarkBase
    {
        [Params("NOP*1K")]
        public override string OpCodes { get; set; }

        [GlobalSetup]
        public override void Setup()
        {
            using (var script = new IsolatedScriptBuilder())
            {
                for (int x = 0; x < 1000; x++)
                {
                    script.Emit(0x61 /*NOP*/);
                }

                _script = script.ToArray();
            }

            base.Setup();
        }
    }
}