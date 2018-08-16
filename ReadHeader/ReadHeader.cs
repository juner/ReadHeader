using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ReadHeader
{
    public class ReadHeader
    {
        class NativeMethods
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
        }
        protected SafeFileHandle FileHandle;
        public ReadHeader(SafeFileHandle FileHandle)
            => this.FileHandle = FileHandle;

    }

    internal enum FileFlagAndAttributes : uint
    {

    }
    [Flags]
    internal enum FileFlags : uint
    {
        /// <summary>
        /// FILE_FLAG_OPEN_NO_RECALL
        /// </summary>
        OpenNoRecall = 0x00100000,
        /// <summary>
        /// FILE_FLAG_OPEN_REPARSE_POINT
        /// </summary>
        OpenReparsePoint = 0x00200000,
        /// <summary>
        /// FILE_FLAG_POSIX_SEMANTICS
        /// </summary>
        PosixSemantics = 0x01000000,
        /// <summary>
        /// FILE_FLAG_BACKUP_SEMANTICS
        /// </summary>
        BackupSemantics = 0x02000000,
        /// <summary>
        /// FILE_FLAG_DELETE_ON_CLOSE
        /// </summary>
        DeleteOnClose = 0x04000000,
        /// <summary>
        /// FILE_FLAG_SEQUENTIAL_SCAN
        /// </summary>
        SequentialScan = 0x08000000,
        /// <summary>
        /// FILE_FLAG_RANDOM_ACCESS
        /// </summary>
        RandomAccess = 0x10000000,
        /// <summary>
        /// FILE_FLAG_NO_BUFFERING
        /// </summary>
        NoBuffering = 0x20000000,
        /// <summary>
        /// FILE_FLAG_OVERLAPPED
        /// </summary>
        Overlapped = 0x40000000,
        /// <summary>
        /// FILE_FLAG_WRITE_THROUGH
        /// </summary>
        WriteThrough = 0x80000000,
    }
    internal static class FileFlagAndAttributesExtensions
    {
        public static FileFlagAndAttributes Create(FileAttributes FileAttributes) => (FileFlagAndAttributes)FileAttributes;
        public static FileFlagAndAttributes Create(FileFlags FileFlags) => (FileFlagAndAttributes)FileFlags;
        public static FileFlagAndAttributes Create(FileAttributes FileAttributes, FileFlags FileFlags) => (FileFlagAndAttributes)(((uint)FileAttributes) | ((uint)FileFlags));
        public static FileAttributes GetAttributes(this FileFlagAndAttributes FileFlagAndAttributes) => (FileAttributes)((uint)FileFlagAndAttributes & 0x000FFFFF);
        public static FileFlags GetFlags(this FileFlagAndAttributes FileFlagAndAttributes) => (FileFlags)((uint)FileFlagAndAttributes & 0xFFF00000);
        public static void Deconstruct(this FileFlagAndAttributes FileFlagAndAttributes, out FileFlags FileFlags, FileAttributes FileAttributes)
            => (FileFlags, FileAttributes) = (GetFlags(FileFlagAndAttributes), GetAttributes(FileFlagAndAttributes));
    }
}
