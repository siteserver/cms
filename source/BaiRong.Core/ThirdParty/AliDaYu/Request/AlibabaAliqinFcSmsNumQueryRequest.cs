using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.aliqin.fc.sms.num.query
    /// </summary>
    public class AlibabaAliqinFcSmsNumQueryRequest : BaseTopRequest<Top.Api.Response.AlibabaAliqinFcSmsNumQueryResponse>
    {
        /// <summary>
        /// 短信发送流水
        /// </summary>
        public string BizId { get; set; }

        /// <summary>
        /// 分页参数,页码
        /// </summary>
        public Nullable<long> CurrentPage { get; set; }

        /// <summary>
        /// 分页参数，每页数量。最大值50
        /// </summary>
        public Nullable<long> PageSize { get; set; }

        /// <summary>
        /// 短信发送日期，支持近30天记录查询，格式yyyyMMdd
        /// </summary>
        public string QueryDate { get; set; }

        /// <summary>
        /// 短信接收号码
        /// </summary>
        public string RecNum { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.aliqin.fc.sms.num.query";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_id", this.BizId);
            parameters.Add("current_page", this.CurrentPage);
            parameters.Add("page_size", this.PageSize);
            parameters.Add("query_date", this.QueryDate);
            parameters.Add("rec_num", this.RecNum);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("current_page", this.CurrentPage);
            RequestValidator.ValidateRequired("page_size", this.PageSize);
            RequestValidator.ValidateRequired("query_date", this.QueryDate);
            RequestValidator.ValidateRequired("rec_num", this.RecNum);
        }

        #endregion
    }
}
