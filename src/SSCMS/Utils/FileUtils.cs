using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Utils
{
    public static class FileUtils
    {
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

        public static async Task WriteStreamAsync(string filePath, MemoryStream stream)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            var encodedText = stream.ToArray();

            await using var sourceStream = new FileStream(filePath,
                FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize: 4096, useAsync: true);
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

        public static async Task WriteTextAsync(string filePath, string content)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            var encodedText = Encoding.UTF8.GetBytes(content);

            using var sourceStream = new FileStream(filePath,
                FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize: 4096, useAsync: true);
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

        public static void WriteText(string filePath, string content)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            var file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            using var writer = new StreamWriter(file, Encoding.UTF8);
            writer.Write(content);
            writer.Flush();
            writer.Close();
            file.Close();
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

        private static bool IsReadOnly(FileAttributes fileAttributes)
        {
            return (fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

        private static bool IsHidden(FileAttributes fileAttributes)
        {
            return (fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        }

        public static FileStream GetFileStreamReadOnly(string filePath)
        {
            return new FileStream(filePath, FileMode.Open);
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

        public static bool CopyFile(string sourceFilePath, string destFilePath, bool isOverride = true)
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

        public static string GetFileSizeByFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return string.Empty;

            var theFile = new FileInfo(filePath);
            return GetFileSizeByFileLength(theFile.Length);
        }

        public static string GetFileSizeByFileLength(long fileLength)
        {
            if (fileLength < 1024)
            {
                return fileLength + "B";
            }

            if (fileLength >= 1024 && fileLength < 1048576)
            {
                return fileLength / 1024 + "KB";
            }

            return fileLength / 1048576 + "MB";
        }

        private static bool IsTextEditable(FileType type)
        {
            return type == FileType.Ascx || type == FileType.Asp || type == FileType.Aspx || type == FileType.Css || type == FileType.Htm || type == FileType.Html || type == FileType.Js || type == FileType.Jsp || type == FileType.Php || type == FileType.SHtml || type == FileType.Txt || type == FileType.Xml || type == FileType.Json;
        }

        public static bool IsHtml(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = StringUtils.TrimAndToLower(fileExtName);
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

        public static bool IsHtml(FileType fileType)
        {
            return fileType == FileType.Asp || fileType == FileType.Aspx || fileType == FileType.Htm || fileType == FileType.Html || fileType == FileType.Jsp || fileType == FileType.Php || fileType == FileType.SHtml;
        }

        public static bool IsImage(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = StringUtils.TrimAndToLower(fileExtName);
                if (!fileExtName.StartsWith("."))
                {
                    fileExtName = "." + fileExtName;
                }
                if (fileExtName == ".jpg" || fileExtName == ".jpeg" || fileExtName == ".gif" || fileExtName == ".png" || fileExtName == ".pneg" || fileExtName == ".bmp"  || fileExtName == ".webp" || fileExtName == ".svg" || fileExtName == ".ico" || fileExtName == ".jfif")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public static bool IsImage(FileType fileType)
        {
            return fileType == FileType.Jpg || fileType == FileType.Jpeg || fileType == FileType.Gif || fileType == FileType.Png || fileType == FileType.Pneg || fileType == FileType.Bmp || fileType == FileType.Webp || fileType == FileType.Svg || fileType == FileType.Ico || fileType == FileType.Jfif;
        }

        public static bool IsZip(string typeStr)
        {
            return StringUtils.EqualsIgnoreCase(".zip", typeStr);
        }

        public static bool IsTxt(string typeStr)
        {
            return StringUtils.EqualsIgnoreCase(".txt", typeStr);
        }

        public static bool IsWord(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = StringUtils.TrimAndToLower(fileExtName);
                if (!fileExtName.StartsWith("."))
                {
                    fileExtName = "." + fileExtName;
                }
                if (fileExtName == ".doc" || fileExtName == ".docx" || fileExtName == ".wps")
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public static bool IsFlash(string fileExtName)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(fileExtName))
            {
                fileExtName = StringUtils.TrimAndToLower(fileExtName);
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
                fileExtName = StringUtils.TrimAndToLower(fileExtName);
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
                fileExtName = StringUtils.TrimAndToLower(fileExtName);
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

        public static FileType GetFileType(string typeStr)
        {
            return TranslateUtils.ToEnum(StringUtils.UpperFirst(StringUtils.Replace(typeStr, ".", string.Empty)), FileType.Unknown);
        }

        public static bool IsFileType(FileType type, string typeStr)
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
            else if (type == FileType.Pdf || type == FileType.Doc || type == FileType.Docx || type == FileType.Ppt || type == FileType.Pptx || type == FileType.Xls || type == FileType.Xlsx || type == FileType.Mdb || type == FileType.Mp3 || type == FileType.Mp4)
            {
                download = true;
            }
            return download;
        }

        public static string ContentMd5(string filePath)
        {
            string output;
            using (var md5 = MD5.Create())
            {
                using var stream = File.OpenRead(filePath);
                var checksum = md5.ComputeHash(stream);
                //output = BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
                output = Convert.ToBase64String(checksum);
            }

            return output;
        }

        public static async Task AppendErrorLogsAsync(string filePath, List<TextLog> logs)
        {
            if (logs == null || logs.Count <= 0) return;

            if (!IsFileExists(filePath))
            {
                await WriteTextAsync(filePath, string.Empty);
            }

            var builder = new StringBuilder();

            foreach (var log in logs)
            {
                builder.AppendLine();
                builder.Append(log);
                builder.AppendLine();
            }

            await AppendTextAsync(filePath, Encoding.UTF8, builder.ToString());
        }

        public static async Task AppendErrorLogAsync(string filePath, TextLog log)
        {
            if (log == null) return;

            if (!IsFileExists(filePath))
            {
                await WriteTextAsync(filePath, string.Empty);
            }

            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append(log);
            builder.AppendLine();

            await AppendTextAsync(filePath, Encoding.UTF8, builder.ToString());
        }

    }
}
