using System;
using System.Runtime.InteropServices;

namespace DiskHeader
{
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public readonly struct LBA8 : IEquatable<LBA8>
    {
        private readonly ulong Bytes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Bytes"></param>
        private LBA8(ulong Bytes) => this.Bytes = Bytes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public static explicit operator ulong(LBA8 self) => self.Bytes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public static explicit operator LBA8(ulong self) => new LBA8(self);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lBA1"></param>
        /// <param name="lBA2"></param>
        /// <returns></returns>
        public static bool operator ==(LBA8 lBA1, LBA8 lBA2) => lBA1.Equals(lBA2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lBA1"></param>
        /// <param name="lBA2"></param>
        /// <returns></returns>
        public static bool operator !=(LBA8 lBA1, LBA8 lBA2) => !(lBA1 == lBA2);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{nameof(LBA8)}(0x{Bytes:X16})";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is LBA8 _obj && Equals(_obj);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LBA8 other) => Bytes == other.Bytes;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => 1182642244 + Bytes.GetHashCode();
    }
}
