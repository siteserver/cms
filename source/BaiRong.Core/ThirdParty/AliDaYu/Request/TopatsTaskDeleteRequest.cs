using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.topats.task.delete
    /// </summary>
    public class TopatsTaskDeleteRequest : BaseTopRequest<Top.Api.Response.TopatsTaskDeleteResponse>
    {
        /// <summary>
        /// 需要取消的任务ID
        /// </summary>
        public Nullable<long> TaskId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.topats.task.delete";
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
