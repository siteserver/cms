using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Report
{
    public class TopSdkFeedbackUploadRequest : BaseTopRequest<TopSdkFeedbackUploadResponse>
    {

        public string Type { get; set; }

        public string Content { get; set; }

        public override string GetApiName()
        {
            return "taobao.top.sdk.feedback.upload";
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("Type", Type);
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("type", this.Type);
            parameters.Add("content", this.Content);
            return parameters;
        }
    }
}
