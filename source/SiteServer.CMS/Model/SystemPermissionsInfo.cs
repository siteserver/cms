using System;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class SystemPermissionsInfo
	{
		private string _roleName;
		private int _publishmentSystemId;
		private string _nodeIdCollection;
		private string _channelPermissions;
		private string _websitePermissions;

		public SystemPermissionsInfo()
		{
			_roleName = string.Empty;
			_publishmentSystemId = 0;
			_nodeIdCollection = string.Empty;
			_channelPermissions = string.Empty;
			_websitePermissions = string.Empty;
		}

        public SystemPermissionsInfo(string roleName, int publishmentSystemId, string nodeIdCollection, string channelPermissions, string websitePermissions) 
		{
			_roleName = roleName;
			_publishmentSystemId = publishmentSystemId;
			_nodeIdCollection = nodeIdCollection;
			_channelPermissions = channelPermissions;
			_websitePermissions = websitePermissions;
		}

		public string RoleName
		{
			get{ return _roleName; }
			set{ _roleName = value; }
		}

		public int PublishmentSystemId
		{
			get{ return _publishmentSystemId; }
			set{ _publishmentSystemId = value; }
		}

		public string NodeIdCollection
		{
			get{ return _nodeIdCollection; }
			set{ _nodeIdCollection = value; }
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
