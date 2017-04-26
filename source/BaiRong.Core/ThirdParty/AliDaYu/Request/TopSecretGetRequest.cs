using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.top.secret.get
    /// </summary>
    public class TopSecretGetRequest : BaseTopRequest<Top.Api.Response.TopSecretGetResponse>
    {
        /// <summary>
        /// 自定义用户id
        /// </summary>
        public Nullable<long> CustomerUserId { get; set; }

        /// <summary>
        /// 伪随机数
        /// </summary>
        public string RandomNum { get; set; }

        /// <summary>
        /// 秘钥版本号
        /// </summary>
        public Nullable<long> SecretVersion { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.top.secret.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("customer_user_id", this.CustomerUserId);
            parameters.Add("random_num", this.RandomNum);
            parameters.Add("secret_version", this.SecretVersion);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("random_num", this.RandomNum);
        }

        #endregion
    }
}
