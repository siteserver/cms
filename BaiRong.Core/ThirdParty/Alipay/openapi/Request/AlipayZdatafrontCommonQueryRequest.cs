using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.zdatafront.common.query
    /// </summary>
    public class AlipayZdatafrontCommonQueryRequest : IAopRequest<AlipayZdatafrontCommonQueryResponse>
    {
        /// <summary>
        /// 如果cacheInterval<=0,就直接从外部获取数据；  如果cacheInterval>0,就先判断cache中的数据是否过期，如果没有过期就返回cache中的数据，如果过期再从外部获取数据并刷新cache，然后返回数据。  单位：秒
        /// </summary>
        public Nullable<long> CacheInterval { get; set; }

        /// <summary>
        /// 通用查询的入参
        /// </summary>
        public string QueryConditions { get; set; }

        /// <summary>
        /// 服务名称请与相关开发负责人联系
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 访问该服务的业务
        /// </summary>
        public string VisitBiz { get; set; }

        /// <summary>
        /// 访问该服务的业务线
        /// </summary>
        public string VisitBizLine { get; set; }

        /// <summary>
        /// 访问该服务的部门名称
        /// </summary>
        public string VisitDomain { get; set; }

        #region IAopRequest Members
		private bool  needEncrypt=false;
        private string apiVersion = "1.0";
		private string terminalType;
		private string terminalInfo;
        private string prodCode;
		private string notifyUrl;
        private string returnUrl;
		private AopObject bizModel;

		public void SetNeedEncrypt(bool needEncrypt){
             this.needEncrypt=needEncrypt;
        }

        public bool GetNeedEncrypt(){

            return this.needEncrypt;
        }

		public void SetNotifyUrl(string notifyUrl){
            this.notifyUrl = notifyUrl;
        }

        public string GetNotifyUrl(){
            return this.notifyUrl;
        }

        public void SetReturnUrl(string returnUrl){
            this.returnUrl = returnUrl;
        }

        public string GetReturnUrl(){
            return this.returnUrl;
        }

        public void SetTerminalType(String terminalType){
			this.terminalType=terminalType;
		}

    	public string GetTerminalType(){
    		return this.terminalType;
    	}

    	public void SetTerminalInfo(String terminalInfo){
    		this.terminalInfo=terminalInfo;
    	}

    	public string GetTerminalInfo(){
    		return this.terminalInfo;
    	}

        public void SetProdCode(String prodCode){
            this.prodCode=prodCode;
        }

        public string GetProdCode(){
            return this.prodCode;
        }

        public string GetApiName()
        {
            return "alipay.zdatafront.common.query";
        }

        public void SetApiVersion(string apiVersion){
            this.apiVersion=apiVersion;
        }

        public string GetApiVersion(){
            return this.apiVersion;
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("cache_interval", this.CacheInterval);
            parameters.Add("query_conditions", this.QueryConditions);
            parameters.Add("service_name", this.ServiceName);
            parameters.Add("visit_biz", this.VisitBiz);
            parameters.Add("visit_biz_line", this.VisitBizLine);
            parameters.Add("visit_domain", this.VisitDomain);
            return parameters;
        }

		public AopObject GetBizModel()
        {
            return this.bizModel;
        }

        public void SetBizModel(AopObject bizModel)
        {
            this.bizModel = bizModel;
        }

        #endregion
    }
}
