using System.Runtime.InteropServices;

namespace DiskHeader
{
    /// <summary>
    /// Logical Block Address
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly struct LBA4
    {
        public readonly uint Bytes;
        private LBA4(uint Bytes) => this.Bytes = Bytes;
        public static explicit operator uint(LBA4 self) => self.Bytes;
        public static explicit operator LBA4(uint self) => new LBA4(self);
        public override string ToString()
            => $"{nameof(LBA4)}(0x{Bytes:X8})";
    }
}
