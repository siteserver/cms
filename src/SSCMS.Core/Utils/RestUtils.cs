using System.IO;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class RestUtils
    {
        private class InternalServerError
        {
            public string Message { get; set; }
        }

        public static async Task<(bool success, TResult result, string failureMessage)> GetAsync<TResult>(string url, string accessToken = null) where TResult : class

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(url)
            {
                Timeout = -1,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }

            var response = await client.ExecuteAsync<TResult>(request);

            if (response.IsSuccessful) {
              return (true, response.Data, null);
            }

            return (false, null, GetErrorMessage(response));
        }


        public static async Task<(bool success, TResult result, string failureMessage)> PostAsync<TRequest, TResult>(string url, TRequest body, string accessToken = null) where TResult : class

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(url)
            {
                Timeout = -1,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }
            request.AddParameter("application/json", TranslateUtils.JsonSerialize(body), ParameterType.RequestBody);
            var response = await client.ExecuteAsync<TResult>(request);

            if (response.IsSuccessful) {
              return (true, response.Data, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task<(bool success, string failureMessage)> PostAsync<TRequest>(string url, TRequest body, string accessToken = null) where TRequest : class

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(url)
            {
                Timeout = -1,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }
            request.AddParameter("application/json", TranslateUtils.JsonSerialize(body), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful) {
              return (true, null);
            }

            return (false, GetErrorMessage(response));
        }

        public static async Task<(bool success, string failureMessage)> UploadAsync(string url,
            string filePath, string accessToken)

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(url)
            {
                Timeout = -1,
                RemoteCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true
            };
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }

            request.AddFile("file", filePath);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful) {
              return (true, null);
            }

            return (false, GetErrorMessage(response));
        }

        public static void Download(string url, string filePath)
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            FileUtils.DeleteFileIfExists(filePath);
            FileUtils.WriteText(filePath, string.Empty);
            using (var writer = File.OpenWrite(filePath))
            {
                var client = new RestClient(url);
                var request = new RestRequest
                {
                    ResponseWriter = responseStream =>
                    {
                        using (responseStream)
                        {
                            responseStream.CopyTo(writer);
                        }
                    }
                };

                client.DownloadData(request);
            }
        }

        public static async Task<string> GetIpAddressAsync()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient("https://api.open.21ds.cn/apiv1/iptest?apkey=iptest")
            {
                Timeout = -1,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var request = new RestRequest(Method.GET);

            var response = await client.ExecuteAsync(request);
            return response.Content;
        }

        private static string GetErrorMessage(IRestResponse response)
        {
            var errorMessage = string.Empty;
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
                if (error != null)
                {
                    errorMessage = error.Message;
                }
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = response.ErrorMessage;
            }
            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = StringUtils.Trim(response.Content, '"');
            }
            return errorMessage;
        }
    }
}
