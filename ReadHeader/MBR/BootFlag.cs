namespace DiskHeader.MBR
{
    public enum BootFlag : byte
    {
        UnBootable = 0x0,
        Bootable = 0x80,
    }
}
