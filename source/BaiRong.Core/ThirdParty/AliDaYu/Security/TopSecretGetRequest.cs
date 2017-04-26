using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Security
{

    public class TopSecretGetRequest : BaseTopRequest<TopSecretGetResponse>
    {
        /// <summary>
        /// 伪随机码
        /// </summary>
        public string RandomNum { get; set; }
        /// <summary>
        /// 秘钥版本
        /// </summary>
        public Nullable<Int64> SecretVersion { get; set; }
        /// <summary>
        /// 自主账号id
        /// </summary>
        public Nullable<Int64> CustomerUserId { get; set; }

        public override string GetApiName()
        {
            return "taobao.top.secret.get";
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("RandomNum", RandomNum);
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("random_num", this.RandomNum);
            parameters.Add("secret_version", this.SecretVersion);
            parameters.Add("customer_user_id", this.CustomerUserId);
            return parameters;
        }
    }
}
