using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.aliqin.fc.iot.qrycard
    /// </summary>
    public class AlibabaAliqinFcIotQrycardRequest : BaseTopRequest<Top.Api.Response.AlibabaAliqinFcIotQrycardResponse>
    {
        /// <summary>
        /// 外部计费号
        /// </summary>
        public string BillReal { get; set; }

        /// <summary>
        /// 外部计费来源
        /// </summary>
        public string BillSource { get; set; }

        /// <summary>
        /// ICCID
        /// </summary>
        public string Iccid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.aliqin.fc.iot.qrycard";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("bill_real", this.BillReal);
            parameters.Add("bill_source", this.BillSource);
            parameters.Add("iccid", this.Iccid);
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
            RequestValidator.ValidateRequired("iccid", this.Iccid);
        }

        #endregion
    }
}
