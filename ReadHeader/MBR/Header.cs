using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace DiskHeader.MBR
{
    /// <summary>
    /// MBR Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
    public readonly struct Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 446)]
        private readonly byte[] _MasterBootStrapLoader;
        /// <summary>
        /// 0000 boot strap loader
        /// </summary>
        public byte[] MasterBootStrapLoader => (_MasterBootStrapLoader ?? Enumerable.Empty<byte>()).Take(446).ToArray();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        /// <summary>
        /// 01be partition table (1-4) 
        /// </summary>
        private readonly Partition[] _PartitionTable;
        public Partition[] PartitionTable => (_PartitionTable ?? Enumerable.Empty<Partition>()).Take(4).ToArray();
        /// <summary>
        /// boot signature
        /// </summary>
        public readonly MBRSignature Signature;
        public Header(byte[] MasterBootStrapLoader, Partition[] PartitionTable, MBRSignature Signature)
            => (this._MasterBootStrapLoader, this._PartitionTable, this.Signature)
            = ((MasterBootStrapLoader?? Enumerable.Repeat((byte)0x00,446)).ToArray(), (PartitionTable ?? Enumerable.Repeat(new Partition(),4)).ToArray(), Signature);
        public bool IsValidSignature => Signature == MBRSignature.MagicNumber;
        public bool IsGPT => PartitionTable.Any(v => v.Type == PartitionType.EFI);
        public override string ToString()
            => $"{nameof(Header)}{{{nameof(MasterBootStrapLoader)}:[{string.Join(" ", MasterBootStrapLoader.Select(v => $"{v:X2}"))}], }}";
    }
}
