using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class KeywordInfo
    {
        public KeywordInfo()
        {
            KeywordId = 0;
            Keyword = string.Empty;
            Alternative = string.Empty;
            Grade = EKeywordGrade.Normal;
        }

        public KeywordInfo(int keywordId, string keyword, string alternative, EKeywordGrade grade)
        {
            KeywordId = keywordId;
            Keyword = keyword;
            Alternative = alternative;
            Grade = grade;
        }

        public int KeywordId { get; set; }

        public string Keyword { get; set; }

        public string Alternative { get; set; }

        public EKeywordGrade Grade { get; set; }
    }
}
