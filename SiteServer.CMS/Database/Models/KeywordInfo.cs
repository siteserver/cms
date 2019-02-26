using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ErrorLog")]
    public class KeywordInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Keyword { get; set; }

        public string Alternative { get; set; }

        private string Grade { get; set; }

        [Computed]
        public EKeywordGrade KeywordGrade
        {
            get => EKeywordGradeUtils.GetEnumType(Grade);
            set => Grade = EKeywordGradeUtils.GetValue(value);
        }
    }
}
