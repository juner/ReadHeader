using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiskHeader.GPT
{
    public readonly struct Headers : IDiskHeaders
    {
        MBR.Header IDiskHeaders.Mbr => ProtectiveMBR;
        /// <summary>
        /// LBA 0 Protective MBR
        /// </summary>
        public readonly MBR.Header ProtectiveMBR;
        /// <summary>
        /// LBA 1 Primary GPT Header
        /// </summary>
        public readonly Header PrimaryGPTHeader;
        readonly Partition[] _PartitionEntries;
        /// <summary>
        /// LBA 2 - LBA * Partition Entries
        /// </summary>
        public Partition[] PartitionEntries => (_PartitionEntries ?? Enumerable.Empty<Partition>()).ToArray();
        public Headers(MBR.Header ProtectiveMBR, Header PrimaryGPTHeader, Partition[] PartitionEntries)
            => (this.ProtectiveMBR, this.PrimaryGPTHeader, _PartitionEntries) = (ProtectiveMBR, PrimaryGPTHeader, (PartitionEntries ?? Enumerable.Empty<Partition>()).ToArray());
        public Headers Set(MBR.Header? ProtectiveMBR, Header? PrimaryGPTHeader, Partition[] PartitionEntries)
            => ProtectiveMBR == null && PrimaryGPTHeader == null && PartitionEntries == null ? this
            : new Headers(ProtectiveMBR ?? this.ProtectiveMBR, PrimaryGPTHeader ?? this.PrimaryGPTHeader, PartitionEntries ?? this.PartitionEntries);
        public override string ToString()
            => $"{nameof(Headers)}{{{nameof(ProtectiveMBR)}:{ProtectiveMBR}, {nameof(PrimaryGPTHeader)}:{PrimaryGPTHeader}, {nameof(PartitionEntries)}:[{string.Join(", ",PartitionEntries)}]}}";
    }
}
