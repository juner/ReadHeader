using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DiskHeader.GPT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Partition
    {
        public readonly Guid Type;
        public readonly Guid PartitionId;
        public readonly LBA8 First;
        public readonly LBA8 Last;
        public readonly PartitionAttributes AttributeFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public readonly ushort[] _Name;
        public string Name => Encoding.Unicode.GetString((_Name ?? Enumerable.Empty<ushort>()).SelectMany(v => BitConverter.GetBytes(v)).ToArray());
        public Partition(Guid Type, Guid PartitionId, LBA8 First, LBA8 Last, PartitionAttributes AttributeFlags, string Name)
            => (this.Type, this.PartitionId, this.First, this.Last, this.AttributeFlags, _Name)
            = (Type, PartitionId, First, Last, AttributeFlags, Encoding.Unicode.GetBytes(Name).Split(2).Select(v => BitConverter.ToUInt16(v.ToArray(), 0)).ToArray());
        public override string ToString()
            => $"{nameof(Partition)}{{{nameof(Type)}:{Type}, {nameof(PartitionId)}:{PartitionId}, {nameof(First)}:{First}, {nameof(Last)}:{Last}, {nameof(AttributeFlags)}:{AttributeFlags}, {nameof(Name)}:{Name}}}";
    }
}