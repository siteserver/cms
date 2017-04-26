using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Top.Api
{
    /// <summary>
    /// 调用出错自动重试客户端。
    /// </summary>
    public class AutoRetryTopClient : DefaultTopClient
    {
        private static readonly TopException RETRY_FAIL = new TopException("sdk.retry-call-fail", "API调用重试失败");

        /// <summary>
        /// 单次请求的最大重试次数，默认值为3次。
        /// </summary>
        private int maxRetryCount = 3;
        /// <summary>
        /// 重试之前休眠时间，默认值为100毫秒。
        /// </summary>
        private int retryWaitTime = 100;
        /// <summary>
        /// 超过最大重试次数时是否抛出异常。
        /// </summary>
        private bool throwIfOverMaxRetry = false;
        /// <summary>
        /// 自定义重试错误码列表。
        /// </summary>
        private IDictionary<string, bool> retryErrorCodes;

        public AutoRetryTopClient(string serverUrl, string appKey, string appSecret)
            : base(serverUrl, appKey, appSecret)
        {
        }

        public AutoRetryTopClient(string serverUrl, string appKey, string appSecret, string format)
            : base(serverUrl, appKey, appSecret, format)
        {
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
            T rsp = null;
            TopException exp = null;

            for (int i = 0; i < maxRetryCount; i++)
            {
                if (i > 0)
                {
                    if ((rsp != null && ((rsp.SubErrCode != null && rsp.SubErrCode.StartsWith("isp."))
                        || (retryErrorCodes != null && retryErrorCodes.ContainsKey(rsp.SubErrCode)))) || exp != null)
                    {
                        Thread.Sleep(retryWaitTime);
                        topLogger.Warn(BuildRetryLog(request.GetApiName(), request.GetParameters(), i));
                    }
                    else
                    {
                        break;
                    }
                }

                try
                {
                    rsp = base.Execute(request, session);
                    if (rsp.IsError)
                    {
                        if (i == maxRetryCount && throwIfOverMaxRetry)
                        {
                            throw RETRY_FAIL;
                        }
                    }
                    else
                    {
                        return rsp;
                    }
                }
                catch (TopException e)
                {
                    if (exp == null)
                    {
                        exp = e;
                    }
                }
            }

            if (exp != null)
            {
                throw exp;
            }
            else
            {
                return rsp;
            }
        }

        public void SetMaxRetryCount(int maxRetryCount)
        {
            this.maxRetryCount = maxRetryCount;
        }

        public void SetRetryWaitTime(int retryWaitTime)
        {
            this.retryWaitTime = retryWaitTime;
        }

        public void SetThrowIfOverMaxRetry(bool throwIfOverMaxRetry)
        {
            this.throwIfOverMaxRetry = throwIfOverMaxRetry;
        }

        public void AddRetryErrorCode(string errorCode)
        {
            if (this.retryErrorCodes == null)
            {
                this.retryErrorCodes = new Dictionary<string, bool>();
            }
            this.retryErrorCodes.Add(errorCode, false);
        }

        private string BuildRetryLog(string apiName, IDictionary<string, string> parameters, int retryCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(apiName).Append(" retry call ").Append(retryCount);
            if (parameters.ContainsKey("fields"))
            {
                parameters.Remove("fields");
            }
            sb.Append(" times, parameters=").Append(parameters);
            return sb.ToString();
        }
    }
}
