using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ReadHeaderExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
#if false
            using (var Source = new System.Threading.CancellationTokenSource())
            using (var reader = DiskHeader.DiskHeader.FromPhysicalDriveNumber(0, true))
            {
                if (reader.IsInvalid)
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                if (await reader.ReadAsync(Source.Token) is DiskHeader.IDiskHeaders Header)
                    Trace.WriteLine(Header);
                else
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                Source.Cancel();
            }
#else
            using (var reader = DiskHeader.DiskHeader.FromPhysicalDriveNumber(0, false))
            {
                if (reader.IsInvalid)
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                if (reader.Read(out DiskHeader.IDiskHeaders Header))
                    Trace.WriteLine(Header);
                else
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
#endif
        }
    }
}
