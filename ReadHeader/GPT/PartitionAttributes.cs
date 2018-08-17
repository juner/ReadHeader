using System;

namespace DiskHeader.GPT
{
    [Flags]
    public enum PartitionAttributes : ulong
    {
        /// <summary>
        /// bit 0: Platform required (required by the computer to function properly, OEM partition for example, disk partitioning utilities must preserve the partition as is) 
        /// </summary>
        PlatformRequired = 0x0000_0000_0000_00001,
        /// <summary>
        /// bit 1: EFI firmware should ignore the content of the partition and not try to read from it 
        /// </summary>
        IgnoreContent = 0x0000_0000_0000_0002,
        /// <summary>
        /// bit 2: Legacy BIOS bootable (equivalent to active flag (typically bit 7 set) at offset +0h in partition entries of the MBR partition table)
        /// </summary>
        LegacyBIOSBootable = 0x0000_0000_0000_0004,

        // bit 3-47: Reserved for future use 

        // bit 48-63: Defined and used byte the individual partition type

        /// <summary>
        /// bit 60: Read-only
        /// </summary>
        ReadOnly = 0x0800_0000_0000_0000,
        ShadowCopy = 0x1000_0000_0000_0000,
        Hidden = 0x2000_0000_0000_0000,
        NoDriveLetter = 0x4000_0000_0000,

    }
}