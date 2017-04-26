using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.CMS.Model
{
    public class TagStyleGovPublicApplyInfo : ExtendedAttributes
    {
        public TagStyleGovPublicApplyInfo(string settingsXML)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXML);
            SetExtendedAttribute(nameValueCollection);
        }

        public bool IsSMS
        {
            get { return GetBool("IsSMS", false); }
            set { SetExtendedAttribute("IsSMS", value.ToString()); }
        }

        public string SMSTo
        {
            get { return GetString("SMSTo", string.Empty); }
            set { SetExtendedAttribute("SMSTo", value); }
        }

        public string SMSTitle
        {
            get { return GetString("SMSTitle", string.Empty); }
            set { SetExtendedAttribute("SMSTitle", value); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }
    }
}
