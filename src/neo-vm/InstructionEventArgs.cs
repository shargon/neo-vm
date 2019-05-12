namespace Neo.VM
{
    public class InstructionEventArgs
    {
        /// <summary>
        /// Context
        /// </summary>
        public ExecutionContext Context { get; set; }

        /// <summary>
        /// Instruction
        /// </summary>
        public Instruction Instruction { get; set; }
    }
}