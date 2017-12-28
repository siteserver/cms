using System.Collections.Specialized;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages
{
    public class BasePageCms : BasePage
	{
        public bool HasChannelPermissions(int nodeId, params string[] channelPermissionArray)
        {
            return AdminUtility.HasChannelPermissions(Body.AdminName, PublishmentSystemId, nodeId, channelPermissionArray);
        }

        public bool HasChannelPermissionsIgnoreNodeId(params string[] channelPermissionArray)
        {
            return AdminUtility.HasChannelPermissionsIgnoreNodeId(Body.AdminName, channelPermissionArray);
        }

        public bool HasSitePermissions(params string[] websitePermissionArray)
        {
            return AdminUtility.HasSitePermissions(Body.AdminName, PublishmentSystemId, websitePermissionArray);
        }

        public bool IsOwningNodeId(int nodeId)
        {
            return AdminUtility.IsOwningNodeId(Body.AdminName, nodeId);
        }

        public bool IsHasChildOwningNodeId(int nodeId)
        {
            return AdminUtility.IsHasChildOwningNodeId(Body.AdminName, nodeId);
        }

        private int _publishmentSystemId = -1;
        public virtual int PublishmentSystemId
        {
            get
            {
                if (_publishmentSystemId == -1)
                {
                    _publishmentSystemId = Body.GetQueryInt("publishmentSystemId");
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

	    public void VerifySitePermissions(string permission)
	    {
            AdminUtility.VerifySitePermissions(Body.AdminName, PublishmentSystemId, permission);
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
