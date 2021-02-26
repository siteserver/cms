using System.IO;
using System.Net;
using RestSharp;
using SSCMS.Core.Plugins;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class RestUtils
    {
        private class InternalServerError
        {
            public string Message { get; set; }
        }

        public static (bool success, TResult result, string failureMessage) Get<TResult>(string relatedUrl, string accessToken = null) where TResult : class

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(CloudUtils.Api.GetCliUrl(relatedUrl))
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
            var response = client.Execute<TResult>(request);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
                    if (error != null)
                    {
                        return (false, null, error.Message);
                    }
                }
                return (false, null, response.ErrorMessage);
            }

            return (true, response.Data, null);
        }


        public static (bool success, TResult result, string failureMessage) Post<TRequest, TResult>(string relatedUrl, TRequest body, string accessToken = null) where TResult : class

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(CloudUtils.Api.GetCliUrl(relatedUrl))
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
            var response = client.Execute<TResult>(request);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
                    if (error != null)
                    {
                        return (false, null, error.Message);
                    }
                }
                return (false, null, response.ErrorMessage);
            }

            return (true, response.Data, null);
        }

        public static (bool success, string failureMessage) Post<TRequest>(string relatedUrl, TRequest body, string accessToken = null) where TRequest : class

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(CloudUtils.Api.GetCliUrl(relatedUrl))
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
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
                    if (error != null)
                    {
                        return (false, error.Message);
                    }
                }
                return (false, response.ErrorMessage);
            }

            return (true, null);
        }

        public static (bool success, string failureMessage) Upload(string relatedUrl, string filePath, string accessToken)

        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient(CloudUtils.Api.GetCliUrl(relatedUrl))
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
            request.AddFile("file", filePath);
            var response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
                    if (error != null)
                    {
                        return (false, error.Message);
                    }
                }
                return (false, response.ErrorMessage);
            }

            return (true, null);
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

        public static string GetIpAddress()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            var client = new RestClient("https://api.open.21ds.cn/apiv1/iptest?apkey=iptest")
            {
                Timeout = -1,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var request = new RestRequest(Method.GET);

            var response = client.Execute(request);
            return response.Content;
        }
    }
}
