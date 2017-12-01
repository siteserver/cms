using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.mobile.public.info.modify
    /// </summary>
    public class AlipayMobilePublicInfoModifyRequest : IAopRequest<AlipayMobilePublicInfoModifyResponse>
    {
        /// <summary>
        /// 服务窗名称，2-20个字之间；不得含有违反法律法规和公序良俗的相关信息；不得侵害他人名誉权、知识产权、商业秘密等合法权利；不得以太过广泛的、或产品、行业词组来命名，如：女装、皮革批发；不得以实名认证的媒体资质账号创建服务窗，或媒体相关名称命名服务窗，如：XX电视台、XX杂志等
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 授权运营书，企业商户若为被经营方授权，需上传加盖公章的扫描件，请使用照片上传接口上传图片获得image_url
        /// </summary>
        public string AuthPic { get; set; }

        /// <summary>
        /// 营业执照地址，建议尺寸 320 x 320px，支持.jpg .jpeg .png 格式，小于3M
        /// </summary>
        public string LicenseUrl { get; set; }

        /// <summary>
        /// 服务窗头像地址，建议尺寸 320 x 320px，支持.jpg .jpeg .png 格式，小于3M
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 服务窗欢迎语，200字以内，首次使用服务窗必须
        /// </summary>
        public string PublicGreeting { get; set; }

        /// <summary>
        /// 第一张门店照片地址，建议尺寸 320 x 320px，支持.jpg .jpeg .png 格式，小于3M
        /// </summary>
        public string ShopPic1 { get; set; }

        /// <summary>
        /// 第二张门店照片地址
        /// </summary>
        public string ShopPic2 { get; set; }

        /// <summary>
        /// 第三张门店照片地址
        /// </summary>
        public string ShopPic3 { get; set; }

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
            return "alipay.mobile.public.info.modify";
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
            parameters.Add("app_name", this.AppName);
            parameters.Add("auth_pic", this.AuthPic);
            parameters.Add("license_url", this.LicenseUrl);
            parameters.Add("logo_url", this.LogoUrl);
            parameters.Add("public_greeting", this.PublicGreeting);
            parameters.Add("shop_pic1", this.ShopPic1);
            parameters.Add("shop_pic2", this.ShopPic2);
            parameters.Add("shop_pic3", this.ShopPic3);
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
