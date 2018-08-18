namespace DiskHeader
{
    internal enum IOControlCode : uint{

        DiskGetDriveGeometry = (FileDevice.Disk << 16) | (0x0000 << 2) | Method.Buffered | (0 << 14),
    }
}
