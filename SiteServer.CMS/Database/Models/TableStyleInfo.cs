using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Extends;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_TableStyle")]
    public class TableStyleInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

	    public int RelatedIdentity { get; set; }

	    public string TableName { get; set; }

	    public string AttributeName { get; set; }

	    public int Taxis { get; set; }

	    public string DisplayName { get; set; }

	    public string HelpText { get; set; }

        private string IsVisibleInList { get; set; }

        [Computed]
        public bool VisibleInList
        {
            get => TranslateUtils.ToBool(IsVisibleInList);
            set => IsVisibleInList = value.ToString();
        }

        private string InputType { get; set; }

        [Computed]
        public InputType Type
        {
            get => InputTypeUtils.GetEnumType(InputType);
            set => InputType = value.Value;
        }

        public string DefaultValue { get; set; }

        private string IsHorizontal { get; set; }

        [Computed]
        public bool Horizontal
        {
            get => TranslateUtils.ToBool(IsHorizontal);
            set => IsHorizontal = value.ToString();
        }

        [Text]
        private string ExtendValues { get; set; }

        private TableStyleInfoExtend _extend;

        [JsonIgnore]
        [Computed]
        public TableStyleInfoExtend Extend => _extend ?? (_extend = new TableStyleInfoExtend(this));

        public string GetExtendValues()
        {
            return ExtendValues;
        }

        public void SetExtendValues(string json)
        {
            ExtendValues = json;
            _extend = null;
        }

        [Computed]
        public List<TableStyleItemInfo> StyleItems { get; set; }
	}
}
