using System.Runtime.InteropServices;

namespace DiskHeader.MBR
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
    public readonly struct Partition
    {
        [MarshalAs(UnmanagedType.U1)]
        public readonly BootFlag BootFlag;
        public readonly CHS CHSStart;
        public readonly PartitionType PartitionType;
        public readonly CHS CHSEnd;
        public readonly LBA LBAStart;
        public readonly LBA LBASectorCount;
    }
}
