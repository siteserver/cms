using System.Collections.Generic;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.appip.get
    /// </summary>
    public class AppipGetRequest : BaseTopRequest<Top.Api.Response.AppipGetResponse>
    {
        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.appip.get";
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
