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

        public string Text { get; set; }

        public string IconClass { get; set; }

        public string Href { get; set; }

        public string Target { get; set; }
    }
}
