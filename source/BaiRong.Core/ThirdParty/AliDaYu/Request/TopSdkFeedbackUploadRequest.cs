using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.top.sdk.feedback.upload
    /// </summary>
    public class TopSdkFeedbackUploadRequest : BaseTopRequest<Top.Api.Response.TopSdkFeedbackUploadResponse>
    {
        /// <summary>
        /// 具体内容，json形式
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 1、回传加密信息
        /// </summary>
        public string Type { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.top.sdk.feedback.upload";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("content", this.Content);
            parameters.Add("type", this.Type);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("type", this.Type);
        }

        #endregion
    }
}
