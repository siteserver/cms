using System;
using System.Collections.Generic;
using System.Text;
using Top.Api.Parser;
using Top.Api.Util;

namespace Top.Api
{
    public class BatchTopClient : DefaultTopClient
    {
        private const string BATCH_API_HEADER_SPLIT = "top-api-separator"; // 批量API用户自定义分隔符Header Key
        private const string BATCH_API_PUBLIC_PARAMETER = "#PUBLIC#"; // 批量API公共参数头
        private const string BATCH_API_DEFAULT_SPLIT = "\r\n-S-\r\n";// 批量API默认分隔符
        private const string BATCH_API_CONTENT_TYPE = "text/plain;charset=utf-8";// 批量API请求文档类型

        private string batchServerUrl;
        private string batchApiSeparator; // 自定义批量API分隔符

        public BatchTopClient(string serverUrl, string appKey, string appSecret)
            : base(BuildApiServerUrl(serverUrl), appKey, appSecret)
        {
            this.batchServerUrl = serverUrl;
        }

        public BatchTopClient(string serverUrl, string appKey, string appSecret, string format)
            : base(BuildApiServerUrl(serverUrl), appKey, appSecret, format)
        {
            this.batchServerUrl = serverUrl;
        }

        public void SetBatchApiSeparator(string batchApiSeparator)
        {
            this.batchApiSeparator = batchApiSeparator;
        }

        public override T Execute<T>(ITopRequest<T> request)
        {
            return Execute<T>(request, null);
        }

        public override T Execute<T>(ITopRequest<T> request, string session)
        {
            return Execute<T>(request, session, DateTime.Now);
        }

        public override T Execute<T>(ITopRequest<T> request, string session, DateTime timestamp)
        {
            if (typeof(TopBatchRequest) == request.GetType())
            {
                return DoExecute<T>(request, session, timestamp);
            }
            else
            {
                return base.Execute<T>(request, session, timestamp);
            }
        }

