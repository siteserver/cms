using System;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class PermissionsInRolesInfo
	{
        private int _id;
        private string _roleName;
		private string _generalPermissions;

		public PermissionsInRolesInfo()
		{
		    _id = 0;
            _roleName = string.Empty;
			_generalPermissions = string.Empty;
		}

        public PermissionsInRolesInfo(int id, string roleName, string generalPermissions)
        {
            _id = id;
            _roleName = roleName;
			_generalPermissions = generalPermissions;
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

		public string GeneralPermissions
		{
			get{ return _generalPermissions; }
			set{ _generalPermissions = value; }
		}
	}
}
