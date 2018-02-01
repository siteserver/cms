using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model
{
    public class ChannelInfoExtend : ExtendedAttributes
    {
        public ChannelInfoExtend(string settings) : base(settings)
        {

        }

        //是否可以添加栏目
        public bool IsChannelAddable
        {
            get { return GetBool("IsChannelAddable", true); }
            set { Set("IsChannelAddable", value.ToString()); }
        }

        //是否可以添加内容
        public bool IsContentAddable
        {
            get { return GetBool("IsContentAddable", true); }
            set { Set("IsContentAddable", value.ToString()); }
        }

        public bool IsChannelCreatable
        {
            get { return GetBool("IsChannelCreatable", true); }
            set { Set("IsChannelCreatable", value.ToString()); }
        }

        public bool IsContentCreatable
        {
            get { return GetBool("IsContentCreatable", true); }
            set { Set("IsContentCreatable", value.ToString()); }
        }

        public bool IsCreateChannelIfContentChanged
        {
            get { return GetBool("IsCreateChannelIfContentChanged", true); }
            set { Set("IsCreateChannelIfContentChanged", value.ToString()); }
        }

        public string CreateChannelIDsIfContentChanged
        {
            get { return GetString("CreateChannelIDsIfContentChanged"); }
            set { Set("CreateChannelIDsIfContentChanged", value); }
        }

        public string ContentAttributesOfDisplay
        {
            get { return GetString("ContentAttributesOfDisplay"); }
            set { Set("ContentAttributesOfDisplay", value); }
        }

        public ECrossSiteTransType TransType
        {
            get { return ECrossSiteTransTypeUtils.GetEnumType(GetString("TransType")); }
            set { Set("TransType", ECrossSiteTransTypeUtils.GetValue(value)); }
        }

        public int TransSiteId
        {
            get { return TranslateUtils.ToInt(GetString("TransSiteId")); }
            set { Set("TransSiteId", value.ToString()); }
        }

        public string TransChannelIds
        {
            get { return GetString("TransChannelIds"); }
            set { Set("TransChannelIds", value); }
        }

        public string TransChannelNames
        {
            get { return GetString("TransChannelNames"); }
            set { Set("TransChannelNames", value); }
        }

        public bool TransIsAutomatic
        {
            get { return GetBool("TransIsAutomatic"); }
            set { Set("TransIsAutomatic", value.ToString()); }
        }

        //跨站转发操作类型：复制 引用地址 引用内容
        public ETranslateContentType TransDoneType
        {
            get { return ETranslateContentTypeUtils.GetEnumType(GetString("TransDoneType")); }
            set { Set("TransDoneType", ETranslateContentTypeUtils.GetValue(value)); }
        }

        public bool IsPreviewContents
        {
            get { return GetBool("IsPreviewContents"); }
            set { Set("IsPreviewContents", value.ToString()); }
        }

        public string DefaultTaxisType
        {
            get { return GetString("DefaultTaxisType", ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc)); }
            set { Set("DefaultTaxisType", value); }
        }
    }
}
