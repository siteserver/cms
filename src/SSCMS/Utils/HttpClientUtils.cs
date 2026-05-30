using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SSCMS.Utils
{
    public static class HttpClientUtils
    {
        private const int MaxRedirects = 5;

        public static async Task<string> GetStringAsync(string url)
        {
            return await GetStringAsync(url, Encoding.UTF8);
        }

        public static async Task<string> GetStringAsync(string url, Encoding encoding)
        {
            try
            {
                var uri = await ValidatePublicHttpUrlAsync(url);
                var bytes = await GetByteArrayAsync(uri);
                return encoding == Encoding.UTF8 ? Encoding.UTF8.GetString(bytes) : ConvertBytesToString(bytes, encoding);
            }
            catch (Exception ex)
            {
                throw new Exception($"页面地址“{url}”无法访问，{ex.Message}！");
            }
        }

        private static string ConvertBytesToString(byte[] bytes, Encoding encoding)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var toEncoding = Encoding.UTF8;
            var toBytes = Encoding.Convert(encoding, toEncoding, bytes);
            return toEncoding.GetString(toBytes);
        }

        public static async Task<bool> DownloadAsync(string remoteUrl, string filePath)
        {
            try
            {
                var uri = await ValidatePublicHttpUrlAsync(remoteUrl);
                DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                FileUtils.DeleteFileIfExists(filePath);

                using (var fs = new FileStream(filePath, FileMode.CreateNew))
                {
                    await DownloadToStreamAsync(uri, fs);
                }

                // using var client = new WebClient();
                // client.DownloadFile(remoteUrl, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"页面地址“{remoteUrl}”无法访问，{ex.Message}！");
            }
            return true;
        }

        public static async Task<Uri> ValidatePublicHttpUrlAsync(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Only absolute HTTP(S) URLs are allowed.", nameof(url));
            }

            if (IPAddress.TryParse(uri.Host, out var address))
            {
                if (!IsPublicIPAddress(address))
                {
                    throw new ArgumentException("Private, loopback, link-local, and reserved IP addresses are not allowed.", nameof(url));
                }

                return uri;
            }

            var addresses = await Dns.GetHostAddressesAsync(uri.IdnHost);
            if (addresses.Length == 0 || addresses.Any(address => !IsPublicIPAddress(address)))
            {
                throw new ArgumentException("The URL host must resolve only to public IP addresses.", nameof(url));
            }

            return uri;
        }

        private static async Task<byte[]> GetByteArrayAsync(Uri uri)
        {
            for (var i = 0; i <= MaxRedirects; i++)
            {
                uri = await ValidatePublicHttpUrlAsync(uri.ToString());

                using (var client = CreateHttpClient())
                using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (TryGetRedirectUri(uri, response, out var redirectUri))
                    {
                        uri = redirectUri;
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }

            throw new HttpRequestException("Too many redirects.");
        }

        private static async Task DownloadToStreamAsync(Uri uri, Stream output)
        {
            for (var i = 0; i <= MaxRedirects; i++)
            {
                uri = await ValidatePublicHttpUrlAsync(uri.ToString());

                using (var client = CreateHttpClient())
                using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (TryGetRedirectUri(uri, response, out var redirectUri))
                    {
                        uri = redirectUri;
                        continue;
                    }

                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        await stream.CopyToAsync(output);
                    }
                    return;
                }
            }

            throw new HttpRequestException("Too many redirects.");
        }

        private static HttpClient CreateHttpClient()
        {
            return new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false
            });
        }

        private static bool TryGetRedirectUri(Uri requestUri, HttpResponseMessage response, out Uri redirectUri)
        {
            redirectUri = null;
            var statusCode = (int)response.StatusCode;
            if (statusCode < 300 || statusCode > 399 || response.Headers.Location == null)
            {
                return false;
            }

            redirectUri = response.Headers.Location.IsAbsoluteUri
                ? response.Headers.Location
                : new Uri(requestUri, response.Headers.Location);
            return true;
        }

        private static bool IsPublicIPAddress(IPAddress address)
        {
            if (address.IsIPv4MappedToIPv6)
            {
                address = address.MapToIPv4();
            }

            if (IPAddress.IsLoopback(address))
            {
                return false;
            }

            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var bytes = address.GetAddressBytes();
                return bytes[0] != 0 &&
                       bytes[0] != 10 &&
                       bytes[0] != 127 &&
                       !(bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127) &&
                       !(bytes[0] == 169 && bytes[1] == 254) &&
                       !(bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) &&
                       !(bytes[0] == 192 && bytes[1] == 168) &&
                       !(bytes[0] == 192 && bytes[1] == 0 && bytes[2] == 0) &&
                       !(bytes[0] == 198 && (bytes[1] == 18 || bytes[1] == 19)) &&
                       bytes[0] < 224;
            }

            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                var bytes = address.GetAddressBytes();
                return !address.IsIPv6LinkLocal &&
                       !address.IsIPv6Multicast &&
                       !address.IsIPv6SiteLocal &&
                       !address.Equals(IPAddress.IPv6None) &&
                       !address.Equals(IPAddress.IPv6Any) &&
                       (bytes[0] & 0xfe) != 0xfc;
            }

            return false;
        }
    }
}
