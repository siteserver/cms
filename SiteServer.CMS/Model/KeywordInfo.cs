using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class KeywordInfo
    {
        public KeywordInfo()
        {
            Id = 0;
            Keyword = string.Empty;
            Alternative = string.Empty;
            Grade = EKeywordGrade.Normal;
        }

        public KeywordInfo(int id, string keyword, string alternative, EKeywordGrade grade)
        {
            Id = id;
            Keyword = keyword;
            Alternative = alternative;
            Grade = grade;
        }

        public int Id { get; set; }

        public string Keyword { get; set; }

        public string Alternative { get; set; }

        public EKeywordGrade Grade { get; set; }
    }
}
