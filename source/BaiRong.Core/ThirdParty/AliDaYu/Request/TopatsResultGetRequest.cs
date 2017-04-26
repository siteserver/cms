using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.topats.result.get
    /// </summary>
    public class TopatsResultGetRequest : BaseTopRequest<Top.Api.Response.TopatsResultGetResponse>
    {
        /// <summary>
        /// 任务ID，创建任务时返回的task_id
        /// </summary>
        public Nullable<long> TaskId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.topats.result.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("task_id", this.TaskId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("task_id", this.TaskId);
        }

        #endregion
    }
}
