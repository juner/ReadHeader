using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DiskHeader.GPT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Partition : IEquatable<Partition>
    {
        public static Partition Empty = new Partition();
        /// <summary>
        /// offset 0x00 length 16 bytes: Partition type GUID 
        /// </summary>
        public readonly Guid Type;
        /// <summary>
        /// offset 0x10 length 16 bytes: Unique partition GUID 
        /// </summary>
        public readonly Guid PartitionId;
        /// <summary>
        /// offset 0x20 length 8 bytes: First LBA (little endian) 
        /// </summary>
        public readonly LBA8 First;
        /// <summary>
        /// offset 0x28 length 8 bytes: Last LBA (inclusive, usually odd) 
        /// </summary>
        public readonly LBA8 Last;
        /// <summary>
        /// offset 0x30 length 8 bytes: Attribute flags (e.g. bit 60 denotes read-only) 
        /// </summary>
        public readonly PartitionAttributes AttributeFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public readonly ushort[] _Name;
        /// <summary>
        /// offset 0x38 length 72 bytes: Partition name (36 UTF-16LE code units) 
        /// </summary>
        public string Name => Encoding.Unicode.GetString((_Name ?? Enumerable.Empty<ushort>()).TakeWhile(v => v != '\0').SelectMany(v => BitConverter.GetBytes(v)).ToArray()).TrimEnd('\0');
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="PartitionId"></param>
        /// <param name="First"></param>
        /// <param name="Last"></param>
        /// <param name="AttributeFlags"></param>
        /// <param name="Name"></param>
        public Partition(Guid Type, Guid PartitionId, LBA8 First, LBA8 Last, PartitionAttributes AttributeFlags, string Name)
            => (this.Type, this.PartitionId, this.First, this.Last, this.AttributeFlags, _Name)
            = (Type, PartitionId, First, Last, AttributeFlags, Encoding.Unicode.GetBytes(Name).Split(2).Select(v => BitConverter.ToUInt16(v.ToArray(), 0)).ToArray());
        /// <summary>
        /// ptr to structure
        /// </summary>
        /// <param name="IntPtr"></param>
        /// <param name="Size"></param>
        public Partition(IntPtr IntPtr, uint Size) => this = (Partition)Marshal.PtrToStructure(IntPtr, typeof(Partition));
        public bool IsEmpty => Type == Guid.Empty && PartitionId == Guid.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{nameof(Partition)}{{{(IsEmpty ? "Empty" : $"{nameof(Type)}:{Type}, {nameof(PartitionId)}:{PartitionId}, {nameof(First)}:{First}, {nameof(Last)}:{Last}, {nameof(AttributeFlags)}:{AttributeFlags}, {nameof(Name)}:{Name}")}}}";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is Partition _obj && Equals(_obj);

        public bool Equals(Partition other)
        {
            return Type.Equals(other.Type) &&
                   PartitionId.Equals(other.PartitionId) &&
                   First.Equals(other.First) &&
                   Last.Equals(other.Last) &&
                   AttributeFlags == other.AttributeFlags &&
                   EqualityComparer<ushort[]>.Default.Equals(_Name, other._Name);
        }

        public override int GetHashCode()
        {
            var hashCode = -665036890;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(PartitionId);
            hashCode = hashCode * -1521134295 + EqualityComparer<LBA8>.Default.GetHashCode(First);
            hashCode = hashCode * -1521134295 + EqualityComparer<LBA8>.Default.GetHashCode(Last);
            hashCode = hashCode * -1521134295 + AttributeFlags.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(_Name);
            return hashCode;
        }

        public static bool operator ==(Partition partition1, Partition partition2) => partition1.Equals(partition2);

        public static bool operator !=(Partition partition1, Partition partition2) => !(partition1 == partition2);
    }
}