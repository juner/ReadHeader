using System.Runtime.InteropServices;

namespace DiskHeader.MBR
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly struct LBA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] Bytes;
    }
}
