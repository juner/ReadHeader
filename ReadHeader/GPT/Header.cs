using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DiskHeader.GPT
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Header
    {
        /// <summary>
        /// offset 0x00 length 8 bytes Signature
        /// </summary>
        public readonly ulong Signature;
        /// <summary>
        /// offset 0x08 length 4 bytes: GPT Revision
        /// </summary>
        public readonly uint Revision;
        /// <summary>
        /// offset 0x0C length 4 bytes: Header Size
        /// </summary>
        public readonly uint Size;
        /// <summary>
        /// offset 0x10 length 4 bytes: CRC32/zlib of header(offset +0 up to header size)
        /// </summary>
        public readonly uint CRC32;
        /// <summary>
        /// offset 0x14 length 4 bytes: reserved must be zero
        /// </summary>
        public readonly uint Reserved;
        /// <summary>
        /// offset 0x18 length 8 bytes: Current GPT Header
        /// </summary>
        public readonly LBA8 Current;
        /// <summary>
        /// offset 0x20 length 8 bytes: Backup GPT Header
        /// </summary>
        public readonly LBA8 Backup;
        /// <summary>
        /// offset 0x28 length 8 bytes: First usable LBA for partitions
        /// </summary>
        public readonly LBA8 First;
        /// <summary>
        /// offset 0x30 length 8 bytes: Last usable LBA for partitions 
        /// </summary>
        public readonly LBA8 Last;
        /// <summary>
        /// offset 0x38 length 16 bytes: Disk Guid
        /// </summary>
        public readonly Guid DiskId;
        /// <summary>
        /// offset 0x48 length 8 bytes: Starting LBA of array of partition entries
        /// </summary>
        public readonly LBA8 PartitionEntries;
        /// <summary>
        /// offset 0x50 length 4 bytes: Number of partition entries in array
        /// </summary>
        public readonly uint NumberOfPartitionCount;
        /// <summary>
        /// offset 0x50 length 4 bytes: Size of single partition entry (usually 0x88 or 128)
        /// </summary>
        public readonly uint SizeOfSinglePartitionEntry;
        /// <summary>
        /// offset 0x58 length 4 bytes: CRC32/zlib of partition array in little endian
        /// </summary>
        public readonly uint PartitionCRC32;
        /// <summary>
        /// offset 0x5c length * bytes: Reserved; mus be zeroes for the rest of the block (420 bytes or a sector size of 512 bytes; but can be more with larger sector sizes)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public readonly byte[] Reserved2;
        public Header(IntPtr IntPtr, uint Size)
        {
            this = (Header)Marshal.PtrToStructure(IntPtr, typeof(Header));
            if (Signature == 0)
                return;
            Reserved2 = new byte[Size - this.Size];
            Marshal.Copy(IntPtr.Add(IntPtr, (int)Marshal.OffsetOf<Header>(nameof(Reserved2))), Reserved2, 0, Reserved2.Length);
        }
        public override string ToString()
            => $"{nameof(Header)}{{{nameof(Signature)}:{(Signature == 0 ? "" : string.Join(string.Empty, BitConverter.GetBytes(Signature).Select(Convert.ToChar)))}, {nameof(Revision)}:{Revision}, {nameof(Size)}:{Size}, {nameof(CRC32)}:0x{CRC32:X4}, {nameof(Reserved)}:0x{Reserved:X4}, {nameof(Current)}:{Current}, {nameof(Backup)}:{Backup}, {nameof(First)}:{First}, {nameof(Last)}:{Last}, {nameof(DiskId)}:{DiskId}, {nameof(PartitionEntries)}:{PartitionEntries}, {nameof(NumberOfPartitionCount)}:{NumberOfPartitionCount}, {nameof(SizeOfSinglePartitionEntry)}:{SizeOfSinglePartitionEntry}, {nameof(PartitionCRC32)}:{PartitionCRC32}, {nameof(Reserved2)}:[{string.Join(" ", Reserved2.Select(v => $"{v:X2}"))}]}}";
    }
}
