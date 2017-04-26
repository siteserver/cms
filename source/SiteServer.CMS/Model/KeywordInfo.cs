using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class KeywordInfo
    {
        private int keywordID;
        private string keyword;
        private string alternative;
        private EKeywordGrade grade;

        public KeywordInfo()
        {
            keywordID = 0;
            keyword = string.Empty;
            alternative = string.Empty;
            grade = EKeywordGrade.Normal;
        }

        public KeywordInfo(int keywordID, string keyword, string alternative, EKeywordGrade grade)
        {
            this.keywordID = keywordID;
            this.keyword = keyword;
            this.alternative = alternative;
            this.grade = grade;
        }

        public int KeywordID
        {
            get { return keywordID; }
            set { keywordID = value; }
        }

        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }

        public string Alternative
        {
            get { return alternative; }
            set { alternative = value; }
        }

        public EKeywordGrade Grade
        {
            get { return grade; }
            set { grade = value; }
        }
    }
}
