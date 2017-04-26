using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.CMS.Model
{
    public class GatherRuleInfoExtend : ExtendedAttributes
    {
        public GatherRuleInfoExtend(string extendValues)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(extendValues);
            SetExtendedAttribute(nameValueCollection);
        }

        public int GatherNum
        {
            get { return GetInt("GatherNum", 0); }
            set { SetExtendedAttribute("GatherNum", value.ToString()); }
        }

        public bool IsSaveImage
        {
            get { return GetBool("IsSaveImage", true); }
            set { SetExtendedAttribute("IsSaveImage", value.ToString()); }
        }

        public bool IsSetFirstImageAsImageUrl
        {
            get { return GetBool("IsSetFirstImageAsImageUrl", true); }
            set { SetExtendedAttribute("IsSetFirstImageAsImageUrl", value.ToString()); }
        }

        public bool IsEmptyContentAllowed
        {
            get { return GetBool("IsEmptyContentAllowed", false); }
            set { SetExtendedAttribute("IsEmptyContentAllowed", value.ToString()); }
        }

        public bool IsSameTitleAllowed
        {
            get { return GetBool("IsSameTitleAllowed", false); }
            set { SetExtendedAttribute("IsSameTitleAllowed", value.ToString()); }
        }

        public bool IsChecked
        {
            get { return GetBool("IsChecked", true); }
            set { SetExtendedAttribute("IsChecked", value.ToString()); }
        }

        public bool IsAutoCreate
        {
            get { return GetBool("IsAutoCreate", false); }
            set { SetExtendedAttribute("IsAutoCreate", value.ToString()); }
        }

        public bool IsOrderByDesc
        {
            get { return GetBool("IsOrderByDesc", true); }
            set { SetExtendedAttribute("IsOrderByDesc", value.ToString()); }
        }

        public string ContentContentStart2
        {
            get { return GetString("ContentContentStart2", string.Empty); }
            set { SetExtendedAttribute("ContentContentStart2", value); }
        }

        public string ContentContentEnd2
        {
            get { return GetString("ContentContentEnd2", string.Empty); }
            set { SetExtendedAttribute("ContentContentEnd2", value); }
        }

        public string ContentContentStart3
        {
            get { return GetString("ContentContentStart3", string.Empty); }
            set { SetExtendedAttribute("ContentContentStart3", value); }
        }

        public string ContentContentEnd3
        {
            get { return GetString("ContentContentEnd3", string.Empty); }
            set { SetExtendedAttribute("ContentContentEnd3", value); }
        }

        public string ContentReplaceFrom
        {
            get { return GetString("ContentReplaceFrom", string.Empty); }
            set { SetExtendedAttribute("ContentReplaceFrom", value); }
        }

        public string ContentReplaceTo
        {
            get { return GetString("ContentReplaceTo", string.Empty); }
            set { SetExtendedAttribute("ContentReplaceTo", value); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }
    }
}
