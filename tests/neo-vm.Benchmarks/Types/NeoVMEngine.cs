using System;
using System.IO;
using System.Reflection;

namespace neo_vm.Benchmarks.Types
{
    public class NeoVMEngine : IDisposable
    {
        /// <summary>
        /// Application domain
        /// </summary>
        public AppDomain Domain { get; }

        private Type _executionEngineType;
        private MethodInfo _loadScriptMethod;
        private MethodInfo _executeMethod;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path</param>
        public NeoVMEngine(string path)
        {
            var vmFile = Path.Combine(path, "Neo.VM.dll");

            if (!File.Exists(vmFile)) throw new ArgumentException("VM File not found at: " + vmFile);

            AppDomainSetup domainInfo = new AppDomainSetup
            {
                ApplicationBase = path
            };

            Domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), AppDomain.CurrentDomain.Evidence, domainInfo);

            Domain.AssemblyLoad += Domain_AssemblyLoad;
            Domain.AssemblyResolve += Domain_AssemblyResolve;

            var asmName = AssemblyName.GetAssemblyName(vmFile);

            var asm = Domain.Load(asmName);

            // Cache methods

            _executionEngineType = asm.GetType("ExecutionEngine");
            _loadScriptMethod = _executionEngineType.GetMethod("LoadScript", new Type[] { typeof(byte[]), typeof(int) });
            _executeMethod = _executionEngineType.GetMethod("Execute", new Type[] { });
        }

        private Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return args.RequestingAssembly;
        }

        private void Domain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            
        }

        /// <summary>
        /// Load and execute
        /// </summary>
        /// <param name="script">Script</param>
        public void LoadAndExecute(byte[] script)
        {
            object crypto = null;
            var engine = (IDisposable)Activator.CreateInstance(_executionEngineType, new object[] { null, crypto, null, null });

            using (engine)
            {
                _loadScriptMethod.Invoke(engine, new object[] { script, -1 });
                _executeMethod.Invoke(engine, new object[] { });
            }
        }

        /// <summary>
        /// Free resources
        /// </summary>
        public void Dispose()
        {
            AppDomain.Unload(Domain);
        }
    }
}