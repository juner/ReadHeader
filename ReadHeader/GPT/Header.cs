using System;
using System.Collections.Generic;
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
        public readonly uint Signature;
        /// <summary>
        /// offset 0x08 length 4 bytes: GPT Revision
        /// </summary>
        public readonly ushort Revision;
        /// <summary>
        /// offset 0x0C length 4 bytes: Header Size
        /// </summary>
        public readonly ushort Size;
        /// <summary>
        /// offset 0x10 length 4 bytes: CRC32/zlib of header(offset +0 up to header size)
        /// </summary>
        public readonly ushort CRC32;
        /// <summary>
        /// offset 0x14 length 4 bytes: reserved must be zero
        /// </summary>
        public readonly ushort Reserved;
        /// <summary>
        /// offset 0x18 length 8 bytes: Current GPT Header
        /// </summary>
        public readonly LBA4 Current;
        /// <summary>
        /// offset 0x20 length 8 bytes: Backup GPT Header
        /// </summary>
        public readonly LBA4 Backup;
        /// <summary>
        /// offset 0x28 length 8 bytes: First usable LBA for partitions
        /// </summary>
        public readonly LBA4 First;
        /// <summary>
        /// offset 0x30 length 8 bytes: Last usable LBA for partitions 
        /// </summary>
        public readonly LBA4 Last;
        /// <summary>
        /// offset 0x38 length 16 bytes: Disk Guid
        /// </summary>
        public readonly Guid DiskId;
        /// <summary>
        /// offset 0x48 length 8 bytes: Starting LBA of array of partition entries
        /// </summary>
        public readonly LBA4 PartitionEntries;
        /// <summary>
        /// offset 0x50 length 4 bytes: Number of partition entries in array
        /// </summary>
        public readonly ushort NumberOfPartitionCount;
        /// <summary>
        /// offset 0x50 length 4 bytes: Size of single partition entry (usually 0x88 or 128)
        /// </summary>
        public readonly ushort SizeOfSinglePartitionEntry;
        /// <summary>
        /// offset 0x58 length 4 bytes: CRC32/zlib of partition array in little endian
        /// </summary>
        public readonly ushort PartitionCRC32;
        /// <summary>
        /// offset 0x5c length * bytes: Reserved; mus be zeroes for the rest of the block (420 bytes ofr a sector size of 512 bytes; but can be more with larger sector sizes)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public readonly byte[] Reserved2;
    }
}
