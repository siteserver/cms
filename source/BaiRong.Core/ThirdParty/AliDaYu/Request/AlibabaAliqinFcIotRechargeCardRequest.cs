using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.aliqin.fc.iot.rechargeCard
    /// </summary>
    public class AlibabaAliqinFcIotRechargeCardRequest : BaseTopRequest<Top.Api.Response.AlibabaAliqinFcIotRechargeCardResponse>
    {
        /// <summary>
        /// IMEI号
        /// </summary>
        public string BillReal { get; set; }

        /// <summary>
        /// 外部计费号类型：写‘IMEI’
        /// </summary>
        public string BillSource { get; set; }

        /// <summary>
        /// 生效时间,1,立即生效; 2,次月生效
        /// </summary>
        public Nullable<long> EffCode { get; set; }

        /// <summary>
        /// ICCID
        /// </summary>
        public string Iccid { get; set; }

        /// <summary>
        /// 流量包offerId
        /// </summary>
        public string OfferId { get; set; }

        /// <summary>
        /// 外部id,用来做幂等
        /// </summary>
        public string OutRechargeId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.aliqin.fc.iot.rechargeCard";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("bill_real", this.BillReal);
            parameters.Add("bill_source", this.BillSource);
            parameters.Add("eff_code", this.EffCode);
            parameters.Add("iccid", this.Iccid);
            parameters.Add("offer_id", this.OfferId);
            parameters.Add("out_recharge_id", this.OutRechargeId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("bill_real", this.BillReal);
            RequestValidator.ValidateRequired("bill_source", this.BillSource);
            RequestValidator.ValidateRequired("eff_code", this.EffCode);
            RequestValidator.ValidateRequired("iccid", this.Iccid);
            RequestValidator.ValidateRequired("offer_id", this.OfferId);
            RequestValidator.ValidateRequired("out_recharge_id", this.OutRechargeId);
            RequestValidator.ValidateMaxLength("out_recharge_id", this.OutRechargeId, 32);
        }

        #endregion
    }
}
