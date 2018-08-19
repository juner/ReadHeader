using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiskHeader.MBR
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
    public readonly struct Partition : IEquatable<Partition>
    {
        public readonly static Partition Empty = new Partition();
        [MarshalAs(UnmanagedType.U1)]
        public readonly BootFlag Active;
        public readonly CHS Begin;
        public readonly PartitionType Type;
        public readonly CHS End;
        public readonly LBA4 Start;
        public readonly LBA4 Length;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Active"></param>
        /// <param name="Begin"></param>
        /// <param name="Type"></param>
        /// <param name="End"></param>
        /// <param name="Start"></param>
        /// <param name="Length"></param>
        public Partition(BootFlag Active, CHS Begin, PartitionType Type, CHS End, LBA4 Start, LBA4 Length)
            => (this.Active, this.Begin, this.Type, this.End, this.Start, this.Length)
            = (Active, Begin, Type, End, Start, Length);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IntPtr"></param>
        /// <param name="ReadBytes"></param>
        public Partition(IntPtr IntPtr, uint ReadBytes) => this = (Partition)Marshal.PtrToStructure(IntPtr, typeof(Partition));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Active"></param>
        /// <param name="Begin"></param>
        /// <param name="Type"></param>
        /// <param name="End"></param>
        /// <param name="Start"></param>
        /// <param name="Length"></param>
        public void Deconstruct(out BootFlag Active, out CHS Begin, out PartitionType Type, out CHS End, out LBA4 Start, out LBA4 Length)
            => (Active, Begin, Type, End, Start, Length)
            = (this.Active, this.Begin, this.Type, this.End, this.Start, this.Length);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is Partition partition ? Equals(partition) : false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Partition other)
            => Active == other.Active
                && Begin == other.Begin
                && Type == other.Type
                && End == other.End
                && Start == other.Start
                && Length == other.Length;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 1637773344;
            hashCode = hashCode * -1521134295 + Active.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<CHS>.Default.GetHashCode(Begin);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<CHS>.Default.GetHashCode(End);
            hashCode = hashCode * -1521134295 + EqualityComparer<LBA4>.Default.GetHashCode(Start);
            hashCode = hashCode * -1521134295 + EqualityComparer<LBA4>.Default.GetHashCode(Length);
            return hashCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partition1"></param>
        /// <param name="partition2"></param>
        /// <returns></returns>
        public static bool operator ==(Partition partition1, Partition partition2) => partition1.Equals(partition2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partition1"></param>
        /// <param name="partition2"></param>
        /// <returns></returns>
        public static bool operator !=(Partition partition1, Partition partition2) => !(partition1 == partition2);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{nameof(Partition)}{{{(this == Empty ? "Empty" : $"{nameof(Active)}:{Active}, {nameof(Begin)}:{Begin}, {nameof(Type)}:{Type}, {nameof(End)}:{End}, {nameof(Start)}:{Start}, {nameof(Length)}:{Length}" )}}}";

    }
}
