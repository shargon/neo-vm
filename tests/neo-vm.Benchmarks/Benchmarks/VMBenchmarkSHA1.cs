using BenchmarkDotNet.Attributes;
using neo_vm.Benchmarks.Types;

namespace neo_vm.Benchmarks
{
    public class VMBenchmarkSHA1 : VMBenchmarkBase
    {
        [Params("SHA1*1K")]
        public override string OpCodes { get; set; }

        [GlobalSetup]
        public override void Setup()
        {
            using (var script = new IsolatedScriptBuilder())
            {
                for (int x = 0; x < 1000; x++)
                {
                    script.Emit(0x01/*PUSHBYTES1*/);
                    script.Emit(new byte[] { 0x01 });
                    script.Emit(0xA7 /*SHA1*/);
                    script.Emit(0x75 /*DROP*/);
                }

                _script = script.ToArray();
            }

            base.Setup();
        }
    }
}