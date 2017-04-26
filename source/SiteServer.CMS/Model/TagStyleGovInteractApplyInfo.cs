using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class TagStyleGovInteractApplyInfo : ExtendedAttributes, ITagStyleMailSMSBaseInfo
    {
        public TagStyleGovInteractApplyInfo(string settingsXML)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXML);
            SetExtendedAttribute(nameValueCollection);
        }

        public bool IsAnomynous         //允许匿名评论
        {
            get { return GetBool("IsAnomynous", true); }
            set { SetExtendedAttribute("IsAnomynous", value.ToString()); }
        }

        public bool IsMail
        {
            get { return GetBool("IsMail", false); }
            set { SetExtendedAttribute("IsMail", value.ToString()); }
        }

        public ETriState MailReceiver
        {
            get { return ETriStateUtils.GetEnumType(GetString("MailReceiver", ETriStateUtils.GetValue(ETriState.True))); }
            set { SetExtendedAttribute("MailReceiver", ETriStateUtils.GetValue(value)); }
        }

        public string MailTo
        {
            get { return GetString("MailTo", string.Empty); }
            set { SetExtendedAttribute("MailTo", value); }
        }

        public string MailFiledName
        {
            get { return GetString("MailFiledName", string.Empty); }
            set { SetExtendedAttribute("MailFiledName", value); }
        }

        public string MailTitle
        {
            get { return GetString("MailTitle", string.Empty); }
            set { SetExtendedAttribute("MailTitle", value); }
        }

        public bool IsMailTemplate
        {
            get { return GetBool("IsMailTemplate", false); }
            set { SetExtendedAttribute("IsMailTemplate", value.ToString()); }
        }

        public string MailContent
        {
            get { return GetString("MailContent", string.Empty); }
            set { SetExtendedAttribute("MailContent", value); }
        }

        public bool IsSMS
        {
            get { return GetBool("IsSMS", false); }
            set { SetExtendedAttribute("IsSMS", value.ToString()); }
        }

        public ETriState SMSReceiver
        {
            get { return ETriStateUtils.GetEnumType(GetString("SMSReceiver", ETriStateUtils.GetValue(ETriState.True))); }
            set { SetExtendedAttribute("SMSReceiver", ETriStateUtils.GetValue(value)); }
        }

        public string SMSTo
        {
            get { return GetString("SMSTo", string.Empty); }
            set { SetExtendedAttribute("SMSTo", value); }
        }

        public string SMSFiledName
        {
            get { return GetString("SMSFiledName", string.Empty); }
            set { SetExtendedAttribute("SMSFiledName", value); }
        }

        public bool IsSMSTemplate
        {
            get { return GetBool("IsSMSTemplate", false); }
            set { SetExtendedAttribute("IsSMSTemplate", value.ToString()); }
        }

        public string SMSContent
        {
            get { return GetString("SMSContent", string.Empty); }
            set { SetExtendedAttribute("SMSContent", value); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }
    }
}
