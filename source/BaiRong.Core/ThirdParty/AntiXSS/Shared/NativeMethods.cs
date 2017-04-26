// ***************************************************************
// <copyright file="NativeMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      ...
// </summary>
// ***************************************************************

namespace Microsoft.Exchange.Data.Internal
{
    using System;
    using Win32.SafeHandles;
    using System.Runtime.InteropServices;

    
    
    
    [ComVisible(false), System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal class NativeMethods    
    {
        
        
        

        internal const uint DELETE = 0x00010000;
        internal const uint READ_CONTROL = 0x00020000;
        internal const uint WRITE_DAC = 0x00040000;
        internal const uint WRITE_OWNER = 0x00080000;
        internal const uint SYNCHRONIZE = 0x00100000;

        internal const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;

        internal const uint STANDARD_RIGHTS_READ = READ_CONTROL;
        internal const uint STANDARD_RIGHTS_WRITE = READ_CONTROL;
        internal const uint STANDARD_RIGHTS_EXECUTE = READ_CONTROL;

        internal const uint STANDARD_RIGHTS_ALL = 0x001F0000;

        internal const uint SPECIFIC_RIGHTS_ALL = 0x0000FFFF;

        
        
        

        internal const uint FILE_SHARE_READ = 0x00000001;
        internal const uint FILE_SHARE_WRITE = 0x00000002;
        internal const uint FILE_SHARE_DELETE = 0x00000004;

        
        
        

        internal const uint FILE_READ_DATA = 0x0001;    
        internal const uint FILE_LIST_DIRECTORY = 0x0001;    
        internal const uint FILE_WRITE_DATA = 0x0002;    
        internal const uint FILE_ADD_FILE = 0x0002;    
        internal const uint FILE_APPEND_DATA = 0x0004;    
        internal const uint FILE_ADD_SUBDIRECTORY = 0x0004;    
        internal const uint FILE_CREATE_PIPE_INSTANCE = 0x0004;    
        internal const uint FILE_READ_EA = 0x0008;    
        internal const uint FILE_WRITE_EA = 0x0010;    
        internal const uint FILE_EXECUTE = 0x0020;    
        internal const uint FILE_TRAVERSE = 0x0020;    
        internal const uint FILE_DELETE_CHILD = 0x0040;    
        internal const uint FILE_READ_ATTRIBUTES = 0x0080;    
        internal const uint FILE_WRITE_ATTRIBUTES = 0x0100;    

        internal const uint FILE_ALL_ACCESS  = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x1FF);

        internal const uint FILE_GENERIC_READ = (STANDARD_RIGHTS_READ | 
            FILE_READ_DATA | 
            FILE_READ_ATTRIBUTES |
            FILE_READ_EA | 
            SYNCHRONIZE);

        internal const uint FILE_GENERIC_WRITE = (STANDARD_RIGHTS_WRITE |
            FILE_WRITE_DATA |
            FILE_WRITE_ATTRIBUTES |
            FILE_WRITE_EA |
            FILE_APPEND_DATA |
            SYNCHRONIZE);

        internal const uint FILE_GENERIC_EXECUTE = (STANDARD_RIGHTS_EXECUTE |
            FILE_READ_ATTRIBUTES |
            FILE_EXECUTE |
            SYNCHRONIZE);

        
        
        

        internal const uint CREATE_NEW = 1;
        internal const uint CREATE_ALWAYS = 2;
        internal const uint OPEN_EXISTING = 3;
        internal const uint OPEN_ALWAYS = 4;
        internal const uint TRUNCATE_EXISTING = 5;

        
        
        

        internal const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        internal const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        internal const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;  
        internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;  
        internal const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;  
        internal const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;  
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;  
        internal const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;  
        internal const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;  
        internal const uint FILE_ATTRIBUTE_REPARSE_POuint = 0x00000400;  
        internal const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;  
        internal const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;  
        internal const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;  
        internal const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;  

        internal const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
        internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        internal const uint FILE_FLAG_RANDOM_ACCESS = 0x10000000;
        internal const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;
        internal const uint FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;
        internal const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        internal const uint FILE_FLAG_POSIX_SEMANTICS = 0x01000000;
        internal const uint FILE_FLAG_OPEN_REPARSE_POuint = 0x00200000;
        internal const uint FILE_FLAG_OPEN_NO_RECALL = 0x00100000;
        internal const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;

        internal const int ERROR_ACCESS_DENIED = 5;
        internal const int ERROR_ALREADY_EXISTS = 183;
        internal const int ERROR_FILE_EXISTS = 80;
        
        internal const int MAX_PATH = 260;
        
        
        
        

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        [DllImport("kernel32.dll", EntryPoint="CreateFileW", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern SafeFileHandle CreateFile(
                                        [In] string filename,
                                        [In] uint accessMode,
                                        [In] uint shareMode,
                                        ref SecurityAttributes securityAttributes,
                                        [In] uint creationDisposition,
                                        [In] uint flags,
                                        [In] IntPtr templateFileHandle);


        #region Data Structures
        
        
        
        [StructLayout(LayoutKind.Sequential)]
            internal struct SecurityAttributes
        {
            internal int length; 
            internal IntPtr securityDescriptor; 
            [MarshalAs(UnmanagedType.Bool)]
            internal bool inheritHandle;

            
            
            
            
            internal SecurityAttributes(bool inheritHandle)
            {
                length = Marshal.SizeOf(typeof(SecurityAttributes));
                securityDescriptor = IntPtr.Zero;
                this.inheritHandle = inheritHandle;
            }
        }
        #endregion 
    }
}
