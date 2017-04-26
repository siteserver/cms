using System.Collections.Generic;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.httpdns.get
    /// </summary>
    public class HttpdnsGetRequest : BaseTopRequest<Top.Api.Response.HttpdnsGetResponse>
    {
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.httpdns.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
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