        private T DoExecute<T>(ITopRequest<T> request, string session, DateTime timestamp) where T : TopResponse
        {
            long start = DateTime.Now.Ticks;

            TopBatchRequest batchRequest = request as TopBatchRequest;
            List<ITopRequest<TopResponse>> requestList = batchRequest.RequestList;
            if (requestList == null || requestList.Count == 0)
            {
                throw new TopException("40", "client-error:api request list is empty");
            }

            // 本地校验请求参数
            if (batchRequest.PublicParams == null || batchRequest.PublicParams.Count == 0)
            {
                for (int i = 0; i < requestList.Count; i++)
                {
                    try
                    {
                        requestList[i].Validate();
                    }
                    catch (TopException e)
                    {
                        return CreateErrorResponse<T>(e.ErrorCode, e.ErrorMsg);
                    }
                }
            }

            // 添加协议级请求参数
            TopDictionary parameters = new TopDictionary();
            parameters.Add(Constants.VERSION, "2.0");
            parameters.Add(Constants.APP_KEY, appKey);
            parameters.Add(Constants.TIMESTAMP, timestamp);
            parameters.Add(Constants.FORMAT, format);
            parameters.Add(Constants.SIGN_METHOD, Constants.SIGN_METHOD_HMAC);
            parameters.Add(Constants.PARTNER_ID, GetSdkVersion());
            parameters.Add(Constants.TARGET_APP_KEY, request.GetTargetAppKey());
            parameters.Add(Constants.SESSION, session);
            if (Constants.FORMAT_JSON.Equals(format) && this.useSimplifyJson)
            {
                parameters.Add(Constants.SIMPLIFY, "true");
            }

            // 添加自定义分隔符
            string separator = BATCH_API_DEFAULT_SPLIT;
            if (!string.IsNullOrEmpty(batchApiSeparator))
            {
                batchRequest.AddHeaderParameter(BATCH_API_HEADER_SPLIT, separator = batchApiSeparator);
            }

            // 是否需要压缩响应
            if (this.useGzipEncoding)
            {
                batchRequest.AddHeaderParameter(Constants.ACCEPT_ENCODING, Constants.CONTENT_ENCODING_GZIP);
            }

            try
            {
                // 添加公共请求头
                if (!string.IsNullOrEmpty(batchRequest.PublicMethod))
                {
                    batchRequest.AddPublicParam(Constants.METHOD, batchRequest.PublicMethod);
                }
                else
                {
                    if (IsSameRequest(requestList))
                    {
                        batchRequest.AddPublicParam(Constants.METHOD, requestList[0].GetApiName());
                    }
                }

                // 构建批量请求主体
                StringBuilder requestBody = new StringBuilder();
                string publicParamStr = WebUtils.BuildQuery(batchRequest.PublicParams);
                if (!string.IsNullOrEmpty(publicParamStr))
                {
                    requestBody.Append(BATCH_API_PUBLIC_PARAMETER).Append(publicParamStr).Append(separator);
                }

                // 组装每个API的请求参数
                for (int i = 0; i < requestList.Count; i++)
                {
                    ITopRequest<TopResponse> bRequest = requestList[i];
                    bRequest.SetBatchApiOrder(i);
                    IDictionary<string, string> apiParams = bRequest.GetParameters();
                    // 如果单个API的方法和批量API的公共方法不一致，那么需要设置单个API的方法名称
                    if (!string.IsNullOrEmpty(bRequest.GetApiName()) && !bRequest.GetApiName().Equals(batchRequest.PublicMethod))
                    {
                        apiParams.Add(Constants.METHOD, bRequest.GetApiName());
                    }
                    if (!string.IsNullOrEmpty(request.GetBatchApiSession()))
                    {
                        apiParams.Add(Constants.SESSION, bRequest.GetBatchApiSession());
                    }
                    if (!string.IsNullOrEmpty(request.GetTargetAppKey()))
                    {
                        apiParams.Add(Constants.TARGET_APP_KEY, bRequest.GetTargetAppKey());
                    }

                    string apiParamStr = WebUtils.BuildQuery(apiParams);
                    if (string.IsNullOrEmpty(apiParamStr))
                    {
                        apiParamStr = "N";
                    }
                    requestBody.Append(apiParamStr);
                    if (i != requestList.Count - 1)
                    {
                        requestBody.Append(separator);
                    }
                }

                string apiBody = requestBody.ToString();

                // 添加签名参数
                parameters.Add(Constants.SIGN, TopUtils.SignTopRequest(parameters, apiBody, appSecret, Constants.SIGN_METHOD_HMAC));

                // 发起批量请求
                string fullUrl = WebUtils.BuildRequestUrl(this.batchServerUrl, parameters);
                string rsp = webUtils.DoPost(fullUrl, Encoding.UTF8.GetBytes(apiBody), BATCH_API_CONTENT_TYPE, batchRequest.GetHeaderParameters());

                // 构造响应解释器
                ITopParser<TopResponse> parser = null;
                if (Constants.FORMAT_XML.Equals(format))
                {
                    parser = new TopXmlParser<TopResponse>();
                }
                else
                {
                    if (this.useSimplifyJson)
                    {
                        parser = new TopSimplifyJsonParser<TopResponse>();
                    }
                    else
                    {
                        parser = new TopJsonParser<TopResponse>();
                    }
                }

                // 解释响应结果
                TopBatchResponse batchResponse = new TopBatchResponse();
                batchResponse.Body = rsp;

                string[] responseArray = batchResponse.Body.Split(new string[] { separator }, StringSplitOptions.None);
                // 批量API在走单通道验证时没通过，如前面验证，此时只有一个报错信息
                if (responseArray.Length > 0 && responseArray.Length != requestList.Count)
                {
                    TopResponse tRsp = parser.Parse(responseArray[0], requestList[0].GetType().BaseType.GetGenericArguments()[0]);
                    batchResponse.ErrCode = tRsp.ErrCode;
                    batchResponse.ErrMsg = tRsp.ErrMsg;
                    batchResponse.SubErrCode = tRsp.SubErrCode;
                    batchResponse.SubErrMsg = tRsp.SubErrMsg;
                }
                else
                {
                    for (int i = 0; i < responseArray.Length; i++)
                    {
                        TopResponse tRsp = parser.Parse(responseArray[i], requestList[i].GetType().BaseType.GetGenericArguments()[0]);
                        tRsp.Body = responseArray[i];
                        batchResponse.AddResponse(tRsp);
                    }
                }

                if (batchResponse.IsError)
                {
                    TimeSpan latency = new TimeSpan(DateTime.Now.Ticks - start);
                    TraceApiError(appKey, "BatchApi", batchServerUrl, parameters, latency.TotalMilliseconds, batchResponse.Body);
                }

                return batchResponse as T;
            }
            catch (Exception e)
            {
                TimeSpan latency = new TimeSpan(DateTime.Now.Ticks - start);
                TraceApiError(appKey, "BatchApi", batchServerUrl, parameters, latency.TotalMilliseconds, e.GetType() + ": " + e.Message);
                throw e;
            }
        }

        private bool IsSameRequest<T>(List<ITopRequest<T>> requestList) where T : TopResponse
        {
            if (requestList != null && requestList.Count > 1) // 只有两个或以上的请求才考虑合并
            {
                string firstMethod = requestList[0].GetApiName();
                for (int i = 1; i < requestList.Count; i++)
                {
                    string currentMethod = requestList[i].GetApiName();
                    if (!firstMethod.Equals(currentMethod))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static string BuildApiServerUrl(string batchServerUrl)
        {
            if (batchServerUrl.Contains("/router/batch"))
            {
                return batchServerUrl.Replace("/router/batch", "/router/rest");
            }
            return batchServerUrl;
        }
    }
}
