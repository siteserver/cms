using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class NodeInfoExtend : ExtendedAttributes
    {
        public NodeInfoExtend(string settings) : base(settings)
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

        public int TransPublishmentSystemId
        {
            get { return TranslateUtils.ToInt(GetString("TransPublishmentSystemId")); }
            set { Set("TransPublishmentSystemId", value.ToString()); }
        }

        public string TransNodeIds
        {
            get { return GetString("TransNodeIds"); }
            set { Set("TransNodeIds", value); }
        }

        public string TransNodeNames
        {
            get { return GetString("TransNodeNames"); }
            set { Set("TransNodeNames", value); }
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
