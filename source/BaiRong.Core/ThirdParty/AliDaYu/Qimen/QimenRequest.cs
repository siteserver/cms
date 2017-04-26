using System;
using Top.Api;

namespace Qimen.Api
{
    public abstract class QimenRequest<T> where T : QimenResponse
    {
        /// <summary>
        /// 自定义URL参数
        /// </summary>
        private TopDictionary queryParameters;

        public TopDictionary GetQueryParameters()
        {
            return this.queryParameters;
        }

        public void AddQueryParameter(string key, string value)
        {
            if (this.queryParameters == null)
            {
                this.queryParameters = new TopDictionary();
            }
            this.queryParameters[key] = value;
        }

        private TopDictionary headerParameters;

        public TopDictionary GetHeaderParameters()
        {
            return this.headerParameters;
        }

        public void AddHeaderParameter(string key, string value)
        {
            if (this.headerParameters == null)
            {
                this.headerParameters = new TopDictionary();
            }
            this.headerParameters[key] = value;
        }

        /// <summary>
        /// 客户ID号
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// 请求时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        private string _version = "1.0";
        /// <summary>
        /// API版本号
        /// </summary>
        public string Version { get { return this._version; } set { this._version = value; } }

        /// <summary>
        /// 测试类型
        /// </summary>
        public string TestType { get; set; }

        /// <summary>
        /// 请求body体
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 获取API名称
        /// </summary>
        /// <returns></returns>
        public abstract string GetApiName();
    }
}
