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
        public readonly LBA Current;
        /// <summary>
        /// offset 0x20 length 8 bytes: Backup GPT Header
        /// </summary>
        public readonly LBA Backup;
        /// <summary>
        /// offset 0x28 length 8 bytes: First usable LBA for partitions
        /// </summary>
        public readonly LBA First;
        /// <summary>
        /// offset 0x30 length 8 bytes: Last usable LBA for partitions 
        /// </summary>
        public readonly LBA Last;
        /// <summary>
        /// offset 0x38 length 16 bytes: Disk Guid
        /// </summary>
        public readonly Guid DiskId;
        /// <summary>
        /// offset 0x48 length 8 bytes: Starting LBA of array of partition entries
        /// </summary>
        public readonly LBA PartitionEntries;
        /// <summary>
        /// offset 0x50 length 4 bytes: Number of partition entries in array
        /// </summary>
        public readonly ushort NumberOfPartitionCount;
        // TODO: 実装すること
    }
}
