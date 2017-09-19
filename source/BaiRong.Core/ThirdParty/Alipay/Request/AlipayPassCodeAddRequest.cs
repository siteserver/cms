using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.pass.code.add
    /// </summary>
    public class AlipayPassCodeAddRequest : IAopRequest<AlipayPassCodeAddResponse>
    {
        /// <summary>
        /// alipass文件Base64编码后的内容。
        /// </summary>
        public string FileContent { get; set; }

        /// <summary>
        /// 识别信息  当 recognition_type=1时， recognition_info={“partner_id”:”2088102114633762”,“out_trade_no”:”1234567”}  当recognition_type=2时， recognition_info={“user_id”:”2088102114633761“ }
        /// </summary>
        public string RecognitionInfo { get; set; }

        /// <summary>
        /// 发放对象识别类型  1- 订单信息  2- 支付宝userId
        /// </summary>
        public string RecognitionType { get; set; }

        /// <summary>
        /// 该pass的核销方式,如果为空，则默认为["wave","qrcode"]
        /// </summary>
        public List<string> VerifyType { get; set; }

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
            return "alipay.pass.code.add";
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
            parameters.Add("file_content", this.FileContent);
            parameters.Add("recognition_info", this.RecognitionInfo);
            parameters.Add("recognition_type", this.RecognitionType);
            parameters.Add("verify_type", this.VerifyType);
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
