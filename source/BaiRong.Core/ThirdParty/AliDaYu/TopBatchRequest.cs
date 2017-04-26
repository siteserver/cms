using System.Collections.Generic;

namespace Top.Api
{
    /// <summary>
    /// 批量API请求包装类。
    /// </summary>
    public class TopBatchRequest : BaseTopRequest<TopBatchResponse>
    {
        /// <summary>
        /// 公共方法
        /// </summary>
        public string PublicMethod { get; set; }

        /// <summary>
        /// 公共参数
        /// </summary>
        public TopDictionary PublicParams { get; set; }

        public List<ITopRequest<TopResponse>> RequestList { get; set; }

        public void AddPublicParam(string key, string value)
        {
            if (this.PublicParams == null)
            {
                this.PublicParams = new TopDictionary();
            }
            this.PublicParams.Add(key, value);
        }

        public TopBatchRequest AddRequest<T>(ITopRequest<T> request) where T : TopResponse
        {
            if (this.RequestList == null)
            {
                this.RequestList = new List<ITopRequest<TopResponse>>();
            }
            this.RequestList.Add(request);
            return this;
        }


        public override string GetApiName()
        {
            return null;
        }

        public override void Validate()
        {
        }

        public override IDictionary<string, string> GetParameters()
        {
            return null;
        }
    }
}
