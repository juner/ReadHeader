using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DiskHeader;

namespace ReadHeaderExample
{
    class Program
    {
        static async Task Main(string[] args)
        {

            using (var source = new CancellationTokenSource())
            using (var reader = DiskHeader.DiskHeader.FromPhysicalDriveNumber(0))
            {
                if (reader.IsInvalid)
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                var result = await reader.ReadAsync(source.Token);
                if (result.Result)
                {
                    Console.WriteLine(result.Header);
                }
                else
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
        }
    }
}
