using System.Runtime.InteropServices;

namespace DiskHeader
{
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public readonly struct LBA8
    {
        public readonly ulong Bytes;
        private LBA8(ulong Bytes) => this.Bytes = Bytes;
        public static explicit operator ulong(LBA8 self) => self.Bytes;
        public static explicit operator LBA8(ulong self) => new LBA8(self);
        public override string ToString()
            => $"{nameof(LBA8)}(0x{Bytes:X16})";
    }
}
