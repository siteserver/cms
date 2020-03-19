using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;

namespace SSCMS.Utils
{	
	public static class FileUtils
	{
	    public static string TryReadText(string filePath)
	    {
	        var text = string.Empty;

	        try
	        {
	            if (IsFileExists(filePath))
	            {
	                text = ReadText(filePath, Encoding.UTF8);
	            }
	        }
	        catch
	        {
	            // ignored
	        }

	        return text;
	    }

        public static string ReadText(string filePath)
        {
            return ReadText(filePath, Encoding.UTF8);
        }

        public static string ReadText(string filePath, Encoding encoding)
        {
            string text;
            using (var sr = new StreamReader(filePath, encoding))
            {
                text = sr.ReadToEnd();
                sr.Close();
            }
            return text;
        }

        public static async Task<string> ReadTextAsync(string filePath)
        {
            return await ReadTextAsync(filePath, Encoding.UTF8);
        }

        public static async Task<string> ReadTextAsync(string filePath, Encoding encoding)
        {
            string text;
            using (var sr = new StreamReader(filePath, encoding))
            {
                text = await sr.ReadToEndAsync();
                sr.Close();
            }
	        return text;
	    }

        public static async Task WriteTextAsync(string filePath, string content)
        {
            await WriteTextAsync(filePath, content, Encoding.UTF8);
        }

        public static async Task WriteTextAsync(string filePath, string content, Encoding encoding)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            var encodedText = encoding.GetBytes(content);

            using var sourceStream = new FileStream(filePath,
                FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize: 4096, useAsync: true);
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

	    public static void WriteText(string filePath, string content)
	    {
	        WriteText(filePath, Encoding.UTF8, content);
	    }

        public static void WriteText(string filePath, Encoding encoding, string content)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            var file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            using var writer = new StreamWriter(file, encoding);
            writer.Write(content);
            writer.Flush();
            writer.Close();
            file.Close();
        }

	    public static void AppendText(string filePath, string content)
	    {
	        AppendText(filePath, Encoding.UTF8, content);
	    }

	    public static void AppendText(string filePath, Encoding encoding, string content)
	    {
	        DirectoryUtils.CreateDirectoryIfNotExists(filePath);

	        var file = new FileStream(filePath, FileMode.Append, FileAccess.Write);
	        using (var writer = new StreamWriter(file, encoding))
	        {
	            writer.Write(content);
	            writer.Flush();
	            writer.Close();

	            file.Close();
	        }
        }

	    public static async Task AppendTextAsync(string filePath, Encoding encoding, string content)
	    {
	        DirectoryUtils.CreateDirectoryIfNotExists(filePath);

	        var file = new FileStream(filePath, FileMode.Append, FileAccess.Write);
	        using (var writer = new StreamWriter(file, encoding))
	        {
	            await writer.WriteAsync(content);
	            writer.Flush();
	            writer.Close();

	            file.Close();
	        }
	    }

        public static void RemoveReadOnlyAndHiddenIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileAttributes = File.GetAttributes(filePath);
                if (IsReadOnly(fileAttributes) || IsHidden(fileAttributes))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                }
            }
        }

        public static bool IsReadOnly(FileAttributes fileAttributes)
        {
            return ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
        }

        public static bool IsHidden(FileAttributes fileAttributes)
        {
            return (fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        }

        public static FileStream GetFileStreamReadOnly(string filePath)
        {
            return new FileStream(filePath, FileMode.Open);
        }

		public static string ReadBase64StringFromFile(string filePath)
		{
		    var fin = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			var storage = new byte[fin.Length];
			fin.Read(storage, 0, storage.Length);
			fin.Close();
			var text = Convert.ToBase64String(storage, 0, storage.Length);
			return text;  
		}

		public static bool IsFileExists(string filePath)
		{
            return File.Exists(filePath);
		}

        public static bool DeleteFileIfExists(string filePath)
		{
            var retVal = true;
            try
            {
                if (IsFileExists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                //try
                //{
                //    Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                //    fso.DeleteFile(filePath, true);
                //}
                //catch
                //{
                //    retVal = false;
                //}
                retVal = false;
            }
            return retVal;
		}

		public static void DeleteFilesIfExists(string directoryPath, List<string> fileNameArrayList)
		{
			foreach (string fileName in fileNameArrayList)
			{
				var filePath = Path.Combine(directoryPath, fileName);
				DeleteFileIfExists(filePath);
			}
		}

        public static void DeleteFilesIfExists(string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                DeleteFileIfExists(filePath);
            }
        }

        public static bool CopyFile(string sourceFilePath, string destFilePath)
        {
            return CopyFile(sourceFilePath, destFilePath, true);
        }

		public static bool CopyFile(string sourceFilePath, string destFilePath, bool isOverride)
		{
            var retVal = true;
		    try
		    {
		        DirectoryUtils.CreateDirectoryIfNotExists(destFilePath);

		        File.Copy(sourceFilePath, destFilePath, isOverride);
		    }
		    catch
		    {
		        retVal = false;
		    }
		    return retVal;
		}

        //public static bool MoveFile(string sourceFilePath, string destFilePath)
        //{
        //    DirectoryUtils.CreateDirectoryIfNotExists(destFilePath);
        //    bool retVal = true;
        //    try
        //    {
        //        File.Move(sourceFilePath, destFilePath);
        //    }
        //    catch
        //    {
        //        retVal = false;
        //    }
        //    return retVal;
        //}

        public static void MoveFile(string sourceFilePath, string destFilePath, bool isOverride)
        {
            //如果文件不存在，则进行复制。 
            var isExists = IsFileExists(destFilePath);
            if (isOverride)
            {
                if (isExists)
                {
                    DeleteFileIfExists(destFilePath);
                }
                CopyFile(sourceFilePath, destFilePath);
            }
            else if (!isExists)
            {
                CopyFile(sourceFilePath, destFilePath);
            }
        }

        public static Encoding GetFileCharset(string filePath)
        {
            return EncodingType.GetType(filePath);
        }

	    public static string ComputeHash(string filePath)
	    {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "‌​").ToLower();
                }
            }
        }

        public class EncodingType
        {
            /// <summary>
            /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
            /// </summary>
            /// <param name="fileName">文件路径</param>
            /// <returns>文件的编码类型</returns>
            public static Encoding GetType(string fileName)
            {
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var r = GetType(fs);
                fs.Close();
                return r;
            }

            /// <summary>
            /// 通过给定的文件流，判断文件的编码类型
            /// </summary>
            /// <param name="fs">文件流</param>
            /// <returns>文件的编码类型</returns>
            public static Encoding GetType(FileStream fs)
            {
                var reVal = Encoding.Default;

                var r = new BinaryReader(fs, Encoding.Default);
                //int i;
                //int.TryParse(fs.Length.ToString(), out i);
                var i = TranslateUtils.ToInt(fs.Length.ToString());
                var ss = r.ReadBytes(i);
                if (IsUtf8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
                {
                    reVal = Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                r.Close();
                return reVal;

            }

            /// <summary>
            /// 判断是否是不带 BOM 的 UTF8 格式
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            private static bool IsUtf8Bytes(byte[] data)
            {
                var charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数

                foreach (var t in data)
                {
                    var curByte = t;
                    if (charByteCounter == 1)
                    {
                        if (curByte >= 0x80)
                        {
                            //判断当前
                            while (((curByte <<= 1) & 0x80) != 0)
                            {
                                charByteCounter++;
                            }
                            //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                            if (charByteCounter == 1 || charByteCounter > 6)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //若是UTF-8 此时第一位必须为1
                        if ((curByte & 0xC0) != 0x80)
                        {
                            return false;
                        }
                        charByteCounter--;
                    }
                }
                if (charByteCounter > 1)
                {
                    throw new Exception("非预期的byte格式");
                }
                return true;
            }

        }

        public static string GetFileSizeByFilePath(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var theFile = new FileInfo(filePath);
                var fileSize = theFile.Length;
                if (fileSize < 1024)
                {
                    return fileSize.ToString() + "B";
                }
                else if (fileSize >= 1024 && fileSize < 1048576)
                {
                    return (fileSize / 1024).ToString() + "KB";
                }
                else
                {
                    return (fileSize / 1048576).ToString() + "MB";
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool IsTextEditable(FileType type)
        {
            return type == FileType.Ascx || type == FileType.Asp || type == FileType.Aspx || type == FileType.Css || type == FileType.Htm || type == FileType.Html || type == FileType.Js || type == FileType.Jsp || type == FileType.Php || type == FileType.SHtml || type == FileType.Txt || type == FileType.Xml || type == FileType.Json;
        }

        public static bool IsHtml(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = fileExtName.ToLower().Trim();
                if (!fileExtName.StartsWith("."))
                {
                    fileExtName = "." + fileExtName;
                }
                if (fileExtName == ".asp" || fileExtName == ".aspx" || fileExtName == ".htm" || fileExtName == ".html" || fileExtName == ".jsp" || fileExtName == ".php" || fileExtName == ".shtml")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public static bool IsImage(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = fileExtName.ToLower().Trim();
                if (!fileExtName.StartsWith("."))
                {
                    fileExtName = "." + fileExtName;
                }
                if (fileExtName == ".bmp" || fileExtName == ".gif" || fileExtName == ".jpg" || fileExtName == ".jpeg" || fileExtName == ".png" || fileExtName == ".pneg" || fileExtName == ".webp")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public static bool IsZip(string typeStr)
        {
            return StringUtils.EqualsIgnoreCase(".zip", typeStr);
        }

        public static bool IsFlash(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = fileExtName.ToLower().Trim();
                if (!fileExtName.StartsWith("."))
                {
                    fileExtName = "." + fileExtName;
                }
                if (fileExtName == ".swf")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public static bool IsPlayer(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = fileExtName.ToLower().Trim();
                if (!fileExtName.StartsWith("."))
                {
                    fileExtName = "." + fileExtName;
                }
                if (fileExtName == ".flv" || fileExtName == ".avi" || fileExtName == ".mpg" || fileExtName == ".mpeg" || fileExtName == ".smi" || fileExtName == ".mp3" || fileExtName == ".mid" || fileExtName == ".midi" || fileExtName == ".rm" || fileExtName == ".rmb" || fileExtName == ".rmvb" || fileExtName == ".wmv" || fileExtName == ".wma" || fileExtName == ".asf" || fileExtName == ".mov" || fileExtName == ".mp4")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public static bool IsImageOrPlayer(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = fileExtName.ToLower().Trim();
                if (!fileExtName.StartsWith("."))
                {
                    fileExtName = "." + fileExtName;
                }
                if (fileExtName == ".bmp" || fileExtName == ".gif" || fileExtName == ".jpeg" || fileExtName == ".jpg" || fileExtName == ".png" || fileExtName == ".pneg" || fileExtName == ".webp")
                {
                    retVal = true;
                }
                if (retVal == false) retVal = IsPlayer(fileExtName);
            }
            return retVal;
        }

        public static FileType GetType(string typeStr)
        {
            return TranslateUtils.ToEnum(StringUtils.UpperFirst(typeStr), FileType.Unknown);
        }

        public static bool IsType(FileType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (StringUtils.EqualsIgnoreCase("." + type.GetValue(), typeStr))
            {
                return true;
            }
            return false;
        }

        public static bool IsDownload(FileType type)
        {
            var download = false;
            if (IsTextEditable(type) || IsImageOrPlayer(type.GetValue()))
            {
                download = true;
            }
            else if (type == FileType.Pdf || type == FileType.Doc || type == FileType.Docx || type == FileType.Ppt || type == FileType.Pptx || type == FileType.Xls || type == FileType.Xlsx || type == FileType.Mdb)
            {
                download = true;
            }
            return download;
        }
    }
}
