using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Template")]
    public class TemplateInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

		public int SiteId { get; set; }

        public string TemplateName { get; set; }

        private string TemplateType { get; set; }

        [Computed]
        public TemplateType Type
        {
            get => TemplateTypeUtils.GetEnumType(TemplateType);
            set => TemplateType = value.Value;
        }

        public string RelatedFileName { get; set; }

        public string CreatedFileFullName { get; set; }

        public string CreatedFileExtName { get; set; }

        private string Charset { get; set; }

        [Computed]
        public ECharset FileCharset
        {
            get => ECharsetUtils.GetEnumType(Charset);
            set => Charset = ECharsetUtils.GetValue(value);
        }

        private string IsDefault { get; set; }

        [Computed]
        public bool Default
        {
            get => TranslateUtils.ToBool(IsDefault);
            set => IsDefault = value.ToString();
        }

        [Computed]
        public string Content { get; set; }
    }
}
