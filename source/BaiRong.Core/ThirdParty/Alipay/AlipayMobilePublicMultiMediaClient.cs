using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aop.Api.Parser;
using Aop.Api.Request;
using Aop.Api.Response;
using Aop.Api.Util;
using System.Net;

namespace Aop.Api
{
    public class AlipayMobilePublicMultiMediaClient : IAopClient
    {

        public const string APP_ID = "app_id";
        public const string FORMAT = "format";
        public const string METHOD = "method";
        public const string TIMESTAMP = "timestamp";
        public const string VERSION = "version";
        public const string SIGN_TYPE = "sign_type";
        public const string ACCESS_TOKEN = "auth_token";
        public const string SIGN = "sign";
        public const string TERMINAL_TYPE = "terminal_type";
        public const string TERMINAL_INFO = "terminal_info";
        public const string PROD_CODE = "prod_code";
        public const string APP_AUTH_TOKEN = "app_auth_token";

        private string version;
        private string format;
        private string serverUrl;
        private string appId;
        private string privateKeyPem;
        private string signType = "RSA";
        private string charset;

        private WebUtils webUtils;

        public string Version
        {
            get { return version != null ? version : "1.0"; }
            set { version = value; }
        }

        public string Format
        {
            get { return format != null ? format : "json"; }
            set { format = value; }
        }

        #region DefaultAopClient Constructors

        public AlipayMobilePublicMultiMediaClient(string serverUrl, string appId, string privateKeyPem)
        {
            this.appId = appId;
            this.privateKeyPem = privateKeyPem;
            this.serverUrl = serverUrl;
            this.webUtils = new WebUtils();
        }

        public AlipayMobilePublicMultiMediaClient(string serverUrl, string appId, string privateKeyPem, string format)
            : this(serverUrl, appId, privateKeyPem)
        {
            this.format = format;
        }

        public AlipayMobilePublicMultiMediaClient(string serverUrl, string appId, string privateKeyPem, string format, string charset)
            : this(serverUrl, appId, privateKeyPem, format)
        {
            this.charset = charset;
        }

        public AlipayMobilePublicMultiMediaClient(string serverUrl, string appId, string privateKeyPem, string format, string version, string signType)
            : this(serverUrl, appId, privateKeyPem, format)
        {
            this.version = version;
            this.signType = signType;
        }

        public void SetTimeout(int timeout)
        {
            webUtils.Timeout = timeout;
        }

        #endregion

        #region IAopClient Members



        public T Execute<T>(IAopRequest<T> request) where T : AopResponse
        {
            return Execute<T>(request, null);
        }

        public T Execute<T>(IAopRequest<T> request, string accessToken) where T : AopResponse
        {

            return Execute<T>(request, accessToken, null);
        }

        public T Execute<T>(IAopRequest<T> request, string accessToken, string appAuthToken) where T : AopResponse
        {


            AlipayMobilePublicMultiMediaDownloadRequest multiMediaDownloadRequest = ((AlipayMobilePublicMultiMediaDownloadRequest)request);
            // 添加协议级请求参数
            AopDictionary txtParams = new AopDictionary(request.GetParameters());
            txtParams.Add(METHOD, request.GetApiName());
            txtParams.Add(VERSION, Version);
            txtParams.Add(APP_ID, appId);
            txtParams.Add(FORMAT, format);
            txtParams.Add(TIMESTAMP, DateTime.Now);
            txtParams.Add(ACCESS_TOKEN, accessToken);
            txtParams.Add(SIGN_TYPE, signType);
            txtParams.Add(TERMINAL_TYPE, request.GetTerminalType());
            txtParams.Add(TERMINAL_INFO, request.GetTerminalInfo());
            txtParams.Add(PROD_CODE, request.GetProdCode());

            if (!string.IsNullOrEmpty(appAuthToken))
            {
                txtParams.Add(APP_AUTH_TOKEN, appAuthToken);
            }


            // 添加签名参数
            txtParams.Add(SIGN, AopUtils.SignAopRequest(txtParams, privateKeyPem, charset,signType));

            Stream outStream = multiMediaDownloadRequest.stream;
            AopResponse rsp = DoGet(txtParams, outStream);

            return (T)rsp;

        }

        #endregion

        private AopResponse DoGet(AopDictionary parameters, Stream outStream)
        {
            AlipayMobilePublicMultiMediaDownloadResponse response = null;

            string url = this.serverUrl;
            System.Console.WriteLine(url);
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + WebUtils.BuildQuery(parameters,charset);
                }
                else
                {
                    url = url + "?" + WebUtils.BuildQuery(parameters,charset);
                }
            }

            HttpWebRequest req = webUtils.GetWebRequest(url, "GET");
            req.ContentType = "application/x-www-form-urlencoded;charset=" + charset;

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                if (rsp.ContentType.ToLower().Contains("text/plain"))
                {
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    string body = webUtils.GetResponseAsString(rsp,encoding);
                    IAopParser<AlipayMobilePublicMultiMediaDownloadResponse> tp = new AopJsonParser<AlipayMobilePublicMultiMediaDownloadResponse>();
                    response = tp.Parse(body, charset);
                }else{
                    GetResponseAsStream(outStream, rsp);
                    response = new AlipayMobilePublicMultiMediaDownloadResponse();
                }
            }
            return response;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public void GetResponseAsStream(Stream outStream, HttpWebResponse rsp)
        {
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;
            BinaryWriter writer = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream);

                writer = new BinaryWriter(outStream);

                //stream.CopyTo(outStream);
                int length = Convert.ToInt32(rsp.ContentLength);
                byte[] buffer = new byte[length];
                int rc = 0; 
                while ((rc=stream.Read(buffer, 0, length)) > 0)
                {
                    outStream.Write(buffer, 0, rc);
                }
                outStream.Flush();
                outStream.Close();
                
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        public T pageExecute<T>(IAopRequest<T> request) where T : AopResponse
        {
            throw new NotImplementedException();
        }

        public T pageExecute<T>(IAopRequest<T> request, string session, string reqMethod) where T : AopResponse
        {
            throw new NotImplementedException();
        }

        public T SdkExecute<T>(IAopRequest<T> request) where T : AopResponse
        {
            throw new NotImplementedException();
        }
    }
}
