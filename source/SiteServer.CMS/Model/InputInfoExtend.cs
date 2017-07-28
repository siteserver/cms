using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.CMS.Model
{
    public class InputInfoExtend : ExtendedAttributes
    {
        public InputInfoExtend(string settingsXml)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXml);
            SetExtendedAttribute(nameValueCollection);
        }

        public string MessageSuccess
        {
            get { return GetString("MessageSuccess", string.Empty); }
            set { SetExtendedAttribute("MessageSuccess", value); }
        }

        public string MessageFailure
        {
            get { return GetString("MessageFailure", string.Empty); }
            set { SetExtendedAttribute("MessageFailure", value); }
        }

        public bool IsAnomynous         //允许匿名评论
        {
            get { return GetBool("IsAnomynous", true); }
            set { SetExtendedAttribute("IsAnomynous", value.ToString()); }
        }

        public bool IsSuccessHide
        {
            get { return GetBool("IsSuccessHide", true); }
            set { SetExtendedAttribute("IsSuccessHide", value.ToString()); }
        }

        public bool IsSuccessReload
        {
            get { return GetBool("IsSuccessReload", false); }
            set { SetExtendedAttribute("IsSuccessReload", value.ToString()); }
        }

        public bool IsCtrlEnter
        {
            get { return GetBool("IsCtrlEnter", true); }
            set { SetExtendedAttribute("IsCtrlEnter", value.ToString()); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }
    }
}
