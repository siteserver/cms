using Ganss.XSS;
using System;

namespace SSCMS.Utils
{
    public static class AttackUtils
    {
        private static readonly HtmlSanitizer Sanitizer = new HtmlSanitizer();

        public static string FilterSqlAndXss(string objStr)
        {
            return FilterXss(FilterSql(objStr));
        }

        public static string FilterXss(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            return Sanitizer.Sanitize(html);
        }

        public static string FilterSql(string objStr)
        {
            if (string.IsNullOrEmpty(objStr)) return string.Empty;

            var isSqlExists = false;
            const string strSql = "',\\(,\\),select ,insert,update,from ";
            var strSqls = strSql.Split(',');
            foreach (var sql in strSqls)
            {
                if (objStr.IndexOf(sql, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    isSqlExists = true;
                    break;
                }
            }
            if (isSqlExists)
            {
                objStr = StringUtils.ReplaceIgnoreCase(objStr, "'", "_sqlquote_");
                objStr = StringUtils.ReplaceIgnoreCase(objStr, "\\(", "_sqlleftparenthesis_");
                objStr = StringUtils.ReplaceIgnoreCase(objStr, "\\)", "_sqlrightparenthesis_");
                objStr = StringUtils.ReplaceIgnoreCase(objStr, "select ", "_sqlselect_ ");
                objStr = StringUtils.ReplaceIgnoreCase(objStr, "insert", "_sqlinsert_");
                objStr = StringUtils.ReplaceIgnoreCase(objStr, "update", "_sqlupdate_");
                objStr = StringUtils.ReplaceIgnoreCase(objStr, "from ", "_sqlfrom_ ");
            }
            return objStr;
        }

        public static string UnFilterSql(string objStr)
        {
            if (string.IsNullOrEmpty(objStr)) return string.Empty;

            return objStr.Replace("_sqlquote_", "'").Replace("_sqlleftparenthesis_", "\\(").Replace("_sqlrightparenthesis_", "\\)");
        }
    }
}
