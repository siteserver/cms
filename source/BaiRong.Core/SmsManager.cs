using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Top.Api;
using Top.Api.Request;

namespace BaiRong.Core
{
    public class SmsManager
    {
        public static bool IsSmsReady()
        {
            if (ConfigManager.SystemConfigInfo.IsSmsAliDaYu && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuAppKey) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuAppSecret) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuSignName))
            {
                return true;
            }
            return ConfigManager.SystemConfigInfo.IsSmsYunPian && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsYunPianApiKey);
        }

        public static bool SendCode(string mobile, int code, out string errorMessage)
        {
            if (string.IsNullOrEmpty(mobile) || !StringUtils.IsMobile(mobile))
            {
                errorMessage = "手机号码格式不正确";
                return false;
            }

            errorMessage = string.Empty;
            var isSuccess = false;

            if (ConfigManager.SystemConfigInfo.IsSmsAliDaYu && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuAppKey) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuAppSecret) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuSignName) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsAliDaYuCodeTplId))
            {
                isSuccess = SendCodeByAliDaYu(mobile, code, out errorMessage);
            }
            if (!isSuccess && ConfigManager.SystemConfigInfo.IsSmsYunPian && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsYunPianApiKey) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.SmsYunPianCodeTplId))
            {
                isSuccess = SendCodeByYunPian(mobile, code, out errorMessage);
            }

            if (!isSuccess && string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "后台短信发送功能暂时无法使用，请联系管理员或稍后再试";
            }

            return isSuccess;
        }

        public static bool SendCodeByAliDaYu(string mobile, int code, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                ITopClient client = new DefaultTopClient("http://gw.api.taobao.com/router/rest", ConfigManager.SystemConfigInfo.SmsAliDaYuAppKey, ConfigManager.SystemConfigInfo.SmsAliDaYuAppSecret);
                var req = new AlibabaAliqinFcSmsNumSendRequest
                {
                    SmsType = "normal",
                    SmsFreeSignName = ConfigManager.SystemConfigInfo.SmsAliDaYuSignName,
                    SmsParam = "{code:'" + code + "'}",
                    RecNum = mobile,
                    SmsTemplateCode = ConfigManager.SystemConfigInfo.SmsAliDaYuCodeTplId
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

        public static bool SendCodeByYunPian(string mobile, int code, out string errorMessage)
        {
            mobile = HttpUtility.UrlEncode(mobile, Encoding.UTF8);
            var tplValue = HttpUtility.UrlEncode(
                HttpUtility.UrlEncode("#code#", Encoding.UTF8) + "=" +
                HttpUtility.UrlEncode(code.ToString(), Encoding.UTF8)
            , Encoding.UTF8);

            return HttpPostToYunPian("https://sms.yunpian.com/v1/sms/tpl_send.json", "apikey=" + ConfigManager.SystemConfigInfo.SmsYunPianApiKey + "&mobile=" + mobile + "&tpl_id=" + ConfigManager.SystemConfigInfo.SmsYunPianCodeTplId + "&tpl_value=" + tplValue, out errorMessage);
        }

        private static bool HttpPostToYunPian(string url, string data, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                byte[] dataArray = Encoding.UTF8.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = dataArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(dataArray, 0, dataArray.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
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