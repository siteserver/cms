using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.aliqin.fc.flow.charge
    /// </summary>
    public class AlibabaAliqinFcFlowChargeRequest : BaseTopRequest<Top.Api.Response.AlibabaAliqinFcFlowChargeResponse>
    {
        /// <summary>
        /// 需要充值的流量
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 唯一流水号
        /// </summary>
        public string OutRechargeId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 充值原因
        /// </summary>
        public string Reason { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.aliqin.fc.flow.charge";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("grade", this.Grade);
            parameters.Add("out_recharge_id", this.OutRechargeId);
            parameters.Add("phone_num", this.PhoneNum);
            parameters.Add("reason", this.Reason);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("grade", this.Grade);
            RequestValidator.ValidateRequired("out_recharge_id", this.OutRechargeId);
            RequestValidator.ValidateRequired("phone_num", this.PhoneNum);
        }

        #endregion
    }
}
