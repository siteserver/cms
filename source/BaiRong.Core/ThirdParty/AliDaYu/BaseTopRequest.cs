using System.Collections.Generic;

namespace Top.Api
{
    /// <summary>
    /// 基础TOP请求类，存放一些通用的请求参数。
    /// </summary>
    public abstract class BaseTopRequest<T> : ITopRequest<T> where T : TopResponse
    {
        /// <summary>
        /// HTTP请求URL参数
        /// </summary>
        internal TopDictionary otherParams;
        /// <summary>
        /// HTTP请求头参数
        /// </summary>
        private TopDictionary headerParams;
        /// <summary>
        /// 请求目标AppKey
        /// </summary>
        private string targetAppKey;

        /// <summary>
        /// 批量API请求的用户授权码
        /// </summary>
        private string batchApiSession;

        /// <summary>
        /// API在批量调用中的顺序
        /// </summary>
        private int batchApiOrder;

        public void AddOtherParameter(string key, string value)
        {
            if (this.otherParams == null)
            {
                this.otherParams = new TopDictionary();
            }
            this.otherParams.Add(key, value);
        }

        public void AddHeaderParameter(string key, string value)
        {
            GetHeaderParameters().Add(key, value);
        }

        public IDictionary<string, string> GetHeaderParameters()
        {
            if (this.headerParams == null)
            {
                this.headerParams = new TopDictionary();
            }
            return this.headerParams;
        }

        public string GetTargetAppKey()
        {
            return this.targetAppKey;
        }

        public void SetTargetAppKey(string targetAppKey)
        {
            this.targetAppKey = targetAppKey;
        }

        public string GetBatchApiSession()
        {
            return this.batchApiSession;
        }

        public void SetBatchApiSession(string session)
        {
            this.batchApiSession = session;
        }

        public int GetBatchApiOrder()
        {
            return this.batchApiOrder;
        }

        public void SetBatchApiOrder(int order)
        {
            this.batchApiOrder = order;
        }

        public abstract string GetApiName();

        public abstract void Validate();

        public abstract IDictionary<string, string> GetParameters();
    }
}
