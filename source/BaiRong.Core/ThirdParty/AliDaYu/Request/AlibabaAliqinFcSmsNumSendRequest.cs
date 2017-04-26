using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.aliqin.fc.sms.num.send
    /// </summary>
    public class AlibabaAliqinFcSmsNumSendRequest : BaseTopRequest<Top.Api.Response.AlibabaAliqinFcSmsNumSendResponse>
    {
        /// <summary>
        /// 公共回传参数，在“消息返回”中会透传回该参数；举例：用户可以传入自己下级的会员ID，在消息返回时，该会员ID会包含在内，用户可以根据该会员ID识别是哪位会员使用了你的应用
        /// </summary>
        public string Extend { get; set; }

        /// <summary>
        /// 短信接收号码。支持单个或多个手机号码，传入号码为11位手机号码，不能加0或+86。群发短信需传入多个号码，以英文逗号分隔，一次调用最多传入200个号码。示例：18600000000,13911111111,13322222222
        /// </summary>
        public string RecNum { get; set; }

        /// <summary>
        /// 短信签名，传入的短信签名必须是在阿里大于“管理中心-短信签名管理”中的可用签名。如“阿里大于”已在短信签名管理中通过审核，则可传入”阿里大于“（传参时去掉引号）作为短信签名。短信效果示例：【阿里大于】欢迎使用阿里大于服务。
        /// </summary>
        public string SmsFreeSignName { get; set; }

        /// <summary>
        /// 短信模板变量，传参规则{"key":"value"}，key的名字须和申请模板中的变量名一致，多个变量之间以逗号隔开。示例：针对模板“验证码${code}，您正在进行${product}身份验证，打死不要告诉别人哦！”，传参时需传入{"code":"1234","product":"alidayu"}
        /// </summary>
        public string SmsParam { get; set; }

        /// <summary>
        /// 短信模板ID，传入的模板必须是在阿里大于“管理中心-短信模板管理”中的可用模板。示例：SMS_585014
        /// </summary>
        public string SmsTemplateCode { get; set; }

        /// <summary>
        /// 短信类型，传入值请填写normal
        /// </summary>
        public string SmsType { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.aliqin.fc.sms.num.send";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("extend", this.Extend);
            parameters.Add("rec_num", this.RecNum);
            parameters.Add("sms_free_sign_name", this.SmsFreeSignName);
            parameters.Add("sms_param", this.SmsParam);
            parameters.Add("sms_template_code", this.SmsTemplateCode);
            parameters.Add("sms_type", this.SmsType);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("rec_num", this.RecNum);
            RequestValidator.ValidateRequired("sms_free_sign_name", this.SmsFreeSignName);
            RequestValidator.ValidateRequired("sms_template_code", this.SmsTemplateCode);
            RequestValidator.ValidateRequired("sms_type", this.SmsType);
        }

        #endregion
    }
}
