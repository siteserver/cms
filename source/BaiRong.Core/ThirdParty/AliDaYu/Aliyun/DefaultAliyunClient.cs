using Aliyun.Api.Parser;
using Aliyun.Api.Util;
using FastJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Top.Api;
using Top.Api.Util;

namespace Aliyun.Api
{
    /// <summary>
    /// 基于REST的阿里云客户端。
    /// </summary>
    public class DefaultAliyunClient : IAliyunClient
    {

        public const string FORMAT_XML = "xml";
        public const string FORMAT_JSON = "json";
        public const string HTTP_METHOD_POST = "POST";
        public const string SDK_VERSION = "top-sdk-net-dynamicVersionNo"; // SDK自动生成会替换成真实的版本


        private AliyunWebUtils webUtils;
        private ITopLogger topLogger;
        private bool disableParser = false; // 禁用响应结果解释
        private bool disableTrace = false; // 禁用日志调试功能
        private IDictionary<string, string> systemParameters; // 设置所有请求共享的系统级参数

        private const string ISO8601_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";
        private const string ENCODING_UTF8 = "UTF-8";

        private string httpMethod = HTTP_METHOD_POST;
        private string format = FORMAT_XML;
        private string serverUrl;
        private string accessKeyId;
        private string accessKeySecret;

        #region DefaultTopClient Constructors

        public DefaultAliyunClient(string serverUrl, string accessKeyId, string accessKeySecret)
        {
            this.accessKeyId = accessKeyId;
            this.accessKeySecret = accessKeySecret;
            this.serverUrl = serverUrl;
            this.webUtils = new AliyunWebUtils();
            this.topLogger = DefaultTopLogger.GetDefault();
        }

        public DefaultAliyunClient(string serverUrl, string accessKeyId, string accessKeySecret, string format)
            : this(serverUrl, accessKeyId, accessKeySecret)
        {
            this.format = format;
        }

        #endregion


        public void SetTopLogger(ITopLogger topLogger)
        {
            this.topLogger = topLogger;
        }

        public void SetTimeout(int timeout)
        {
            this.webUtils.Timeout = timeout;
        }

        public void SetDisableParser(bool disableParser)
        {
            this.disableParser = disableParser;
        }

        public void SetDisableTrace(bool disableTrace)
        {
            this.disableTrace = disableTrace;
        }

        public void SetSystemParameters(IDictionary<string, string> systemParameters)
        {
            this.systemParameters = systemParameters;
        }

        #region ITopClient Members

        public T Execute<T>(IAliyunRequest<T> request) where T : AliyunResponse
        {
            return Execute<T>(request, null);
        }

        public T Execute<T>(IAliyunRequest<T> request, string session) where T : AliyunResponse
        {
            return Execute<T>(request, session, DateTime.Now);
        }

        public T Execute<T>(IAliyunRequest<T> request, string session, DateTime timestamp) where T : AliyunResponse
        {
            return DoExecute<T>(request, session, timestamp);
        }

        #endregion

        private T DoExecute<T>(IAliyunRequest<T> request, string session, DateTime timestamp) where T : AliyunResponse
        {
            // 提前检查业务参数
            try
            {
                request.Validate();
            }
            catch (TopException e)
            {
                return CreateErrorResponse<T>(e.ErrorCode, e.ErrorMsg);
            }

            // 添加协议级请求参数
            TopDictionary txtParams = new TopDictionary(request.GetParameters());
            txtParams.AddAll(this.systemParameters);
            AddCommonParams(request, txtParams);

            string reqUrl = webUtils.BuildGetUrl(this.serverUrl, txtParams);
            try
            {
                string body;
                if (request is IAliyunUploadRequest<T>) // 是否需要上传文件
                {
                    IAliyunUploadRequest<T> uRequest = (IAliyunUploadRequest<T>)request;
                    IDictionary<string, FileItem> fileParams = TopUtils.CleanupDictionary(uRequest.GetFileParameters());
                    body = webUtils.DoPost(this.serverUrl.TrimEnd('/'), txtParams, fileParams);
                }
                else
                {
                    body = webUtils.DoPost(this.serverUrl.TrimEnd('/'), txtParams);
                }

                // 解释响应结果
                T rsp;
                if (disableParser)
                {
                    rsp = Activator.CreateInstance<T>();
                    rsp.Body = body;
                }
                else
                {
                    if (FORMAT_XML.Equals(format))
                    {
                        IAliyunParser tp = new AliyunXmlParser();
                        rsp = tp.Parse<T>(body);
                    }
                    else
                    {
                        IAliyunParser tp = new AliyunJsonParser();
                        rsp = tp.Parse<T>(body);
                    }
                }

                // 追踪错误的请求
                if (!disableTrace && rsp.IsError)
                {
                    StringBuilder sb = new StringBuilder(reqUrl).Append(" response error!\r\n").Append(rsp.Body);
                    topLogger.Warn(sb.ToString());
                }
                return rsp;
            }
            catch (Exception e)
            {
                if (!disableTrace)
                {
                    StringBuilder sb = new StringBuilder(reqUrl).Append(" request error!\r\n").Append(e.StackTrace);
                    topLogger.Error(sb.ToString());
                }
                throw e;
            }
        }

