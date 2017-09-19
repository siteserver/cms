using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.ecapiprod.data.put
    /// </summary>
    public class AlipayEcapiprodDataPutRequest : IAopRequest<AlipayEcapiprodDataPutResponse>
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 数据字符编码，默认UTF-8
        /// </summary>
        public string CharSet { get; set; }

        /// <summary>
        /// 数据采集平台生成的采集任务编号
        /// </summary>
        public string CollectingTaskId { get; set; }

        /// <summary>
        /// 身份证，工商注册号等
        /// </summary>
        public string EntityCode { get; set; }

        /// <summary>
        /// 姓名或公司名等，name和code不能同时为空
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 人或公司等
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// 渠道商
        /// </summary>
        public string IsvCode { get; set; }

        /// <summary>
        /// 数据主体,以json格式传输的数据
        /// </summary>
        public string JsonData { get; set; }

        /// <summary>
        /// 数据合作方
        /// </summary>
        public string OrgCode { get; set; }

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
            return "alipay.ecapiprod.data.put";
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
            parameters.Add("category", this.Category);
            parameters.Add("char_set", this.CharSet);
            parameters.Add("collecting_task_id", this.CollectingTaskId);
            parameters.Add("entity_code", this.EntityCode);
            parameters.Add("entity_name", this.EntityName);
            parameters.Add("entity_type", this.EntityType);
            parameters.Add("isv_code", this.IsvCode);
            parameters.Add("json_data", this.JsonData);
            parameters.Add("org_code", this.OrgCode);
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
