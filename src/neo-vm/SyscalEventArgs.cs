namespace Neo.VM
{
    public class SyscalEventArgs
    {
        /// <summary>
        /// Method
        /// </summary>
        public uint Method { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public bool? Result { get; set; }
    }
}