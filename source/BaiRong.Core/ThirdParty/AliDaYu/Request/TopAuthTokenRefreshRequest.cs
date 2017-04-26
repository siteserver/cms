using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.top.auth.token.refresh
    /// </summary>
    public class TopAuthTokenRefreshRequest : BaseTopRequest<Top.Api.Response.TopAuthTokenRefreshResponse>
    {
        /// <summary>
        /// grantType==refresh_token 时需要
        /// </summary>
        public string RefreshToken { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.top.auth.token.refresh";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("refresh_token", this.RefreshToken);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("refresh_token", this.RefreshToken);
        }

        #endregion
    }
}
