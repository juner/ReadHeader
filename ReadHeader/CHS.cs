using System;
using System.Runtime.InteropServices;

namespace DiskHeader
{
    /// <summary>
    /// Cylindeer Head Sector
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public readonly struct CHS : IEquatable<CHS>
    {
        readonly byte Bit1;
        readonly byte Bit2;
        readonly byte Bit3;
        /// <summary>
        /// Cylinder
        /// </summary>
        public ushort Cylinder => (ushort)((Bit1 & 0b1111_1111u) << 2 | ((Bit2 & 0b1100_0000u) >> 6 ));
        /// <summary>
        /// 
        /// </summary>
        public byte Head => (byte)(((Bit2 & 0b0011_1111u) << 2) | ((Bit3 & 0b1100_0000u) >> 6));
        /// <summary>
        /// 
        /// </summary>
        public byte TrackSector => (byte)(Bit3 & 0x0011_1111u);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Bit1"></param>
        /// <param name="Bit2"></param>
        /// <param name="Bit3"></param>
        private CHS(byte Bit1, byte Bit2, byte Bit3) => (this.Bit1, this.Bit2, this.Bit3) = (Bit1, Bit2, Bit3);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Cylinder"></param>
        /// <param name="Head"></param>
        /// <param name="TrackSector"></param>
        public CHS(ushort Cylinder, byte Head, byte TrackSector) : this((byte)((Cylinder & 0b0000_0011_1111_1100u) >> 2), (byte)(((Cylinder & 0b0000_0000_0000_0011u) << 6) | ((Head & 0b1111_1100u) >> 2)), (byte)(((Head & 0b0000_0011u) << 6) | (TrackSector & 0b0011_1111u))) { }
        public CHS(IntPtr IntPtr, uint Size) => this = (CHS)Marshal.PtrToStructure(IntPtr, typeof(CHS));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Cylinder"></param>
        /// <param name="Head"></param>
        /// <param name="TrackSector"></param>
        /// <returns></returns>
        public CHS Set(ushort? Cylinder = null, byte? Head = null, byte? TrackSector = null)
            => Cylinder == null && Head == null && TrackSector == null ? this
            : new CHS(Cylinder ?? this.Cylinder, Head ?? this.Head, TrackSector ?? this.TrackSector);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Cylinder"></param>
        /// <param name="Head"></param>
        /// <param name="TrackSector"></param>
        public void Deconstruct(out ushort Cylinder, out byte Head, out byte TrackSector)
            => (Cylinder, Head, TrackSector) = (this.Cylinder, this.Head, this.TrackSector);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{nameof(CHS)}{{{nameof(Cylinder)}:{Cylinder}, {nameof(Head)}:{Head}, {nameof(TrackSector)}:{TrackSector}}}";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is CHS chs ? Equals(chs) : false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CHS other)
            => Bit1 == other.Bit1
                && Bit2 == other.Bit2
                && Bit3 == other.Bit3;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -976301582;
            hashCode *= -1521134295 + Bit1.GetHashCode();
            hashCode *= -1521134295 + Bit2.GetHashCode();
            hashCode *= -1521134295 + Bit3.GetHashCode();
            return hashCode;
        }
    }
}
