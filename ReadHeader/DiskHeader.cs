using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Linq;
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
                uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped = default);
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
                 SafeFileHandle File, long DistanceToMove,
                 IntPtr NewFilePointer = default, FileMove MoveMethod = default);
            [DllImport("kernel32.dll", SetLastError = true,
                CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeviceIoControl(
                SafeFileHandle hDevice,
                IOControlCode IoControlCode,
                IntPtr InBuffer,
                uint nInBufferSize,
                IntPtr OutBuffer,
                uint nOutBufferSize,
                out uint BytesReturned,
                IntPtr Overlapped = default
            );
        }
        protected SafeFileHandle FileHandle;
        public readonly string FilePath = null;
        public readonly bool? AsyncFlag;
        public bool IsInvalid => FileHandle.IsInvalid;
        public bool IsClosed => FileHandle.IsClosed;
        public IntPtr DangerousGetHandle() => FileHandle.DangerousGetHandle();
        public uint? SectorSize { get; protected set; }
        public IDiskHeaders Headers { get; protected set; }
        public override string ToString()
            => $"{nameof(DiskHeader)}{{{(string.IsNullOrWhiteSpace(FilePath) ? "" : $"{nameof(FilePath)}:{FilePath}, ")}{(AsyncFlag == null ? "" : $"{nameof(AsyncFlag)}:{AsyncFlag}, ")}{nameof(IsInvalid)}:{IsInvalid}, {nameof(IsClosed)}:{IsClosed}{(SectorSize == null ? "" : $", {nameof(SectorSize)}:{SectorSize}")}{(Headers == null ? "" : $", {nameof(Headers)}:{Headers}")}}}";
        public DiskHeader(SafeFileHandle FileHandle, bool? AsyncFlag = null)
            => (this.FileHandle, this.AsyncFlag) = (FileHandle, AsyncFlag);
        public DiskHeader(string FilePath, bool AsyncFlag = false)
            : this(NativeMethods.CreateFileW(FilePath,
                FileAccess.Read,
                FileShare.ReadWrite,
                IntPtr.Zero,
                FileMode.Open,
                FileFlagAndAttributesExtensions.Create(FileAttributes.Normal, AsyncFlag ? FileFlags.Overlapped : default),
                IntPtr.Zero), AsyncFlag)
            => this.FilePath = FilePath;
        public static DiskHeader FromPhysicalDriveNumber(int PhysicalDriveNumber, bool AsyncFlag = false)
            => PhysicalDriveNumber >= 0 ?
                new DiskHeader($@"\\.\PhysicalDrive{PhysicalDriveNumber}", AsyncFlag)
                : throw new ArgumentException($"{nameof(PhysicalDriveNumber)} は 0 以上の必要があります。", nameof(PhysicalDriveNumber));
        protected static IDisposable CreatePtr<T>(out IntPtr IntPtr, out uint Size)
        {
            Size = (uint)Marshal.SizeOf<T>();
            return CreatePtr(Size, out IntPtr);
        }
        protected static IDisposable CreatePtr(uint Size, out IntPtr IntPtr)
        {
            var _Ptr = Marshal.AllocCoTaskMem((int)Size);
            var Dispose = Disposable.Create(() => Marshal.FreeCoTaskMem(_Ptr));
            IntPtr = _Ptr;
            return Dispose;
        }
        protected static uint AdjustBySectorSize(uint SectorSize, ref uint Size) => Size = Size + (Size % SectorSize != 0 ? SectorSize - (Size % SectorSize) : 0u);
        protected uint AdjustBySectorSize(ref uint Size) => AdjustBySectorSize(SectorSize.Value, ref Size);
        protected static IDisposable CreatePtr(uint SectorSize, ref uint Size, out IntPtr IntPtr) => CreatePtr(AdjustBySectorSize(SectorSize, ref Size), out IntPtr);
        protected static IDisposable CreatePtr<T>(uint SectorSize, out IntPtr IntPtr, out uint Size)
        {
            Size = (uint)Marshal.SizeOf<T>();
            return CreatePtr(SectorSize, ref Size, out IntPtr);
        }
        internal static IDisposable CreateOverlappedAndEvent(out Overlapped Overlapped, out ManualResetEvent Event)
        {
            var _Overlapped = new Overlapped();
            var _Event = new ManualResetEvent(false);
            _Overlapped.ClearAndSetEvent(_Event.GetSafeWaitHandle().DangerousGetHandle());
            var Dispose = Disposable.Create(() =>
            {
                _Event.Dispose();
                _Overlapped.Dispose();
            });
            Overlapped = _Overlapped;
            Event = _Event;
            return Dispose;
        }
        internal bool DeviceIoControl(IOControlCode IoControlCode, IntPtr InPtr, uint InSize, IntPtr OutPtr, uint OutSize, out uint ReturnBytes) => NativeMethods.DeviceIoControl(FileHandle, IoControlCode, InPtr, InSize, OutPtr, OutSize, out ReturnBytes, IntPtr.Zero);
        internal bool GetDiskGeometry(out DiskGeometry DiskGeometry, out uint ReturnBytes)
        {
            bool Result;
            using (CreatePtr<DiskGeometry>(out var Ptr, out var Size))
                DiskGeometry = (Result = DeviceIoControl(IOControlCode.DiskGetDriveGeometry, IntPtr.Zero, 0, Ptr, Size, out ReturnBytes)) && ReturnBytes > 0
                    ? new DiskGeometry(Ptr, ReturnBytes) : default;
            return Result;
        }
        internal async Task<(bool Result, uint ReturnBytes)> DeviceIoControlAsync(IOControlCode IoControlCode, IntPtr InPtr, uint InSize, IntPtr OutPtr, uint OutSize, CancellationToken Token = default)
        {
            bool Result;
            using (CreateOverlappedAndEvent(out var overlapped, out var Event))
            {
                Result = NativeMethods.DeviceIoControl(FileHandle, IoControlCode, InPtr, InSize, OutPtr, OutSize, out _, overlapped.GlobalOverlapped);
                using (Token.Register(() => NativeMethods.CancelIoEx(FileHandle, overlapped.GlobalOverlapped)))
                    Result = await Event.WaitOneAsync(Token);
                return (NativeMethods.GetOverlappedResult(FileHandle, overlapped, out var ReturnBytes, false), ReturnBytes);
                
            }
        }
        
        internal async Task<DiskGeometry?> GetDiskGeometryAsync(CancellationToken Token = default)
        {
            Token.ThrowIfCancellationRequested();
            bool Result;
            using (CreatePtr<DiskGeometry>(out var Ptr, out var Size))
            using (CreateOverlappedAndEvent(out var overlapped,out var Event))
            {
                Result = NativeMethods.DeviceIoControl(FileHandle, IOControlCode.DiskGetDriveGeometry, IntPtr.Zero, 0, Ptr, (uint)Size, out _, overlapped.GlobalOverlapped);
                using (Token.Register(() => NativeMethods.CancelIoEx(FileHandle, overlapped.GlobalOverlapped)))
                    Result = await Event.WaitOneAsync(Token);
                Result = NativeMethods.GetOverlappedResult(FileHandle, overlapped, out var ReturnBytes, false);
                return Result ? new DiskGeometry(Ptr, ReturnBytes) : default;
            }
        }
        protected bool ReadFile(IntPtr IntPtr, uint Size, out uint ReadBytes) => NativeMethods.ReadFile(FileHandle, IntPtr, Size, out ReadBytes);
        protected async Task<(bool Result, uint ReadBytes)> ReadFileAsync(IntPtr IntPtr, uint Size, CancellationToken Token = default)
        {
            const int TASK_SEQUENCE_ERROR = unchecked((int)0x800703E5);
            Token.ThrowIfCancellationRequested();
            using (CreateOverlappedAndEvent(out var overlapped, out var Event))
            {
                bool Result;
                Result = NativeMethods.ReadFile(FileHandle, IntPtr, Size, out _, overlapped.GlobalOverlapped);
                if (!Result && Marshal.GetHRForLastWin32Error() != TASK_SEQUENCE_ERROR)
                    return (Result, 0);
                using (Token.Register(() => NativeMethods.CancelIoEx(FileHandle, overlapped.GlobalOverlapped)))
                    await Event.WaitOneAsync(Token);
                Result = NativeMethods.GetOverlappedResult(FileHandle, overlapped, out var ReadBytes, false);
                return (Result, ReadBytes);
            }
        }
        internal bool SetFilePointer(long DitanceToMove, FileMove MoveMethod) => NativeMethods.SetFilePointerEx(FileHandle, DitanceToMove, IntPtr.Zero, MoveMethod);
        protected bool ReadMBR(out MBR.Header Header)
        {
            using (CreatePtr<MBR.Header>(SectorSize.Value, out var IntPtr, out var Size))
            {
                var result = ReadFile(IntPtr, Size, out var ReadBytes);
                Header = new MBR.Header(IntPtr, ReadBytes);
                return result;
            }
        }
        protected async Task<MBR.Header?> ReadMBRAsync(CancellationToken Token = default)
        {
            Token.ThrowIfCancellationRequested();
            using (CreatePtr(SectorSize.Value, out var IntPtr))
            {
                var (Result,ReadBytes) = await ReadFileAsync(IntPtr, SectorSize.Value, Token);
                return Result ? new MBR.Header(IntPtr, ReadBytes) : (MBR.Header?)null;
            }
        }
        protected bool ReadGptHeader(out GPT.Header Header)
        {
            using (CreatePtr(SectorSize.Value, out var IntPtr))
            {
                var result = ReadFile(IntPtr, SectorSize.Value, out var ReadBytes);
                Header = new GPT.Header(IntPtr, ReadBytes);
                return result;
            }
        }
        protected async Task<GPT.Header?> ReadGptHeaderAsync(CancellationToken Token = default)
        {
            Token.ThrowIfCancellationRequested();
            using (CreatePtr(SectorSize.Value, out var IntPtr))
            {
                var (Result, ReadBytes) = await ReadFileAsync(IntPtr, SectorSize.Value, Token);
                return !Result ? (GPT.Header?)null : new GPT.Header(IntPtr, SectorSize.Value < ReadBytes ? SectorSize.Value : ReadBytes);
            }
        }
        protected bool ReadGptPartitionsBySector(out GPT.Partition[] Partitions)
        {
            using (CreatePtr(SectorSize.Value, out var IntPtr))
            {
                var result = ReadFile(IntPtr, SectorSize.Value, out var ReadBytes);
                var PartitionSize = Marshal.SizeOf<GPT.Partition>();
                Partitions = Enumerable.Range(0, (int)(SectorSize / PartitionSize))
                    .Select(index => new GPT.Partition(IntPtr.Add(IntPtr, PartitionSize * index), ReadBytes - (uint)(PartitionSize * index)))
                    .ToArray();
                return result;
            }
        }
        protected async Task<GPT.Partition[]> ReadGptPartitionsBySectorAsync(CancellationToken Token = default)
        {
            using (CreatePtr(SectorSize.Value, out var IntPtr))
            {
                var (Result, ReadBytes) = await ReadFileAsync(IntPtr, SectorSize.Value, Token);
                var PartitionSize = Marshal.SizeOf<GPT.Partition>();
                return !Result ? null : Enumerable.Range(0, (int)(SectorSize / PartitionSize))
                    .Select(index => new GPT.Partition(IntPtr.Add(IntPtr, PartitionSize * index), (SectorSize.Value < ReadBytes ? SectorSize.Value : ReadBytes) - (uint)(PartitionSize * index)))
                    .ToArray();
            }
        }

        public bool Read(out IDiskHeaders Headers)
        {
            do
            {
                if (!GetDiskGeometry(out var Geometry, out _))
                    break;
                SectorSize = Geometry.BytesPerSector;
                // Set LBA 0
                if (!SetFilePointer(0, FileMove.Begin))
                    break;
                if (!ReadMBR(out var MBRHeader))
                    break;
                if (!MBRHeader.IsGPT)
                {
                    this.Headers = Headers = (MBR.Headers)MBRHeader;
                    return true;
                }
                // Set LBA 1
                if (!SetFilePointer(SectorSize.Value, FileMove.Begin))
                    break;
                if (!ReadGptHeader(out var GPTHeader))
                    break;
                var GptPartitions = new GPT.Partition[GPTHeader.NumberOfPartitionCount];
                var PartitionSize = Marshal.SizeOf<GPT.Partition>();
                var SectorsCount = (int)(SectorSize / PartitionSize);
                foreach(var sectorLine in Enumerable.Range(0, (int)(GPTHeader.NumberOfPartitionCount / SectorsCount)))
                {
                    if (!SetFilePointer(SectorSize.Value * ((uint)GPTHeader.PartitionEntries + sectorLine), FileMove.Begin))
                        break;
                    if (!ReadGptPartitionsBySector(out var Partition))
                        break;
                    foreach(var index in Enumerable.Range(sectorLine * SectorsCount, SectorsCount))
                        GptPartitions[index] = Partition[index % SectorsCount];
                }
                this.Headers = Headers = new GPT.Headers(MBRHeader, GPTHeader, GptPartitions);
                return true;
            } while (false);
            this.Headers = Headers = default;
            return false;
        }
        public async Task<IDiskHeaders> ReadAsync(CancellationToken Token = default)
        {
            do
            {
                if (!(await GetDiskGeometryAsync(Token) is DiskGeometry DiskGeometry))
                    break;
                SectorSize = DiskGeometry.BytesPerSector;
                if (!SetFilePointer(0, FileMove.Begin))
                    break;
                if (!(await ReadMBRAsync(Token) is MBR.Header MBRHeader))
                    break;
                if (!MBRHeader.IsGPT)
                    return Headers = (MBR.Headers)MBRHeader;
                // Set LBA 1
                if (!SetFilePointer(SectorSize.Value, FileMove.Begin))
                    break;
                if (!(await ReadGptHeaderAsync(Token) is GPT.Header GPTHeader))
                    break;
                return Headers = new GPT.Headers(MBRHeader, GPTHeader, default);
            } while (false);
            return this.Headers = default;
                
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
