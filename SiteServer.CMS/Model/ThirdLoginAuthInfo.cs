using BaiRong.Core;
using BaiRong.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteServer.CMS.Model
{
    public class ThirdLoginAuthInfo : ExtendedAttributes
    {
        public ThirdLoginAuthInfo(string settingsXML)
        {
            NameValueCollection nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXML);
            base.SetExtendedAttribute(nameValueCollection);
        }

        public string AppKey     //第三方认证ID
        {
            get { return base.GetString("AppKey", string.Empty); }
            set { base.SetExtendedAttribute("AppKey", value); }
        }

        public string AppSercet     //第三方认证秘钥
        {
            get { return base.GetString("AppSercet", string.Empty); }
            set { base.SetExtendedAttribute("AppSercet", value); }
        }

        public string CallBackUrl     //第三方请求返回地址
        {
            get { return base.GetString("CallBackUrl", string.Empty); }
            set { base.SetExtendedAttribute("CallBackUrl", value); }
        }
    }
}
