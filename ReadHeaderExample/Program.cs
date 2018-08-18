using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ReadHeaderExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            using (var reader = DiskHeader.DiskHeader.FromPhysicalDriveNumber(0))
            {
                if (reader.IsInvalid)
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                if(reader.Read(out var Header))
                {
                    Trace.WriteLine(Header);
                }
                else
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
        }
    }
}
