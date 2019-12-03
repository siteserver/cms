using System.Collections.Specialized;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages
{
    public class BasePageCms : BasePage
	{
        public bool HasChannelPermissions(int channelId, params string[] channelPermissionArray)
        {
            return true;
            //return AuthRequest.AdminPermissionsImpl.HasChannelPermissionsAsync(SiteId, channelId, channelPermissionArray).GetAwaiter().GetResult();
        }

        public bool HasChannelPermissionsIgnoreChannelId(params string[] channelPermissionArray)
        {
            return true;
            //return AuthRequest.AdminPermissionsImpl.HasChannelPermissionsIgnoreChannelIdAsync(channelPermissionArray).GetAwaiter().GetResult();
        }

        public bool HasSitePermissions(params string[] websitePermissionArray)
        {
            return true;
            //return AuthRequest.AdminPermissionsImpl.HasSitePermissionsAsync(SiteId, websitePermissionArray).GetAwaiter().GetResult();
        }

        public bool IsOwningChannelId(int channelId)
        {
            return true;
            //return AuthRequest.AdminPermissionsImpl.IsOwningChannelIdAsync(channelId).GetAwaiter().GetResult();
        }

        public bool IsDescendantOwningChannelId(int channelId)
        {
            return true;
            //return AuthRequest.AdminPermissionsImpl.IsDescendantOwningChannelIdAsync(SiteId, channelId).GetAwaiter().GetResult();
        }

        private int _siteId = -1;
        public virtual int SiteId
        {
            get
            {
                if (_siteId == -1)
                {
                    _siteId = AuthRequest.GetQueryInt("siteId");
                }
                return _siteId;
            }
        }

        private AuthenticatedRequest _authRequest;

        public AuthenticatedRequest AuthRequest
        {
            get
            {
                if (_authRequest == null)
                {
                    _authRequest = AuthenticatedRequest.GetAuthAsync().GetAwaiter().GetResult();
                }

                return _authRequest;
            }
        }

        private Site _site;

	    public Site Site
	    {
	        get
	        {
	            if (_site != null) return _site;
	            _site = DataProvider.SiteRepository.GetAsync(SiteId).GetAwaiter().GetResult();
	            return _site;
	        }
	    }

        public void VerifySitePermissions(params string[] sitePermissions)
        {
            if (AuthRequest.AdminPermissionsImpl.HasSitePermissionsAsync(SiteId, sitePermissions).GetAwaiter().GetResult())
            {
                return;
            }
            AuthRequest.AdminLogout();
            PageUtils.Redirect(PageUtils.GetAdminUrl(string.Empty));
        }

        public void VerifyChannelPermissions(int channelId, params string[] channelPermissions)
        {
            if (HasChannelPermissions(channelId, channelPermissions))
            {
                return;
            }
            AuthRequest.AdminLogout();
            PageUtils.Redirect(PageUtils.GetAdminUrl(string.Empty));
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
