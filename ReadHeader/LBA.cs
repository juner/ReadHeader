using System.Runtime.InteropServices;

namespace DiskHeader
{
    /// <summary>
    /// Logical Block Address
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly struct LBA
    {
        public readonly uint Bytes;
        private LBA(uint Bytes) => this.Bytes = Bytes;
        public static implicit operator uint(LBA self) => self.Bytes;
        public static implicit operator LBA(uint self) => new LBA(self);
        public override string ToString()
            => $"{nameof(LBA)}(0x{Bytes:X8})";
    }
}
