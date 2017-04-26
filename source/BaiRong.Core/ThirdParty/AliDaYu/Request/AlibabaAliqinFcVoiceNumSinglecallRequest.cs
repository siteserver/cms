using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.aliqin.fc.voice.num.singlecall
    /// </summary>
    public class AlibabaAliqinFcVoiceNumSinglecallRequest : BaseTopRequest<Top.Api.Response.AlibabaAliqinFcVoiceNumSinglecallResponse>
    {
        /// <summary>
        /// 被叫号码，支持国内手机号与固话号码,格式如下057188773344,13911112222,4001112222,95500
        /// </summary>
        public string CalledNum { get; set; }

        /// <summary>
        /// 被叫号显，传入的显示号码必须是阿里大于“管理中心-号码管理”中申请通过的号码
        /// </summary>
        public string CalledShowNum { get; set; }

        /// <summary>
        /// 公共回传参数，在“消息返回”中会透传回该参数；举例：用户可以传入自己下级的会员ID，在消息返回时，该会员ID会包含在内，用户可以根据该会员ID识别是哪位会员使用了你的应用
        /// </summary>
        public string Extend { get; set; }

        /// <summary>
        /// 语音文件ID，传入的语音文件必须是在阿里大于“管理中心-语音文件管理”中的可用语音文件
        /// </summary>
        public string VoiceCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.aliqin.fc.voice.num.singlecall";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("called_num", this.CalledNum);
            parameters.Add("called_show_num", this.CalledShowNum);
            parameters.Add("extend", this.Extend);
            parameters.Add("voice_code", this.VoiceCode);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("called_num", this.CalledNum);
            RequestValidator.ValidateRequired("called_show_num", this.CalledShowNum);
            RequestValidator.ValidateRequired("voice_code", this.VoiceCode);
        }

        #endregion
    }
}
