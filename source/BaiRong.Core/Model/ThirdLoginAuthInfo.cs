namespace BaiRong.Core.Model
{
    public class ThirdLoginAuthInfo : ExtendedAttributes
    {
        public ThirdLoginAuthInfo(string settingsXml)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXml);
            SetExtendedAttribute(nameValueCollection);
        }

        public string AppKey     //第三方认证ID
        {
            get { return GetString("AppKey", string.Empty); }
            set { SetExtendedAttribute("AppKey", value); }
        }

        public string AppSercet     //第三方认证秘钥
        {
            get { return GetString("AppSercet", string.Empty); }
            set { SetExtendedAttribute("AppSercet", value); }
        }

        public string CallBackUrl     //第三方请求返回地址
        {
            get { return GetString("CallBackUrl", string.Empty); }
            set { SetExtendedAttribute("CallBackUrl", value); }
        }
    }
}
