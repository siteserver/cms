using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages
{
    public class BasePageCms : BasePage
	{
        public bool HasChannelPermissions(int nodeId, params string[] channelPermissionArray)
        {
            return AdminUtility.HasChannelPermissions(Body.AdministratorName, PublishmentSystemId, nodeId, channelPermissionArray);
        }

        public bool HasChannelPermissionsIgnoreNodeId(params string[] channelPermissionArray)
        {
            return AdminUtility.HasChannelPermissionsIgnoreNodeId(Body.AdministratorName, channelPermissionArray);
        }

        public bool HasWebsitePermissions(params string[] websitePermissionArray)
        {
            return AdminUtility.HasWebsitePermissions(Body.AdministratorName, PublishmentSystemId, websitePermissionArray);
        }

        public bool IsOwningNodeId(int nodeId)
        {
            return AdminUtility.IsOwningNodeId(Body.AdministratorName, nodeId);
        }

        public bool IsHasChildOwningNodeId(int nodeId)
        {
            return AdminUtility.IsHasChildOwningNodeId(Body.AdministratorName, nodeId);
        }

        private int _publishmentSystemId = -1;
        public int PublishmentSystemId
        {
            get
            {
                if (_publishmentSystemId == -1)
                {
                    _publishmentSystemId = Body.GetQueryInt("PublishmentSystemID");
                }
                return _publishmentSystemId;
            }
        }

        private PublishmentSystemInfo _publishmentSystemInfo;

	    public PublishmentSystemInfo PublishmentSystemInfo
	    {
	        get
	        {
	            if (_publishmentSystemInfo != null) return _publishmentSystemInfo;
	            _publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(PublishmentSystemId);
	            return _publishmentSystemInfo;
	        }
	    }

	    public void BreadCrumbWithItemTitle(string leftMenuId, string pageTitle, string itemTitle, string permission)
        {
            BreadCrumbWithItemTitle(leftMenuId, string.Empty, pageTitle, itemTitle, permission);
        }

        public void BreadCrumbWithItemTitle(string leftMenuId, string leftSubMenuId, string pageTitle, string itemTitle, string permission)
        {
            if (ltlBreadCrumb != null)
            {
                var pageUrl = PathUtils.GetFileName(Request.FilePath);
                var leftTitle = AppManager.GetLeftMenuName(leftMenuId);
                var leftSubTitle = AppManager.GetLeftSubMenuName(leftSubMenuId);
                ltlBreadCrumb.Text = StringUtils.GetBreadCrumbHtml(AppManager.IdManagement, string.Empty, leftMenuId, leftTitle, leftSubMenuId, leftSubTitle, pageUrl, pageTitle, itemTitle);
            }

            if (!string.IsNullOrEmpty(permission))
            {
                AdminUtility.VerifyWebsitePermissions(Body.AdministratorName, PublishmentSystemId, permission);
            }
        }

        public override void BreadCrumb(string leftMenuId, string pageTitle, string permission)
        {
            BreadCrumbWithItemTitle(leftMenuId, pageTitle, string.Empty, permission);
        }

        public void BreadCrumb(string leftMenuId, string leftSubMenuId, string pageTitle, string permission)
        {
            BreadCrumbWithItemTitle(leftMenuId, leftSubMenuId, pageTitle, string.Empty, permission);
        }

        private NameValueCollection _attributes;
        public NameValueCollection Attributes => _attributes ?? (_attributes = new NameValueCollection());

	    public void AddAttributes(NameValueCollection attributes)
        {
            if (attributes == null) return;
            foreach (string key in attributes.Keys)
            {
                Attributes[key] = attributes[key];
            }
        }

        public NameValueCollection GetAttributes()
        {
            return Attributes;
        }

        public virtual string GetValue(string attributeName)
        {
            return _attributes != null ? _attributes[attributeName] : string.Empty;
        }

        public void SetValue(string name, string value)
        {
            Attributes[name] = value;
        }

        public void RemoveValue(string name)
        {
            Attributes.Remove(name);
        }

        public string GetSelected(string attributeName, string value)
        {
            if (_attributes == null) return string.Empty;

            return _attributes[attributeName] == value ? @"selected=""selected""" : string.Empty;
        }

        public string GetSelected(string attributeName, string value, bool isDefault)
        {
            if (_attributes != null)
            {
                if (_attributes[attributeName] == value)
                {
                    return @"selected=""selected""";
                }
            }
            else
            {
                if (isDefault)
                {
                    return @"selected=""selected""";
                }
            }
            return string.Empty;
        }

        public string GetChecked(string attributeName, string value)
        {
            if (_attributes == null) return string.Empty;
            return _attributes[attributeName] == value ? @"checked=""checked""" : string.Empty;
        }

        public string GetChecked(string attributeName, string value, bool isDefault)
        {
            if (_attributes != null)
            {
                if (_attributes[attributeName] == value)
                {
                    return @"checked=""checked""";
                }
            }
            else
            {
                if (isDefault)
                {
                    return @"checked=""checked""";
                }
            }
            return string.Empty;
        }
    }
}
