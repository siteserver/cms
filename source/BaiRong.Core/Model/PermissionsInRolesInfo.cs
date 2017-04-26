using System;

namespace BaiRong.Core.Model
{
	[Serializable]
	public class PermissionsInRolesInfo
	{
		private string _roleName;
		private string _generalPermissions;

		public PermissionsInRolesInfo()
		{
			_roleName = string.Empty;
			_generalPermissions = string.Empty;
		}

        public PermissionsInRolesInfo(string roleName, string generalPermissions) 
		{
			_roleName = roleName;
			_generalPermissions = generalPermissions;
		}

		public string RoleName
		{
			get{ return _roleName; }
			set{ _roleName = value; }
		}

		public string GeneralPermissions
		{
			get{ return _generalPermissions; }
			set{ _generalPermissions = value; }
		}
	}
}
