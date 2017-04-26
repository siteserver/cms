using System.Collections.Generic;

namespace Top.Api
{
    /// <summary>
    /// 批量API响应类。
    /// </summary>
    public class TopBatchResponse : TopResponse
    {
        /// <summary>
        /// 当批量API请求成功后，或获取所有API的响应结果。
        /// </summary>
        public List<TopResponse> ResponseList { get; set; }

        public TopBatchResponse() { }

        public TopBatchResponse(string errorCode, string errorMessage)
        {
            base.ErrCode = errorCode;
            base.ErrMsg = ErrMsg;
        }

        /// <summary>
        /// 根据指定的API请求获取相应的API响应结果。
        /// </summary>
        public T GetResponse<T>(ITopRequest<T> request) where T : TopResponse
        {
            if (this.ResponseList == null || this.ResponseList.Count == 0)
            {
                return null;
            }
            return this.ResponseList[request.GetBatchApiOrder()] as T;
        }

        public void AddResponse(TopResponse response)
        {
            if (this.ResponseList == null)
            {
                this.ResponseList = new List<TopResponse>();
            }
            this.ResponseList.Add(response);
        }
    }
}
