using System.Text.RegularExpressions;

namespace BaiRong.Core.Model.Enumerations
{
    public class ETextEditorTypeUtils
    {
        public static string GetInsertHtmlScript(string attributeName, string html)
        {
            html = html.Replace("\"", "'");
            string script = $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).execCommand(""insertHTML"",""{html}"");";
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace(@"""", @"\""");
                script = $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).execCommand(""insertHTML"",""{html}"");";
            }
            return script;
        }

        public static string GetEditorInstanceScript()
        {
            return "UE";
        }

        public static string GetInsertVideoScript(string attributeName, string playUrl, int width, int height, bool isAutoPlay)
        {
            var script = string.Empty;
            if (!string.IsNullOrEmpty(playUrl))
            {
                if (width > 0 && height > 0)
                {
                    script =
                        $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).execCommand(""insertVideo"",{{url: ""{playUrl}"",width: {width},height: {height},isAutoPlay: ""{isAutoPlay
                            .ToString().ToLower()}""}});";
                }
                else
                {
                    script =
                        $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).execCommand(""insertVideo"",{{url: ""{playUrl}"",isAutoPlay: ""{isAutoPlay
                            .ToString().ToLower()}""}});";
                }
            }
            return script;
        }

        public static string GetInsertAudioScript(string attributeName, string playUrl, bool isAutoPlay)
        {
            var script = string.Empty;
            if (!string.IsNullOrEmpty(playUrl))
            {
                script =
                    $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).execCommand(""music"",{{url: ""{playUrl}"",isAutoPlay: ""{isAutoPlay
                        .ToString().ToLower()}""}});";
            }
            return script;
        }

        public static string GetPureTextScript(string attributeName)
        {
            string script = $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).getContentTxt();";
            return script;
        }

        public static string GetContentScript(string attributeName)
        {
            string script = $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).getContent();";
            return script;
        }

        public static string GetSetContentScript(string attributeName, string contentWithoutQuote)
        {
            string script = $@"UE.getEditor(""{attributeName}"", {{allowDivTransToP: false}}).setContent({contentWithoutQuote});";
            return script;
        }

        public static string TranslateToStlElement(string html)
        {
            var retval = html;
            if (!string.IsNullOrEmpty(retval))
            {
                var regex = new Regex(@"<embed[^>]*class=""edui-faked-[^>]*/>", ((RegexOptions.Singleline | RegexOptions.IgnoreCase) | RegexOptions.IgnorePatternWhitespace) | RegexOptions.Compiled);

                var mc = regex.Matches(retval);
                for (var i = 0; i < mc.Count; i++)
                {
                    var original = mc[i].Value;
                    if (original.Contains("edui-faked-video"))
                    {
                        var replace = original.Replace("embed", "stl:player");
                        retval = retval.Replace(original, replace);
                    }
                    else if (original.Contains("edui-faked-music"))
                    {
                        var replace = original.Replace("embed", "stl:audio");
                        retval = retval.Replace(original, replace);
                    }
                }

                //retval = retval.Replace(@"<embed class=""edui-faked-video"" ", "<stl:player ");
                //retval = retval.Replace(@"<embed class=""edui-faked-music"" ", "<stl:audio ");
            }
            return retval;
        }

        public static string TranslateToHtml(string html)
        {
            var retval = html;
            if (!string.IsNullOrEmpty(retval))
            {
                retval = retval.Replace("<stl:player ", @"<embed ");
                retval = retval.Replace("<stl:audio ", @"<embed ");
            }
            return retval;
        }
    }
}
