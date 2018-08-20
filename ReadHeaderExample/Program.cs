using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ReadHeaderExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            foreach(var index in Enumerable.Range(0,10))
#if false
                using (var Source = new System.Threading.CancellationTokenSource())
                using (var reader = DiskHeader.DiskHeader.FromPhysicalDriveNumber(index, true))
                    try
                    {
                        if (reader.IsInvalid)
                            continue;
                        if (await reader.ReadAsync(Source.Token) is DiskHeader.IDiskHeaders Header)
                            Trace.WriteLine(reader);
                        else
                            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                        Source.Cancel();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(reader);
                        Trace.WriteLine(e);
                        continue;
                    }
#else
                using (var reader = DiskHeader.DiskHeader.FromPhysicalDriveNumber(index, false))
                    try
                    {
                        if (reader.IsInvalid)
                            continue;
                        if (reader.Read(out DiskHeader.IDiskHeaders Header))
                            Trace.WriteLine(reader);
                        else
                            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(reader);
                        Trace.WriteLine(e);
                        continue;
                    }
#endif
        }
    }
}
