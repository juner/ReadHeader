using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DiskHeader
{
    public class DiskHeader : IDisposable
    {
        private class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true,
                CallingConvention = CallingConvention.StdCall,
                CharSet = CharSet.Unicode)]
            public static extern SafeFileHandle CreateFileW(
                 [MarshalAs(UnmanagedType.LPWStr)] string filename,
                 [MarshalAs(UnmanagedType.U4)] FileAccess access,
                 [MarshalAs(UnmanagedType.U4)] FileShare share,
                 IntPtr securityAttributes,
                 [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                 [MarshalAs(UnmanagedType.U4)] FileFlagAndAttributes flagsAndAttributes,
                 IntPtr templateFile);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer,
                uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool GetOverlappedResult(
                  SafeFileHandle hFile,                       // ファイル、パイプ、通信デバイスのハンドル
                  Overlapped lpOverlapped,          // オーバーラップ構造体
                  out ushort lpNumberOfBytesTransferred, // 転送されたバイト数
                  bool bWait                          // 待機オプション
                );
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CancelIoEx(SafeFileHandle hFile, IntPtr lpOverlapped = default);
            [DllImport("kernel32.dll")]
            public static extern bool SetFilePointerEx(
                 SafeFileHandle hFile, long liDistanceToMove,
                 IntPtr lpNewFilePointer, FileMove dwMoveMethod);
        }
        protected SafeFileHandle FileHandle;
        public bool IsInvalid => FileHandle.IsInvalid;
        public bool IsClosed => FileHandle.IsClosed;
        public IntPtr DangerousGetHandle() => FileHandle.DangerousGetHandle();
        public DiskHeader(SafeFileHandle FileHandle)
            => this.FileHandle = FileHandle;
        public DiskHeader(string FilePath)
            : this(NativeMethods.CreateFileW(FilePath, 
                FileAccess.Read, 
                FileShare.ReadWrite, 
                IntPtr.Zero, 
                FileMode.Open, 
                FileFlagAndAttributesExtensions.Create(FileAttributes.Normal, FileFlags.Overlapped), 
                IntPtr.Zero)){ }
        public static DiskHeader FromPhysicalDriveNumber(int PhysicalDriveNumber)
            => PhysicalDriveNumber >= 0 ?
                new DiskHeader($@"\\.\PhysicalDrive{PhysicalDriveNumber}")
                : throw new ArgumentException($"{nameof(PhysicalDriveNumber)} は 0 以上の必要があります。",nameof(PhysicalDriveNumber));
        public async Task<(bool Result, MBR.Header Header)> ReadAsync(CancellationToken Token = default)
        {
            var Size = Marshal.SizeOf<MBR.Header>();
            var IntPtr = Marshal.AllocCoTaskMem(Size);
            using (Disposable.Create(() => Marshal.FreeCoTaskMem(IntPtr)))
            using (var overlapped = new Overlapped())
            using (var Event = new ManualResetEvent(false))
            {
                bool result;
                overlapped.ClearAndSetEvent(Event.SafeWaitHandle.DangerousGetHandle());
                result = NativeMethods.SetFilePointerEx(FileHandle, 0, IntPtr.Zero, FileMove.Begin);
                if (!result)
                    return (result, default);
                result = NativeMethods.ReadFile(FileHandle, IntPtr, (uint)Size, out var ReadBytes, overlapped.GlobalOverlapped);
                if (!result && ReadBytes == 0)
                    return (result, default);
                using (Token.Register(() => NativeMethods.CancelIoEx(FileHandle, overlapped.GlobalOverlapped)))
                    await Event.WaitOneAsync(Token);
                result = NativeMethods.GetOverlappedResult(FileHandle, overlapped, out var _ReadBytes, false);
                return (result, (MBR.Header)Marshal.PtrToStructure(IntPtr, typeof(MBR.Header)));
            }
        }
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FileHandle.Dispose();
                }
                disposedValue = true;
            }
        }
        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
