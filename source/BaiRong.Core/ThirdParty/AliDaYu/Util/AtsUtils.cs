using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;

namespace Top.Api.Util
{
    /// <summary>
    /// 异步API下载工具类。
    /// </summary>
    public abstract class AtsUtils
    {
        private const string CTYPE_OCTET = "application/octet-stream";
        private static Regex regex = new Regex("attachment;filename=\"([\\w\\-]+)\"", RegexOptions.Compiled);

        /// <summary>
        /// 通过HTTP GET方式下载文件到指定的目录。
        /// </summary>
        /// <param name="url">需要下载的URL</param>
        /// <param name="destDir">需要下载到的目录</param>
        /// <returns>下载后的文件</returns>
        public static string Download(string url, string destDir)
        {
            string file = null;

            try
            {
                WebUtils wu = new WebUtils();
                HttpWebRequest req = wu.GetWebRequest(url, "GET", null);
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                if (CTYPE_OCTET.Equals(rsp.ContentType))
                {
                    file = Path.Combine(destDir, GetFileName(rsp.Headers["Content-Disposition"]));
                    using (System.IO.Stream rspStream = rsp.GetResponseStream())
                    {
                        int len = 0;
                        byte[] buf = new byte[8192];
                        using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate))
                        {
                            while ((len = rspStream.Read(buf, 0, buf.Length)) > 0)
                            {
                                fileStream.Write(buf, 0, len);
                            }
                        }
                    }
                }
                else
                {
                    throw new TopException(wu.GetResponseAsString(rsp, Encoding.UTF8));
                }
            }
            catch (WebException e)
            {
                throw new TopException("isv.file-already-download", e.Message);
            }
            return file;
        }

        /// <summary>
        /// 解压gzip文件到指定的目录，目前只能解压gzip包里面只包含一个文件的压缩包。
        /// </summary>
        /// <param name="gzipFile">需要解压的gzip文件</param>
        /// <param name="destDir">需要解压到的目录（不能和压缩文件在同一个目录）</param>
        /// <returns>解压后的文件</returns>
        public static string Ungzip(string gzipFile, string destDir)
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(gzipFile));
            using (System.IO.Stream output = File.Create(destFile))
            {
                using (System.IO.Stream input = new GZipStream(File.Open(gzipFile, FileMode.Open), CompressionMode.Decompress))
                {
                    int size = 0;
                    byte[] buf = new byte[8192];
                    while ((size = input.Read(buf, 0, buf.Length)) > 0)
                    {
                        output.Write(buf, 0, size);
                    }
                }
            }

            return destFile;
        }

        /// <summary>
        /// 解压zip文件到指定的目录。
        /// </summary>
        /// <param name="zipFile">需要解压的zip文件</param>
        /// <param name="destDir">需要解压到的目录</param>
        /// <returns>解压后的文件列表（不包含文件夹）</returns>
        public static List<string> Unzip(string zipFile, string destDir)
        {
            List<string> files = new List<string>();

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFile)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsDirectory)
                    {
                        Directory.CreateDirectory(Path.Combine(destDir, theEntry.Name));
                        continue;
                    }

                    string fileName = Path.Combine(destDir, theEntry.Name);
                    using (FileStream streamWriter = File.Create(fileName))
                    {
                        int size = 0;
                        byte[] buf = new byte[8192];
                        while ((size = s.Read(buf, 0, buf.Length)) > 0)
                        {
                            streamWriter.Write(buf, 0, size);
                        }
                    }
                    files.Add(fileName);
                }
            }

            return files;
        }

        /// <summary>
        /// 检查指定文件的md5sum和指定的检验码是否一致。
        /// </summary>
        /// <param name="fileName">需要检验的文件</param>
        /// <param name="checkCode">已知的md5sum检验码</param>
        /// <returns>true/false</returns>
        public static bool CheckMd5sum(string fileName, string checkCode)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString().Equals(checkCode);
            }
        }

        private static string GetFileName(string contentDisposition)
        {
            Match match = regex.Match(contentDisposition);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            else
            {
                throw new TopException("Invalid response header format!");
            }
        }
    }
}
