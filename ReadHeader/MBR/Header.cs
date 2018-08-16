using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DiskHeader.MBR
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
    public readonly struct Header
    {
        /// <summary>
        /// 0000 boot strap loader
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 446)]
        public readonly byte[] MasterBootStrapLoader;
        /// <summary>
        /// 01be partition table (1-4) 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly Partition[] PartitionTable;
        /// <summary>
        /// boot signature
        /// </summary>
        public readonly MBRSignature Signature;
    }
}
