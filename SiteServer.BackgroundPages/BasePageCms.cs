using System.Collections.Specialized;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages
{
    public class BasePageCms : BasePage
	{
        public bool HasChannelPermissions(int channelId, params string[] channelPermissionArray)
        {
            return AdminUtility.HasChannelPermissions(Body.AdminName, SiteId, channelId, channelPermissionArray);
        }

        public bool HasChannelPermissionsIgnoreChannelId(params string[] channelPermissionArray)
        {
            return AdminUtility.HasChannelPermissionsIgnoreChannelId(Body.AdminName, channelPermissionArray);
        }

        public bool HasSitePermissions(params string[] websitePermissionArray)
        {
            return AdminUtility.HasSitePermissions(Body.AdminName, SiteId, websitePermissionArray);
        }

        public bool IsOwningChannelId(int channelId)
        {
            return AdminUtility.IsOwningChannelId(Body.AdminName, channelId);
        }

        public bool IsHasChildOwningChannelId(int channelId)
        {
            return AdminUtility.IsHasChildOwningChannelId(Body.AdminName, channelId);
        }

        private int _siteId = -1;
        public virtual int SiteId
        {
            get
            {
                if (_siteId == -1)
                {
                    _siteId = Body.GetQueryInt("siteId");
                }
                return _siteId;
            }
        }

        private SiteInfo _siteInfo;

	    public SiteInfo SiteInfo
	    {
	        get
	        {
	            if (_siteInfo != null) return _siteInfo;
	            _siteInfo = SiteManager.GetSiteInfo(SiteId);
	            return _siteInfo;
	        }
	    }

	    public void VerifySitePermissions(string permission)
	    {
            AdminUtility.VerifySitePermissions(Body.AdminName, SiteId, permission);
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
