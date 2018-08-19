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
        public ushort TrackPerCylinder => (ushort)((Bit1 & 0b1111_1111u) << 2 | ((Bit2 & 0b1100_0000u) >> 6 ));
        /// <summary>
        /// 
        /// </summary>
        public byte Head => (byte)(((Bit2 & 0b0011_1111u) << 2) | ((Bit3 & 0b1100_0000u) >> 6));
        /// <summary>
        /// 
        /// </summary>
        public byte Sector => (byte)(Bit3 & 0x0011_1111u);
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
        /// <param name="TrackPerCylinder"></param>
        /// <param name="Head"></param>
        /// <param name="Sector"></param>
        public CHS(ushort TrackPerCylinder, byte Head, byte Sector) : this((byte)((TrackPerCylinder & 0b0000_0011_1111_1100u) >> 2), (byte)(((TrackPerCylinder & 0b0000_0000_0000_0011u) << 6) | ((Head & 0b1111_1100u) >> 2)), (byte)(((Head & 0b0000_0011u) << 6) | (Sector & 0b0011_1111u))) { }
        public CHS(IntPtr IntPtr, uint Size) => this = (CHS)Marshal.PtrToStructure(IntPtr, typeof(CHS));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TrackPerCylinder"></param>
        /// <param name="Head"></param>
        /// <param name="Sector"></param>
        /// <returns></returns>
        public CHS Set(ushort? TrackPerCylinder = null, byte? Head = null, byte? Sector = null)
            => TrackPerCylinder == null && Head == null && Sector == null ? this
            : new CHS(TrackPerCylinder ?? this.TrackPerCylinder, Head ?? this.Head, Sector ?? this.Sector);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TrackPerCylinder"></param>
        /// <param name="Head"></param>
        /// <param name="Sector"></param>
        public void Deconstruct(out ushort TrackPerCylinder, out byte Head, out byte Sector)
            => (TrackPerCylinder, Head, Sector) = (this.TrackPerCylinder, this.Head, this.Sector);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{nameof(CHS)}{{{nameof(TrackPerCylinder)}:{TrackPerCylinder}, {nameof(Head)}:{Head}, {nameof(Sector)}:{Sector}}}";
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
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(CHS a, CHS b) => a.Equals(b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(CHS a, CHS b) => !a.Equals(b);
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
