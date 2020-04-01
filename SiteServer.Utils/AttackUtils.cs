using System;
using Ganss.XSS;

namespace SiteServer.Utils
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
            const string strSql = "',\\(,\\)";
            var strSqls = strSql.Split(',');
            foreach (var sql in strSqls)
            {
                if (objStr.IndexOf(sql, StringComparison.Ordinal) != -1)
                {
                    isSqlExists = true;
                    break;
                }
            }
            if (isSqlExists)
            {
                return objStr.Replace("'", "_sqlquote_").Replace("\\(", "_sqlleftparenthesis_").Replace("\\)", "_sqlrightparenthesis_");
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
