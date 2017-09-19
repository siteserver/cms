using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
    public class NodeInfoExtend : ExtendedAttributes
    {
        public NodeInfoExtend(string extendValues)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(extendValues);
            SetExtendedAttribute(nameValueCollection);
        }

        //是否可以添加栏目
        public bool IsChannelAddable
        {
            get { return GetBool("IsChannelAddable", true); }
            set { SetExtendedAttribute("IsChannelAddable", value.ToString()); }
        }

        //是否可以添加内容
        public bool IsContentAddable
        {
            get { return GetBool("IsContentAddable", true); }
            set { SetExtendedAttribute("IsContentAddable", value.ToString()); }
        }

        public bool IsChannelCreatable
        {
            get { return GetBool("IsChannelCreatable", true); }
            set { SetExtendedAttribute("IsChannelCreatable", value.ToString()); }
        }

        public bool IsContentCreatable
        {
            get { return GetBool("IsContentCreatable", true); }
            set { SetExtendedAttribute("IsContentCreatable", value.ToString()); }
        }

        public bool IsCreateChannelIfContentChanged
        {
            get { return GetBool("IsCreateChannelIfContentChanged", true); }
            set { SetExtendedAttribute("IsCreateChannelIfContentChanged", value.ToString()); }
        }

        public string CreateChannelIDsIfContentChanged
        {
            get { return GetExtendedAttribute("CreateChannelIDsIfContentChanged"); }
            set { SetExtendedAttribute("CreateChannelIDsIfContentChanged", value); }
        }

        public string ContentAttributesOfDisplay
        {
            get { return GetExtendedAttribute("ContentAttributesOfDisplay"); }
            set { SetExtendedAttribute("ContentAttributesOfDisplay", value); }
        }

        public ECrossSiteTransType TransType
        {
            get { return ECrossSiteTransTypeUtils.GetEnumType(GetExtendedAttribute("TransType")); }
            set { SetExtendedAttribute("TransType", ECrossSiteTransTypeUtils.GetValue(value)); }
        }

        public int TransPublishmentSystemId
        {
            get { return TranslateUtils.ToInt(GetExtendedAttribute("TransPublishmentSystemId")); }
            set { SetExtendedAttribute("TransPublishmentSystemId", value.ToString()); }
        }

        public string TransNodeIds
        {
            get { return GetExtendedAttribute("TransNodeIds"); }
            set { SetExtendedAttribute("TransNodeIds", value); }
        }

        public string TransNodeNames
        {
            get { return GetExtendedAttribute("TransNodeNames"); }
            set { SetExtendedAttribute("TransNodeNames", value); }
        }

        public bool TransIsAutomatic
        {
            get { return GetBool("TransIsAutomatic", false); }
            set { SetExtendedAttribute("TransIsAutomatic", value.ToString()); }
        }

        //跨站转发操作类型：复制 引用地址 引用内容
        public ETranslateContentType TransDoneType
        {
            get { return ETranslateContentTypeUtils.GetEnumType(GetExtendedAttribute("TransDoneType")); }
            set { SetExtendedAttribute("TransDoneType", ETranslateContentTypeUtils.GetValue(value)); }
        }

        public bool IsPreviewContents
        {
            get { return GetBool("IsPreviewContents", false); }
            set { SetExtendedAttribute("IsPreviewContents", value.ToString()); }
        }

        public string DefaultTaxisType
        {
            get { return GetString("DefaultTaxisType", ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc)); }
            set { SetExtendedAttribute("DefaultTaxisType", value); }
        }

        /****************内容签收设置********************/

        public bool IsSignin
        {
            get { return GetBool("IsSignin", false); }
            set { SetExtendedAttribute("IsSignin", value.ToString()); }
        }

        public bool IsSigninGroup
        {
            get { return GetBool("IsSigninGroup", true); }
            set { SetExtendedAttribute("IsSigninGroup", value.ToString()); }
        }

        public string SigninUserGroupCollection
        {
            get { return GetExtendedAttribute("SigninUserGroupCollection"); }
            set { SetExtendedAttribute("SigninUserGroupCollection", value); }
        }

        public string SigninUserNameCollection
        {
            get { return GetExtendedAttribute("SigninUserNameCollection"); }
            set { SetExtendedAttribute("SigninUserNameCollection", value); }
        }

        public int SigninPriority
        {
            get { return TranslateUtils.ToInt(GetExtendedAttribute("SigninPriority")); }
            set { SetExtendedAttribute("SigninPriority", value.ToString()); }
        }

        public string SigninEndDate
        {
            get { return GetExtendedAttribute("SigninEndDate"); }
            set { SetExtendedAttribute("SigninEndDate", value); }
        }

        public string PluginIds
        {
            get { return GetExtendedAttribute("PluginIds"); }
            set { SetExtendedAttribute("PluginIds", value); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(GetExtendedAttributes());
        }
    }
}
