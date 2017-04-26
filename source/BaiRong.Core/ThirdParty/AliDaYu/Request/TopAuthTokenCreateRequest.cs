using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.top.auth.token.create
    /// </summary>
    public class TopAuthTokenCreateRequest : BaseTopRequest<Top.Api.Response.TopAuthTokenCreateResponse>
    {
        /// <summary>
        /// 授权code，grantType==authorization_code 时需要
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 与生成code的uuid配对
        /// </summary>
        public string Uuid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.top.auth.token.create";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("code", this.Code);
            parameters.Add("uuid", this.Uuid);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("code", this.Code);
        }

        #endregion
    }
}
