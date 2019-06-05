using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_Keyword")]
    public class KeywordInfo : Entity
    {
        [TableColumn]
        public string Keyword { get; set; }

        [TableColumn]
        public string Alternative { get; set; }

        [TableColumn]
        private string Grade { get; set; }

        public EKeywordGrade KeywordGrade
        {
            get => EKeywordGradeUtils.GetEnumType(Grade);
            set => Grade = EKeywordGradeUtils.GetValue(value);
        }
    }
}
