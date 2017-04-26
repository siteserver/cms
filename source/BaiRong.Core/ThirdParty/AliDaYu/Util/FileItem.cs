using System.IO;

namespace Top.Api.Util
{
    /// <summary>
    /// 文件元数据。
    /// 可以使用以下几种构造方法：
    /// 本地路径：new FileItem("C:/temp.jpg");
    /// 本地文件：new FileItem(new FileInfo("C:/temp.jpg"));
    /// 字节数组：new FileItem("abc.jpg", bytes);
    /// 输入流：new FileItem("abc.jpg", stream);
    /// </summary>
    public class FileItem
    {
        private Contract contract;

        /// <summary>
        /// 基于本地文件的构造器。
        /// </summary>
        /// <param name="fileInfo">本地文件</param>
        public FileItem(FileInfo fileInfo)
        {
            this.contract = new LocalContract(fileInfo);
        }

        /// <summary>
        /// 基于本地文件全路径的构造器。
        /// </summary>
        /// <param name="filePath">本地文件全路径</param>
        public FileItem(string filePath) : this(new FileInfo(filePath))
        {
        }

        /// <summary>
        /// 基于文件名和字节数组的构造器。
        /// </summary>
        /// <param name="fileName">文件名称（服务端持久化字节数组到磁盘时的文件名）</param>
        /// <param name="content">文件字节数组</param>
        public FileItem(string fileName, byte[] content) : this(fileName, content, null)
        {
        }

        /// <summary>
        /// 基于文件名、字节数组和媒体类型的构造器。
        /// </summary>
        /// <param name="fileName">文件名（服务端持久化字节数组到磁盘时的文件名）</param>
        /// <param name="content">文件字字节数组</param>
        /// <param name="mimeType">媒体类型</param>
        public FileItem(string fileName, byte[] content, string mimeType)
        {
            this.contract = new ByteArrayContract(fileName, content, mimeType);
        }

        /// <summary>
        /// 基于文件名和输入流的构造器。
        /// </summary>
        /// <param name="fileName">文件名称（服务端持久化输入流到磁盘时的文件名）</param>
        /// <param name="content">文件输入流</param>
        public FileItem(string fileName, Stream stream) : this(fileName, stream, null)
        {
        }

        /// <summary>
        /// 基于文件名、输入流和媒体类型的构造器。
        /// </summary>
        /// <param name="fileName">文件名（服务端持久化输入流到磁盘时的文件名）</param>
        /// <param name="content">文件输入流</param>
        /// <param name="mimeType">媒体类型</param>
        public FileItem(string fileName, Stream stream, string mimeType)
        {
            this.contract = new StreamContract(fileName, stream, mimeType);
        }

        public bool IsValid()
        {
            return this.contract.IsValid();
        }

        public long GetFileLength()
        {
            return this.contract.GetFileLength();
        }

        public string GetFileName()
        {
            return this.contract.GetFileName();
        }

        public string GetMimeType()
        {
            return this.contract.GetMimeType();
        }

        public void Write(Stream output)
        {
            this.contract.Write(output);
        }
    }

    internal interface Contract
    {
        bool IsValid();
        string GetFileName();
        string GetMimeType();
        long GetFileLength();
        void Write(Stream output);
    }

    internal class LocalContract : Contract
    {
        private FileInfo fileInfo;

        public LocalContract(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public long GetFileLength()
        {
            return this.fileInfo.Length;
        }

        public string GetFileName()
        {
            return this.fileInfo.Name;
        }

        public string GetMimeType()
        {
            return Constants.CTYPE_DEFAULT;
        }

        public bool IsValid()
        {
            return this.fileInfo != null && this.fileInfo.Exists;
        }

        public void Write(Stream output)
        {
            using (BufferedStream bfs = new BufferedStream(this.fileInfo.OpenRead()))
            {
                int n = 0;
                byte[] buffer = new byte[Constants.READ_BUFFER_SIZE];
                while ((n = bfs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, n);
                }
            }
        }
    }

    internal class ByteArrayContract : Contract
    {
        private string fileName;
        private byte[] content;
        private string mimeType;

        public ByteArrayContract(string fileName, byte[] content, string mimeType)
        {
            this.fileName = fileName;
            this.content = content;
            this.mimeType = mimeType;
        }

        public bool IsValid()
        {
            return this.content != null && this.fileName != null;
        }

        public long GetFileLength()
        {
            return this.content.Length;
        }

        public string GetFileName()
        {
            return this.fileName;
        }

        public string GetMimeType()
        {
            if (string.IsNullOrEmpty(this.mimeType))
            {
                return Constants.CTYPE_DEFAULT;
            }
            else
            {
                return this.mimeType;
            }
        }

        public void Write(Stream output)
        {
            output.Write(this.content, 0, this.content.Length);
        }
    }

    internal class StreamContract : Contract
    {
        private string fileName;
        private Stream stream;
        private string mimeType;

        public StreamContract(string fileName, Stream stream, string mimeType)
        {
            this.fileName = fileName;
            this.stream = stream;
            this.mimeType = mimeType;
        }

        public long GetFileLength()
        {
            return 0L;
        }

        public string GetFileName()
        {
            return this.fileName;
        }

        public string GetMimeType()
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                return Constants.CTYPE_DEFAULT;
            }
            else
            {
                return this.mimeType;
            }
        }

        public bool IsValid()
        {
            return this.stream != null && this.fileName != null;
        }

        public void Write(Stream output)
        {
            using (this.stream)
            {
                int n = 0;
                byte[] buffer = new byte[Constants.READ_BUFFER_SIZE];
                while ((n = this.stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, n);
                }
            }
        }
    }
}
