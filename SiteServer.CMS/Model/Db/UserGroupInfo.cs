using Dapper.Contrib.Extensions;
using SiteServer.CMS.Provider;

namespace SiteServer.CMS.Model.Db
{
    [Table(UserGroupDao.DatabaseTableName)]
    public class UserGroupInfo
    {
        public int Id { get; set; }

        public string GroupName { get; set; }

        public string AdminName { get; set; }
    }
}
