using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
    public class InputInfoExtend : ExtendedAttributes
    {
        public InputInfoExtend(string settingsXml)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXml);
            SetExtendedAttribute(nameValueCollection);
        }

        //允许匿名评论
        public bool IsAnomynous
        {
            get { return GetBool("IsAnomynous", true); }
            set { SetExtendedAttribute("IsAnomynous", value.ToString()); }
        }

        //向管理员发送短信通知
        public bool IsAdministratorSmsNotify
        {
            get { return GetBool("IsAdministratorSmsNotify", false); }
            set { SetExtendedAttribute("IsAdministratorSmsNotify", value.ToString()); }
        }

        public string AdministratorSmsNotifyTplId
        {
            get { return GetString("AdministratorSmsNotifyTplId", string.Empty); }
            set { SetExtendedAttribute("AdministratorSmsNotifyTplId", value); }
        }

        public string AdministratorSmsNotifyKeys
        {
            get { return GetString("AdministratorSmsNotifyKeys", string.Empty); }
            set { SetExtendedAttribute("AdministratorSmsNotifyKeys", value); }
        }

        public string AdministratorSmsNotifyMobile
        {
            get { return GetString("AdministratorSmsNotifyMobile", string.Empty); }
            set { SetExtendedAttribute("AdministratorSmsNotifyMobile", value); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(GetExtendedAttributes());
        }
    }
}
