using System;
using System.Collections.Generic;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.top.secret.register
    /// </summary>
    public class TopSecretRegisterRequest : BaseTopRequest<Top.Api.Response.TopSecretRegisterResponse>
    {
        /// <summary>
        /// 用户id，保证唯一
        /// </summary>
        public Nullable<long> UserId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.top.secret.register";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("user_id", this.UserId);
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
