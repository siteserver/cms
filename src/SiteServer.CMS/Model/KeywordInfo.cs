using Datory;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
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