        private T CreateErrorResponse<T>(string errCode, string errMsg) where T : AliyunResponse
        {
            T rsp = Activator.CreateInstance<T>();
            rsp.Code = errCode;
            rsp.Message = errMsg;

            if (FORMAT_XML.Equals(format))
            {
                XmlDocument root = new XmlDocument();
                XmlElement bodyE = root.CreateElement("Error");
                XmlElement codeE = root.CreateElement("Code");
                codeE.InnerText = errCode;
                bodyE.AppendChild(codeE);
                XmlElement msgE = root.CreateElement("Message");
                msgE.InnerText = errMsg;
                bodyE.AppendChild(msgE);
                root.AppendChild(bodyE);
                rsp.Body = root.OuterXml;
            }
            else
            {
                IDictionary<string, object> errObj = new Dictionary<string, object>();
                errObj.Add("Code", errCode);
                errObj.Add("Message", errMsg);

                string body = JSON.ToJSON(errObj);
                rsp.Body = body;
            }
            return rsp;
        }

        private void AddCommonParams<T>(IAliyunRequest<T> request, TopDictionary parameters) where T : AliyunResponse
        {
            String[] strArray = request.GetApiName().Split('.');
            if (strArray.Length < 5)
            {
                throw new TopException("Wrong api name.");
            }
            String action = strArray[3];
            parameters.Add("Action", action);

            String version = strArray[4];

            parameters.Add("Version", version);
            parameters.Add("AccessKeyId", accessKeyId);
            parameters.Add("Timestamp", FormatIso8601Date(DateTime.Now));
            parameters.Add("SignatureMethod", "HMAC-SHA1");
            parameters.Add("SignatureVersion", "1.0");
            parameters.Add("SignatureNonce", Guid.NewGuid().ToString()); // 可以使用GUID作为SignatureNonce
            parameters.Add("Format", format);

            // 计算签名，并将签名结果加入请求参数中
            parameters.Add("Signature", ComputeSignature(parameters));
        }

        private String ComputeSignature(TopDictionary parameters)
        {

            const String SEPARATOR = "&";

            // 生成规范化请求字符串
            StringBuilder canonicalizedQueryString = new StringBuilder();

            var orderedParameters = SortDictionary(parameters);

            foreach (var p in orderedParameters)
            {
                canonicalizedQueryString.Append("&")
                    .Append(PercentEncode(p.Key)).Append("=")
                    .Append(PercentEncode(p.Value));
            }

            // 生成用于计算签名的字符串 stringToSign
            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append(httpMethod).Append(SEPARATOR);
            stringToSign.Append(PercentEncode("/")).Append(SEPARATOR);

            stringToSign.Append(PercentEncode(
                canonicalizedQueryString.ToString().Substring(1)));

            // 注意accessKeySecret后面要加入一个字符"&"
            String signature = CalculateSignature(accessKeySecret + "&",
                                                  stringToSign.ToString());
            return signature;
        }

        private static String FormatIso8601Date(DateTime date)
        {
            // 注意使用UTC时间
            return date.ToUniversalTime().ToString(ISO8601_DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US"));
        }

        private static String CalculateSignature(String key, String stringToSign)
        {
            // 使用HmacSHA1算法计算HMAC值
            using (var algorithm = KeyedHashAlgorithm.Create("HMACSHA1"))
            {
                algorithm.Key = Encoding.GetEncoding(ENCODING_UTF8).GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(
                        Encoding.GetEncoding(ENCODING_UTF8).GetBytes(stringToSign.ToCharArray())));
            }
        }

        private static string PercentEncode(String value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding(ENCODING_UTF8).GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(
                        string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        private static Dictionary<string, string> SortDictionary(Dictionary<string, string> dic)
        {
            ArrayList arrayList = new ArrayList(dic.Keys);
            arrayList.Sort(StringComparer.Ordinal);
            Dictionary<string, string> sortedDictionary = new Dictionary<string, string>();
            foreach (string key in arrayList)
            {
                sortedDictionary.Add(key, dic[key]);
            }
            return sortedDictionary;
        }
    }
}
