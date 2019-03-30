using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;

namespace neo_vm.Benchmarks.Types
{
    // https://benchmarkdotnet.org/articles/configs/exporters.html#plots

    //[CoreJob]
    //[SimpleJob(3, 2, 10, 1000)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    [RPlotExporter, RankColumn(NumeralSystem.Arabic)]
    public abstract class VMBenchmarkBase
    {
        #region Params

        protected byte[] _script;

        [Params(nameof(VMBenchmarkBase))]
        public abstract string OpCodes { get; set; }

        public static NeoVMEngine NeoVMLastRelease { get; set; }
        public static NeoVMEngine NeoVMCurrent { get; set; }

        #endregion

        #region Benchmarks

        [Benchmark(Description = "Last Release of NeoVM")]
        public virtual void NeoVM_LastRelease() => NeoVMLastRelease.LoadAndExecute(_script);

        [Benchmark(Description = "Current code of NeoVM")]
        public virtual void NeoVM_CurrentCode() => NeoVMCurrent.LoadAndExecute(_script);

        #endregion

        /// <summary>
        /// Test setup
        /// </summary>
        public virtual void Setup()
        {
            
        }
    }
}