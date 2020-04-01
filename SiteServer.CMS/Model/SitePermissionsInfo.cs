using System;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class SitePermissionsInfo
	{
        public SitePermissionsInfo() { }

        public SitePermissionsInfo(string roleName, int siteId, string channelIdCollection, string channelPermissions, string websitePermissions) 
		{
			RoleName = roleName;
			SiteId = siteId;
			ChannelIdCollection = channelIdCollection;
			ChannelPermissions = channelPermissions;
			WebsitePermissions = websitePermissions;
		}

        public int Id { get; set; }

        public string RoleName { get; set; }

        public int SiteId { get; set; }

        public string ChannelIdCollection { get; set; }

        public string ChannelPermissions { get; set; }

        public string WebsitePermissions { get; set; }
    }
}
