using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.operator.mobile.bind
    /// </summary>
    public class AlipayOperatorMobileBindRequest : IAopRequest<AlipayOperatorMobileBindResponse>
    {
        /// <summary>
        /// 标识该运营商是否需要验证用户的手机号绑定过快捷卡  1：需要  0：不需要
        /// </summary>
        public string CheckSigncard { get; set; }

        /// <summary>
        /// 支付宝处理完请求后，如验证失败，当前页面自动跳转到商户网站里指定页面的http路径。
        /// </summary>
        public string FReturnUrl { get; set; }

        /// <summary>
        /// 标识该运营商是否提供了查询手机归属的spi接口。  1：提供了  0：没提供
        /// </summary>
        public string HasSpi { get; set; }

        /// <summary>
        /// 标识该运营商名称
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 标识该运营商所在省份
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 支付宝处理完请求后，如验证成功，当前页面自动跳转到商户网站里指定页面的http路径。
        /// </summary>
        public string SReturnUrl { get; set; }

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
            return "alipay.operator.mobile.bind";
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
            parameters.Add("check_signcard", this.CheckSigncard);
            parameters.Add("f_return_url", this.FReturnUrl);
            parameters.Add("has_spi", this.HasSpi);
            parameters.Add("operator_name", this.OperatorName);
            parameters.Add("province_name", this.ProvinceName);
            parameters.Add("s_return_url", this.SReturnUrl);
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
