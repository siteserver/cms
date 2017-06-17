using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using BaiRong.Core.Model.Enumerations;
using Top.Api;
using Top.Api.Request;

namespace BaiRong.Core
{
    public class SmsManager
    {
        public static bool IsSmsReady()
        {
            if (ConfigManager.SystemConfigInfo.SmsProviderType == ESmsProviderType.Aliyun && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAppKey) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAppSecret) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsSignName))
            {
                return true;
            }
            if (ConfigManager.SystemConfigInfo.SmsProviderType == ESmsProviderType.Yunpian && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAppKey))
            {
                return true;
            }
            return false;
        }

        public static bool SendNotify(string mobile, string tplId, NameValueCollection parameters, out string errorMessage)
        {
            if (string.IsNullOrEmpty(mobile) || !StringUtils.IsMobile(mobile))
            {
                errorMessage = "手机号码格式不正确";
                return false;
            }

            errorMessage = string.Empty;
            var isSuccess = false;

            if (ConfigManager.SystemConfigInfo.SmsProviderType == ESmsProviderType.Aliyun)
            {
                isSuccess = SendByAliyun(mobile, tplId, parameters, out errorMessage);
            }
            else if (ConfigManager.SystemConfigInfo.SmsProviderType == ESmsProviderType.Yunpian)
            {
                isSuccess = SendByYunpian(mobile, tplId, parameters, out errorMessage);
            }

            if (!isSuccess && string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "后台短信发送功能暂时无法使用，请联系管理员或稍后再试";
            }

            return isSuccess;
        }

        public static bool SendVerify(string mobile, int code, string tplId, out string errorMessage)
        {
            var parameters = new NameValueCollection { { "code", code.ToString() } };
            return SendNotify(mobile, tplId, parameters, out errorMessage);
        }

        private static bool SendByAliyun(string mobile, string tplId, NameValueCollection parameters, out string errorMessage)
        {
            errorMessage = null;
            var param = new StringBuilder("{");
            foreach (string key in parameters.Keys)
            {
                var value = parameters[key];
                param.Append($"{key}:'{value}',");
            }
            param.Length--;
            param.Append("}");
            try
            {
                ITopClient client = new DefaultTopClient("http://gw.api.taobao.com/router/rest", ConfigManager.SystemConfigInfo.SmsAppKey, ConfigManager.SystemConfigInfo.SmsAppSecret);
                var req = new AlibabaAliqinFcSmsNumSendRequest
                {
                    SmsType = "normal",
                    SmsFreeSignName = ConfigManager.SystemConfigInfo.SmsSignName,
                    //SmsParam = "{code:'" + code + "'}",
                    SmsParam = param.ToString(),
                    RecNum = mobile,
                    SmsTemplateCode = tplId
                };

                var rsp = client.Execute(req);
                var retval = rsp.Body;
                if (!retval.Contains("error_response"))
                {
                    return true;
                }
                errorMessage = RegexUtils.GetInnerContent("msg", retval);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
            return false;
        }

        private static bool SendByYunpian(string mobile, string tplId, NameValueCollection parameters, out string errorMessage)
        {
            var param = new StringBuilder();
            foreach (string key in parameters.Keys)
            {
                var value = parameters[key];

                param.Append(HttpUtility.UrlEncode("#" + key + "#", Encoding.UTF8) + "=" +
                             HttpUtility.UrlEncode(value, Encoding.UTF8)).Append("&");
            }
            param.Length--;

            mobile = HttpUtility.UrlEncode(mobile, Encoding.UTF8);
            //var tplValue = HttpUtility.UrlEncode(
            //    HttpUtility.UrlEncode("#code#", Encoding.UTF8) + "=" +
            //    HttpUtility.UrlEncode(code.ToString(), Encoding.UTF8)
            //, Encoding.UTF8);
            var tplValue = HttpUtility.UrlEncode(param.ToString(), Encoding.UTF8);

            return HttpPostToYunpian("https://sms.yunpian.com/v1/sms/tpl_send.json", "apikey=" + ConfigManager.SystemConfigInfo.SmsAppKey + "&mobile=" + mobile + "&tpl_id=" + tplId + "&tpl_value=" + tplValue, out errorMessage);
        }

        private static bool HttpPostToYunpian(string url, string data, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                var dataArray = Encoding.UTF8.GetBytes(data);

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = dataArray.Length;
                var dataStream = request.GetRequestStream();
                dataStream.Write(dataArray, 0, dataArray.Length);
                dataStream.Close();

                var response = (HttpWebResponse)request.GetResponse();
                // ReSharper disable once AssignNullToNotNullAttribute
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                reader.ReadToEnd();
                reader.Close();
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
            return false;
        }

    }
}