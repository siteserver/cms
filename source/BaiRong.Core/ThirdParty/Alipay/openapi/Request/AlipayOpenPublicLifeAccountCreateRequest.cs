using System;
using System.Collections.Generic;
using Aop.Api.Response;
using Aop.Api.Util;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.open.public.life.account.create
    /// </summary>
    public class AlipayOpenPublicLifeAccountCreateRequest : IAopUploadRequest<AlipayOpenPublicLifeAccountCreateResponse>
    {
        /// <summary>
        /// 背景图片，需上传图片原始二进制流，此图片显示在支付宝客户端生活号主页上方背景图位置，后缀是jpg或者jpeg，图片大小限制1mb
        /// </summary>
        public FileItem Background { get; set; }

        /// <summary>
        /// 生活号二级分类id，请按照以下分类进行填写，非以下分类账号请联系相应人员添加类别  综合媒体(INTEG)，新闻(NEWS)，科技(SCIENCE)，养生(WELLNESS)，财经(FINANCE)，情感(EMOTION)，美食(DELICACY)，搞笑(FUNNY)，娱乐(ENTERTM)，摄影(SHOOT)，影视(MOVIES)，教育(EDUCATE)，文艺(LITER)，时尚(FASHION)，动漫(COMIC)，美妆(COSMETIC)，体育(SPOTRS)，旅行(TRIP)，健身(BODYB)，星座(CONSTT)，音乐(ONGAKU)，母婴(MUNBABY)，公益(PUBLICW)，汽车(CARS)，地产(LAND)，数码(NUMERAL)，游戏(GAMES)，电视剧(TVPLAY)，宠物(PET)，其他(OTHERS)
        /// </summary>
        public string CatagoryId { get; set; }

        /// <summary>
        /// 联系人邮箱，可以是调用者的联系人邮箱
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系人电话，可以是调用者的联系人电话
        /// </summary>
        public string ContactTel { get; set; }

        /// <summary>
        /// 生活号简介，此内容显示在支付宝客户端生活号主页简介区块
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 客服电话，可以是电话号码，手机号码，400电话
        /// </summary>
        public string CustomerTel { get; set; }

        /// <summary>
        /// 生活号名称，该名称会显示在支付宝客户端生活号主页上方
        /// </summary>
        public string LifeName { get; set; }

        /// <summary>
        /// logo图片，需上传图片原始二进制流，此图片显示在支付宝客户端生活号主页上方位置，后缀是jpg或者jpeg，图片大小限制1mb
        /// </summary>
        public FileItem Logo { get; set; }

        /// <summary>
        /// 支付宝用户id，由支付宝同学提供用户id，为该生活号对应pid
        /// </summary>
        public string UserId { get; set; }

        #region IAopRequest Members
		private bool needEncrypt=false;
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

		public void SetApiVersion(string apiVersion){
            this.apiVersion=apiVersion;
        }

        public string GetApiVersion(){
            return this.apiVersion;
        }

        public string GetApiName()
        {
            return "alipay.open.public.life.account.create";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("catagory_id", this.CatagoryId);
            parameters.Add("contact_email", this.ContactEmail);
            parameters.Add("contact_tel", this.ContactTel);
            parameters.Add("content", this.Content);
            parameters.Add("customer_tel", this.CustomerTel);
            parameters.Add("life_name", this.LifeName);
            parameters.Add("user_id", this.UserId);
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

        #region IAopUploadRequest Members

        public IDictionary<string, FileItem> GetFileParameters()
        {
            IDictionary<string, FileItem> parameters = new Dictionary<string, FileItem>();
            parameters.Add("background", this.Background);
            parameters.Add("logo", this.Logo);
            return parameters;
        }

        #endregion
    }
}
