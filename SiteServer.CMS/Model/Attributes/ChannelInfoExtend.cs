using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model.Attributes
{
    public class ChannelInfoExtend : AttributesImpl
    {
        public ChannelInfoExtend(string settings) : base(settings)
        {

        }

        //是否可以添加栏目
        public bool IsChannelAddable
        {
            get => GetBool(nameof(IsChannelAddable), true);
            set => Set(nameof(IsChannelAddable), value);
        }

        //是否可以添加内容
        public bool IsContentAddable
        {
            get => GetBool(nameof(IsContentAddable), true);
            set => Set(nameof(IsContentAddable), value);
        }

        //是否可以生成栏目
        public bool IsChannelCreatable
        {
            get => GetBool(nameof(IsChannelCreatable), true);
            set => Set(nameof(IsChannelCreatable), value);
        }

        //是否可以生成内容
        public bool IsContentCreatable
        {
            get => GetBool(nameof(IsContentCreatable), true);
            set => Set(nameof(IsContentCreatable), value);
        }

        public bool IsCreateChannelIfContentChanged
        {
            get => GetBool(nameof(IsCreateChannelIfContentChanged), true);
            set => Set(nameof(IsCreateChannelIfContentChanged), value);
        }

        public string CreateChannelIdsIfContentChanged
        {
            get => GetString(nameof(CreateChannelIdsIfContentChanged));
            set => Set(nameof(CreateChannelIdsIfContentChanged), value);
        }

        public string ContentAttributesOfDisplay
        {
            get => GetString(nameof(ContentAttributesOfDisplay));
            set => Set(nameof(ContentAttributesOfDisplay), value);
        }

        public ECrossSiteTransType TransType
        {
            get => ECrossSiteTransTypeUtils.GetEnumType(GetString(nameof(TransType)));
            set => Set(nameof(TransType), ECrossSiteTransTypeUtils.GetValue(value));
        }

        public int TransSiteId
        {
            get => TranslateUtils.ToInt(GetString(nameof(TransSiteId)));
            set => Set(nameof(TransSiteId), value);
        }

        public string TransChannelIds
        {
            get => GetString(nameof(TransChannelIds));
            set => Set(nameof(TransChannelIds), value);
        }

        public string TransChannelNames
        {
            get => GetString(nameof(TransChannelNames));
            set => Set(nameof(TransChannelNames), value);
        }

        public bool TransIsAutomatic
        {
            get => GetBool(nameof(TransIsAutomatic));
            set => Set(nameof(TransIsAutomatic), value);
        }

        //跨站转发操作类型：复制 引用地址 引用内容
        public ETranslateContentType TransDoneType
        {
            get => ETranslateContentTypeUtils.GetEnumType(GetString(nameof(TransDoneType)));
            set => Set(nameof(TransDoneType), ETranslateContentTypeUtils.GetValue(value));
        }

        public bool IsPreviewContentsExists
        {
            get => GetBool(nameof(IsPreviewContentsExists));
            set => Set(nameof(IsPreviewContentsExists), value);
        }

        public string DefaultTaxisType
        {
            get => GetString(nameof(DefaultTaxisType), ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc));
            set => Set(nameof(DefaultTaxisType), value);
        }
    }
}
