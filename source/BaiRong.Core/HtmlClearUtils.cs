using System.Text;
using System.Text.RegularExpressions;

namespace BaiRong.Core
{
    public class HtmlClearUtils
    {
        /// <summary>
        /// 替换新闻内容中的Html标签的多余属性
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearFormat(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            //清理word标签，如o:p之类，带冒号的
            html = ClearWordStyle(html);
            var el = new string[] { "p", "strong", "ol", "ul", "li", "table", "tr", "td" };
            foreach (var s in el)
            {
                try
                {
                    html = ClearElementAttributes(html, s);
                }
                catch
                {
                    continue;
                }
            }
            //清除样式
            el = new string[] { "div", "span", "font", "h1", "h2", "h3", "h4", "h5", "h6", "tbody" };
            foreach (var s in el)
            {
                try
                {
                    html = RemoveElementTag(html, s);
                }
                catch
                {
                    continue;
                }
            }
            return html;
        }

        public static string FirstLineIndent(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            html = html.Replace("<p>　　", "<p>");
            html = html.Replace("<p>", @"<p>　　");
            return html;
        }

        public static string ClearFontSize(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            html = RegexUtils.Replace(@"font-size:\w+;", html, string.Empty);
            return html;
        }

        public static string ClearFontFamily(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            html = RegexUtils.Replace(@"font-family:'\w+';", html, string.Empty);
            html = RegexUtils.Replace(@"font-family:\w+;", html, string.Empty);
            return html;
        }

        //

        /// <summary>
        /// 清理HTML标签的多余样式；如<div style="color:#454353">示例</div>;换成<div>示例</div>
        /// </summary>
        /// <param name="str">原始文本</param>
        /// <param name="element">要清除的标签</param>
        /// <returns></returns>
        //private static string ClearElementAttributes(string str, string element)
        //{
        //    string old = @"<" + element + "[^>]+>";
        //    string rep = "<" + element + ">";
        //    str = Regex.Replace(str, old, rep, RegexOptions.IgnoreCase);
        //    return str;
        //}

        public static string ClearElementAttributes(string str, string el)
        {
            var sb = new StringBuilder(str);
            //第一次匹配，查出还标签的文本，如<p style=''>内部文本</p>
            var pat = @"(?=<(" + el + @")[^>]+>).*(?<=<\/\1>)";
            var re = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var m = re.Match(sb.ToString());
            while (m.Success)
            {
                //第三次匹配，当段落内不含有图片时，则清理冗余代码；
                var pEl = @"<" + el + "[^>]+>";
                var tm = m.Value.ToString();
                tm = Regex.Replace(tm, pEl, "<" + el + ">");
                sb = sb.Replace(m.Value.ToString(), tm);

                m = m.NextMatch();
            }
            return sb.ToString();
        }


        /// <summary>
        /// 清除HTML标签；如<div style="color:#454353">示例</div>;换成：示例
        /// </summary>
        /// <param name="str">原始文本</param>
        /// <param name="element">要清除的标签</param>
        /// <returns></returns>
        private static string RemoveElementTag(string str, string element)
        {
            var regFront = @"<" + element + "[^>]*>";
            var regAfter = "</" + element + ">";
            str = Regex.Replace(str, regFront, "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, regAfter, "", RegexOptions.IgnoreCase);
            return str;
        }
        ///// <summary>
        ///// 清理指定字符串，大小写不敏感
        ///// </summary>
        ///// <param name="strText">原始文本</param>
        ///// <param name="strOld">要替换的字符串，支持正则表达式，大小写不敏感</param>
        ///// <param name="strNew">替换后的字符串</param>
        ///// <returns></returns>
        //private static string RegexReplace(string strText, string strOld, string strNew)
        //{
        //    strText = Regex.Replace(strText, strOld, strNew, RegexOptions.IgnoreCase);
        //    return strText;
        //}
        /// <summary>
        /// 清理Word的样式，主要是一些带冒号的标签，如o:p
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        private static string ClearWordStyle(string strText)
        {
            var regFront = @"<\w+:[^>]*>";
            var regAfter = @"</\w+:[^>]*>";
            strText = Regex.Replace(strText, regFront, "", RegexOptions.IgnoreCase);
            strText = Regex.Replace(strText, regAfter, "", RegexOptions.IgnoreCase);
            return strText;
        }
    }
}
