using Dapper.Contrib.Extensions;
using SiteServer.CMS.Provider;

namespace SiteServer.CMS.Model
{
    [Table(UserMenuDao.DatabaseTableName)]
    public class UserMenuInfo
    {
        public int Id { get; set; }

        public string SystemId { get; set; }

        public string GroupIdCollection { get; set; }

        public bool IsDisabled { get; set; }

        public int ParentId { get; set; }

        public int Taxis { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string IconClass { get; set; }

        public bool IsOpenWindow { get; set; }
    }
}
