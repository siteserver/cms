using System.IO;
using System.IO.Compression;

namespace Taobao.Top.Link.Util
{
    /// <summary>zip helper compatible with java version
    /// </summary>
    public class GZIPHelper
    {
        public static byte[] Zip(byte[] value)
        {
            using (var stream = new MemoryStream())
            using (var zip = new GZipStream(stream, CompressionMode.Compress))
            {
                zip.Write(value, 0, value.Length);
                zip.Close();
                return stream.ToArray();
            }
        }

        public static byte[] Unzip(byte[] value)
        {
            using (var stream = new MemoryStream(value))
            using (var zip = new GZipStream(stream, CompressionMode.Decompress))
            using (var unzip = new MemoryStream())
            {
                var buffer = new byte[1024];
                var r = 0;
                while ((r = zip.Read(buffer, 0, buffer.Length)) > 0)
                    unzip.Write(buffer, 0, r);
                return unzip.ToArray();
            }
        }
    }
}
