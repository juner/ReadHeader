using System.Runtime.InteropServices;

namespace DiskHeader.MBR
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
    public readonly struct Partition
    {
        public readonly static Partition Empty = new Partition();
        [MarshalAs(UnmanagedType.U1)]
        public readonly BootFlag Active;
        public readonly CHS Begin;
        public readonly PartitionType Type;
        public readonly CHS End;
        public readonly LBA4 Start;
        public readonly LBA4 Length;
    }
}
