using System.Collections.Generic;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.aliqin.fc.flow.query
    /// </summary>
    public class AlibabaAliqinFcFlowQueryRequest : BaseTopRequest<Top.Api.Response.AlibabaAliqinFcFlowQueryResponse>
    {
        /// <summary>
        /// 唯一流水号
        /// </summary>
        public string OutId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.aliqin.fc.flow.query";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("out_id", this.OutId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

        #endregion
    }
}
