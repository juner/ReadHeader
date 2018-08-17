using System;
using System.Runtime.InteropServices;

namespace ReadHeaderExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = DiskHeader.DiskHeader.FromPhysicalDriveNumber(0))
            {
                if (reader.IsInvalid)
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                if(reader.Read(out var Header))
                {
                    Console.WriteLine(Header);
                }
                else
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
        }
    }
}
