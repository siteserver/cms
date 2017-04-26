using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovPublicIdentifierRuleInfoExtend : ExtendedAttributes
	{
        public GovPublicIdentifierRuleInfoExtend(string settingsXML)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXML);
            SetExtendedAttribute(nameValueCollection);
        }

        //！！！！！！！！！！Sequence！！！！！！！！！！/

        public bool IsSequenceChannelZero
        {
            get { return GetBool("IsSequenceChannelZero", true); }
            set { SetExtendedAttribute("IsSequenceChannelZero", value.ToString()); }
        }

        public bool IsSequenceDepartmentZero
        {
            get { return GetBool("IsSequenceDepartmentZero", false); }
            set { SetExtendedAttribute("IsSequenceDepartmentZero", value.ToString()); }
        }

        public bool IsSequenceYearZero
        {
            get { return GetBool("IsSequenceYearZero", true); }
            set { SetExtendedAttribute("IsSequenceYearZero", value.ToString()); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }
	}
}
