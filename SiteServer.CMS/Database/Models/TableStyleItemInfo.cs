using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_TableStyleItem")]
    public class TableStyleItemInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int TableStyleId { get; set; }

	    public string ItemTitle { get; set; }

	    public string ItemValue { get; set; }

	    private string IsSelected { get; set; }

        [Computed]
        public bool Selected
        {
            get => TranslateUtils.ToBool(IsSelected);
            set => IsSelected = value.ToString();
        }
    }
}
