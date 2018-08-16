using System.Runtime.InteropServices;

namespace DiskHeader.MBR
{
    /// <summary>
    /// Cylindeer Header Sector
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public readonly struct CHS
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly byte[] Bytes;
    }
}
