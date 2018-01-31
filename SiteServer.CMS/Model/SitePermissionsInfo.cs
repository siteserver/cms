using System;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class SitePermissionsInfo
	{
        private int _id;
        private string _roleName;
		private int _siteId;
		private string _channelIdCollection;
		private string _channelPermissions;
		private string _websitePermissions;

		public SitePermissionsInfo()
		{
		    _id = 0;
			_roleName = string.Empty;
			_siteId = 0;
			_channelIdCollection = string.Empty;
			_channelPermissions = string.Empty;
			_websitePermissions = string.Empty;
		}

        public SitePermissionsInfo(string roleName, int siteId, string channelIdCollection, string channelPermissions, string websitePermissions) 
		{
			_roleName = roleName;
			_siteId = siteId;
			_channelIdCollection = channelIdCollection;
			_channelPermissions = channelPermissions;
			_websitePermissions = websitePermissions;
		}

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string RoleName
		{
			get{ return _roleName; }
			set{ _roleName = value; }
		}

		public int SiteId
		{
			get{ return _siteId; }
			set{ _siteId = value; }
		}

		public string ChannelIdCollection
		{
			get{ return _channelIdCollection; }
			set{ _channelIdCollection = value; }
		}

		public string ChannelPermissions
		{
			get{ return _channelPermissions; }
			set{ _channelPermissions = value; }
		}

		public string WebsitePermissions
		{
			get{ return _websitePermissions; }
			set{ _websitePermissions = value; }
		}
	}
}
