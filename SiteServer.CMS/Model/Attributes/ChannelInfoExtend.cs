using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model.Attributes
{
    public class ChannelInfoExtend : ExtendedAttributes
    {
        public ChannelInfoExtend(string settings) : base(settings)
        {

        }

        //是否可以添加栏目
        public bool IsChannelAddable
        {
            get => GetBool("IsChannelAddable", true);
            set => Set("IsChannelAddable", value);
        }

        //是否可以添加内容
        public bool IsContentAddable
        {
            get => GetBool("IsContentAddable", true);
            set => Set("IsContentAddable", value);
        }

        public bool IsCreateChannelIfContentChanged
        {
            get => GetBool("IsCreateChannelIfContentChanged", true);
            set => Set("IsCreateChannelIfContentChanged", value);
        }

        public string CreateChannelIDsIfContentChanged
        {
            get => GetString("CreateChannelIDsIfContentChanged");
            set => Set("CreateChannelIDsIfContentChanged", value);
        }

        public string ContentAttributesOfDisplay
        {
            get => GetString("ContentAttributesOfDisplay");
            set => Set("ContentAttributesOfDisplay", value);
        }

        public ECrossSiteTransType TransType
        {
            get => ECrossSiteTransTypeUtils.GetEnumType(GetString("TransType"));
            set => Set("TransType", ECrossSiteTransTypeUtils.GetValue(value));
        }

        public int TransSiteId
        {
            get => TranslateUtils.ToInt(GetString("TransSiteId"));
            set => Set("TransSiteId", value);
        }

        public string TransChannelIds
        {
            get => GetString("TransChannelIds");
            set => Set("TransChannelIds", value);
        }

        public string TransChannelNames
        {
            get => GetString("TransChannelNames");
            set => Set("TransChannelNames", value);
        }

        public bool TransIsAutomatic
        {
            get => GetBool("TransIsAutomatic");
            set => Set("TransIsAutomatic", value);
        }

        //跨站转发操作类型：复制 引用地址 引用内容
        public ETranslateContentType TransDoneType
        {
            get => ETranslateContentTypeUtils.GetEnumType(GetString("TransDoneType"));
            set => Set("TransDoneType", ETranslateContentTypeUtils.GetValue(value));
        }

        public bool IsPreviewContents
        {
            get => GetBool("IsPreviewContents");
            set => Set("IsPreviewContents", value);
        }

        public string DefaultTaxisType
        {
            get => GetString("DefaultTaxisType", ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc));
            set => Set("DefaultTaxisType", value);
        }
    }
}
