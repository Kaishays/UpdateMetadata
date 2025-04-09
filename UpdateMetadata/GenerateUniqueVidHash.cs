using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMetadata
{
    /*Summary
    * GetFileInformationByHandle is built into fileapi.h.
    * Each query takes 1 ms to 15 ms on Y:\ drive so 16.5 sec to 4.13 minutes to get unique ID for 16500 files. 
    * https://learn.microsoft.com/en-us/windows/win32/api/fileapi/ns-fileapi-by_handle_file_information 
    * If path tableName / file tableName is changed the ID will not change.
    * If the file is moved the ID will change.
    * If the file is copy and pasted to a new 
    * 
    * location the ID will change.
    Summary */
    public class GenerateUniqueVidHash
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        [StructLayout(LayoutKind.Sequential)]
        struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential)]

        struct BY_HANDLE_FILE_INFORMATION
        {
            public uint FileAttributes;
            public FILETIME CreationTime;
            public FILETIME LastAccessTime;
            public FILETIME LastWriteTime;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }
        public static async Task<ulong> GetUniqueFileHashCode(string filePath)
        {
            using (SafeFileHandle fileHandle = File.OpenHandle(filePath))
            {
                if (GetFileInformationByHandle(fileHandle, out BY_HANDLE_FILE_INFORMATION fileInfo))
                {
                    ulong uniqueID = ((ulong)fileInfo.FileIndexHigh << 32) | fileInfo.FileIndexLow;
                    return await Task.FromResult(uniqueID);
                }
                throw new IOException("Unable to get file information.");
            }
        }
    }
}
