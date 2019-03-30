using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace neo_vm.Benchmarks.Types
{
    public class IsolatedScriptBuilder : IDisposable
    {
        private const byte OpCode_APPCALL = 0x67;
        private const byte OpCode_SYSCALL = 0x68;
        private const byte OpCode_TAILCALL = 0x69;

        private const byte OpCode_JMP = 0x62;
        private const byte OpCode_JMPIF = 0x63;
        private const byte OpCode_JMPIFNOT = 0x64;
        private const byte OpCode_CALL = 0x65;

        private const byte OpCode_PUSH0 = 0x00;
        private const byte OpCode_PUSH1 = 0x51;
        private const byte OpCode_PUSHM1 = 0x4F;
        private const byte OpCode_PUSHBYTES75 = 0x4B;
        private const byte OpCode_PUSHDATA1= 0x4C;
        private const byte OpCode_PUSHDATA2= 0x4D;
        private const byte OpCode_PUSHDATA4= 0x4E;

        private const byte OpCode_PUSHT = 0x51;
        private const byte OpCode_PUSHF = 0x00;

        private readonly MemoryStream ms = new MemoryStream();
        private readonly BinaryWriter writer;

        public int Offset => (int)ms.Position;

        public IsolatedScriptBuilder()
        {
            this.writer = new BinaryWriter(ms);
        }

        public void Dispose()
        {
            writer.Dispose();
            ms.Dispose();
        }

        public IsolatedScriptBuilder Emit(byte[] arg = null)
        {
            if (arg != null)
                writer.Write(arg);
            return this;
        }

        public IsolatedScriptBuilder Emit(byte op, byte[] arg = null)
        {
            writer.Write(op);
            if (arg != null)
                writer.Write(arg);
            return this;
        }

        public IsolatedScriptBuilder EmitAppCall(byte[] scriptHash, bool useTailCall = false)
        {
            if (scriptHash.Length != 20)
                throw new ArgumentException();
            return Emit(useTailCall ? OpCode_TAILCALL : OpCode_APPCALL, scriptHash);
        }

        public IsolatedScriptBuilder EmitJump(byte op, short offset)
        {
            if (op != OpCode_JMP && op != OpCode_JMPIF && op != OpCode_JMPIFNOT && op != OpCode_CALL)
                throw new ArgumentException();
            return Emit(op, BitConverter.GetBytes(offset));
        }

        public IsolatedScriptBuilder EmitPush(BigInteger number)
        {
            if (number == -1) return Emit(OpCode_PUSHM1);
            if (number == 0) return Emit(OpCode_PUSH0);
            if (number > 0 && number <= 16) return Emit((byte)(OpCode_PUSH1 - 1 + (byte)number));
            return EmitPush(number.ToByteArray());
        }

        public IsolatedScriptBuilder EmitPush(bool data)
        {
            return Emit(data ? OpCode_PUSHT : OpCode_PUSHF);
        }

        public IsolatedScriptBuilder EmitPush(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();
            if (data.Length <= (int)OpCode_PUSHBYTES75)
            {
                writer.Write((byte)data.Length);
                writer.Write(data);
            }
            else if (data.Length < 0x100)
            {
                Emit(OpCode_PUSHDATA1);
                writer.Write((byte)data.Length);
                writer.Write(data);
            }
            else if (data.Length < 0x10000)
            {
                Emit(OpCode_PUSHDATA2);
                writer.Write((ushort)data.Length);
                writer.Write(data);
            }
            else// if (data.Length < 0x100000000L)
            {
                Emit(OpCode_PUSHDATA4);
                writer.Write(data.Length);
                writer.Write(data);
            }
            return this;
        }

        public IsolatedScriptBuilder EmitPush(string data)
        {
            return EmitPush(Encoding.UTF8.GetBytes(data));
        }

        public IsolatedScriptBuilder EmitSysCall(string api)
        {
            if (api == null)
                throw new ArgumentNullException();
            byte[] api_bytes = Encoding.ASCII.GetBytes(api);
            if (api_bytes.Length == 0 || api_bytes.Length > 252)
                throw new ArgumentException();
            byte[] arg = new byte[api_bytes.Length + 1];
            arg[0] = (byte)api_bytes.Length;
            Buffer.BlockCopy(api_bytes, 0, arg, 1, api_bytes.Length);

            return Emit(OpCode_SYSCALL, arg);
        }

        public byte[] ToArray()
        {
            writer.Flush();
            return ms.ToArray();
        }
    }
}