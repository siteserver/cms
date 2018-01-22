using SiteServer.Utils.Model;

namespace SiteServer.CMS.Model
{
    public class GatherRuleInfoExtend : ExtendedAttributes
    {
        public GatherRuleInfoExtend(string settings) : base(settings)
        {

        }

        public int GatherNum
        {
            get { return GetInt("GatherNum"); }
            set { Set("GatherNum", value.ToString()); }
        }

        public bool IsSaveImage
        {
            get { return GetBool("IsSaveImage", true); }
            set { Set("IsSaveImage", value.ToString()); }
        }

        public bool IsSetFirstImageAsImageUrl
        {
            get { return GetBool("IsSetFirstImageAsImageUrl", true); }
            set { Set("IsSetFirstImageAsImageUrl", value.ToString()); }
        }

        public bool IsEmptyContentAllowed
        {
            get { return GetBool("IsEmptyContentAllowed"); }
            set { Set("IsEmptyContentAllowed", value.ToString()); }
        }

        public bool IsSameTitleAllowed
        {
            get { return GetBool("IsSameTitleAllowed"); }
            set { Set("IsSameTitleAllowed", value.ToString()); }
        }

        public bool IsChecked
        {
            get { return GetBool("IsChecked", true); }
            set { Set("IsChecked", value.ToString()); }
        }

        public bool IsAutoCreate
        {
            get { return GetBool("IsAutoCreate"); }
            set { Set("IsAutoCreate", value.ToString()); }
        }

        public bool IsOrderByDesc
        {
            get { return GetBool("IsOrderByDesc", true); }
            set { Set("IsOrderByDesc", value.ToString()); }
        }

        public string ContentContentStart2
        {
            get { return GetString("ContentContentStart2", string.Empty); }
            set { Set("ContentContentStart2", value); }
        }

        public string ContentContentEnd2
        {
            get { return GetString("ContentContentEnd2", string.Empty); }
            set { Set("ContentContentEnd2", value); }
        }

        public string ContentContentStart3
        {
            get { return GetString("ContentContentStart3", string.Empty); }
            set { Set("ContentContentStart3", value); }
        }

        public string ContentContentEnd3
        {
            get { return GetString("ContentContentEnd3", string.Empty); }
            set { Set("ContentContentEnd3", value); }
        }

        public string ContentReplaceFrom
        {
            get { return GetString("ContentReplaceFrom", string.Empty); }
            set { Set("ContentReplaceFrom", value); }
        }

        public string ContentReplaceTo
        {
            get { return GetString("ContentReplaceTo", string.Empty); }
            set { Set("ContentReplaceTo", value); }
        }
    }
}
