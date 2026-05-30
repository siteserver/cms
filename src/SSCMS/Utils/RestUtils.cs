using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace SSCMS.Utils
{
    public static class RestUtils
    {
        private class InternalServerError
        {
            public string Message { get; set; }
        }

        public static async Task<(bool success, TResult result, string failureMessage)> GetAsync<TResult>(string url, string accessToken = null) where TResult : class

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Get,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }

            var response = await client.ExecuteAsync<TResult>(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, response.Data, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task<(bool success, string result, string errorMessage)> GetStringAsync(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Get,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, response.Content, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task<(bool success, string result, string errorMessage)> PostStringAsync(string url, string body)
        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            request.AddHeader("Content-Type", "application/json");
            request.AddBody(body, "application/json");
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, response.Content, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task<(bool success, TResult result, string failureMessage)> PostAsync<TRequest, TResult>(string url, TRequest body, string accessToken = null) where TResult : class

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }
            request.AddBody(body, "application/json");
            var response = await client.ExecuteAsync<TResult>(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, response.Data, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task<(bool success, string failureMessage)> PostAsync<TRequest>(string url, TRequest body, string accessToken = null) where TRequest : class

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }
            request.AddBody(body, "application/json");
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, null);
            }

            return (false, GetErrorMessage(response));
        }

        public static async Task<(bool success, string failureMessage)> PostAsync(string url, string accessToken = null)

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, null);
            }

            return (false, GetErrorMessage(response));
        }


        public static async Task<(bool success, TResult result, string failureMessage)> PostAsync<TResult>(string url, string accessToken = null) where TResult : class

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }
            var response = await client.ExecuteAsync<TResult>(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, response.Data, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task<(bool success, TResult result, string failureMessage)> UploadAsync<TResult>(string url,
            string filePath, string accessToken) where TResult : class

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            //request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }

            request.AddFile("file", filePath);

            var response = await client.ExecuteAsync<TResult>(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, response.Data, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task<(bool success, string failureMessage)> UploadAsync(string url,
            string filePath, string accessToken)

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            //request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }

            request.AddFile("file", filePath);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, null);
            }

            return (false, GetErrorMessage(response));
        }

        public static async Task<(bool success, string results, string failureMessage)> UploadStringAsync(string url,
            string filePath, string accessToken = null)

        {
            var client = new RestClient(url);
            var request = new RestRequest
            {
                Method = Method.Post,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };
            //request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }

            request.AddFile("file", filePath);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && string.IsNullOrEmpty(response.ErrorMessage))
            {
                return (true, response.Content, null);
            }

            return (false, null, GetErrorMessage(response));
        }

        public static async Task DownloadAsync(string url, string filePath)
        {
            await HttpClientUtils.DownloadAsync(url, filePath);
        }

        public static async Task<string> GetIpAddressAsync()
        {
            var client = new RestClient("https://api.ipify.org/?format=text");
            var request = new RestRequest
            {
                Method = Method.Get,
                // Timeout = -1,
                Timeout = System.Threading.Timeout.InfiniteTimeSpan,
            };

            var response = await client.ExecuteAsync(request);
            return response.Content;
        }

        private static string GetErrorMessage(RestResponse response)
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
