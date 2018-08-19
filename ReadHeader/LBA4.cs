using System;
using System.Runtime.InteropServices;

namespace DiskHeader
{
    /// <summary>
    /// Logical Block Address
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly struct LBA4 : IEquatable<LBA4>
    {
        private readonly uint Bytes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Bytes"></param>
        private LBA4(uint Bytes) => this.Bytes = Bytes;
        public override string ToString()
            => $"{nameof(LBA4)}(0x{Bytes:X8})";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public static explicit operator uint(LBA4 self) => self.Bytes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public static explicit operator LBA4(uint self) => new LBA4(self);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is LBA4 && Equals((LBA4)obj);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LBA4 other) => Bytes == other.Bytes;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => 1182642244 + Bytes.GetHashCode();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lBA1"></param>
        /// <param name="lBA2"></param>
        /// <returns></returns>
        public static bool operator ==(LBA4 lBA1, LBA4 lBA2) => lBA1.Equals(lBA2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lBA1"></param>
        /// <param name="lBA2"></param>
        /// <returns></returns>
        public static bool operator !=(LBA4 lBA1, LBA4 lBA2) => !(lBA1 == lBA2);

    }
}
