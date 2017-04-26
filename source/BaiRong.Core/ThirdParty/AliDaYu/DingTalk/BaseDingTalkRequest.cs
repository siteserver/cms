using System.Collections.Generic;
using Top.Api;

namespace DingTalk.Api
{
    /// <summary>
    /// 基础TOP请求类，存放一些通用的请求参数。
    /// </summary>
    public abstract class BaseDingTalkRequest<T> : IDingTalkRequest<T> where T : DingTalkResponse
    {
        /// <summary>
        /// HTTP请求URL参数
        /// </summary>
        internal TopDictionary otherParams;
        /// <summary>
        /// HTTP请求头参数
        /// </summary>
        private TopDictionary headerParams;

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

        public abstract string GetApiName();

        public abstract void Validate();

        public abstract IDictionary<string, string> GetParameters();
    }
}
