using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ContentCheck")]
    public class ContentCheckInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string TableName { get; set; }

        public int SiteId { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string UserName { get; set; }

        private string IsChecked { get; set; }

        [Computed]
        public bool Checked
        {
            get => TranslateUtils.ToBool(IsChecked);
            set => IsChecked = value.ToString();
        }

        public int CheckedLevel { get; set; }

        public DateTime? CheckDate { get; set; }

        public string Reasons { get; set; }
    }
}
