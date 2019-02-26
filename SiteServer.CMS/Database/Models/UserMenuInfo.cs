using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_UserMenu")]
    public class UserMenuInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string SystemId { get; set; }

        public string GroupIdCollection { get; set; }

        private string IsDisabled { get; set; }

        [Computed]
        public bool Disabled
        {
            get => TranslateUtils.ToBool(IsDisabled);
            set => IsDisabled = value.ToString();
        }

        public int ParentId { get; set; }

        public int Taxis { get; set; }

        public string Text { get; set; }

        public string IconClass { get; set; }

        public string Href { get; set; }

        public string Target { get; set; }
    }
}
