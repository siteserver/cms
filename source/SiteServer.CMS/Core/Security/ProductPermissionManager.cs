using BaiRong.Core.Auth;

namespace SiteServer.CMS.Core.Security
{
	public class ProductPermissionsManager
	{
        private readonly string _userName;
        private ProductAdministratorWithPermissions _permissions;

        private ProductPermissionsManager(string userName)
        {
            _userName = userName;
        }

        public static ProductAdministratorWithPermissions Current
        {
            get
            {
                var instance = new ProductPermissionsManager(RequestBody.CurrentAdministratorName);
                return instance.Permissions;
            }
        }

        public ProductAdministratorWithPermissions Permissions
        {
            get {
                return _permissions ??
                       (_permissions =
                           !string.IsNullOrEmpty(_userName)
                               ? new ProductAdministratorWithPermissions(_userName)
                               : ProductAdministratorWithPermissions.GetProductAnonymousUserWithPermissions());
            }
            set { _permissions = value; }
        }
	}
}
