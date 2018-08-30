using System.Collections.Generic;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class UEditorUtils
    {
        public const string ConfigValues = "{allowDivTransToP: false, maximumWords:99999999}";

        public static string GetInsertHtmlScript(string attributeName, string html)
        {
            html = html.Replace("\"", "'");
            string script = $@"UE.getEditor(""{attributeName}"", {ConfigValues}).execCommand(""insertHTML"",""{html}"");";
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace(@"""", @"\""");
                script = $@"UE.getEditor(""{attributeName}"", {ConfigValues}).execCommand(""insertHTML"",""{html}"");";
            }
            return script;
        }

        public static string GetEditorInstanceScript()
        {
            return "UE";
        }

        public static string GetInsertVideoScript(string attributeName, string playUrl, string imageUrl, SiteInfo siteInfo)
        {
            if (string.IsNullOrEmpty(playUrl)) return string.Empty;

            var dict = new Dictionary<string, string>
            {
                {StlPlayer.PlayUrl, playUrl},
                {StlPlayer.IsAutoPlay, siteInfo.Additional.ConfigUEditorVideoIsAutoPlay.ToString()},
                {StlPlayer.PlayBy, siteInfo.Additional.ConfigUEditorVideoPlayBy},
                {"style", "width: 333px; height: 333px;" }
            };
            if (siteInfo.Additional.ConfigUEditorVideoIsImageUrl && !string.IsNullOrEmpty(imageUrl))
            {
                dict.Add(StlPlayer.ImageUrl, imageUrl);
            }
            if (siteInfo.Additional.ConfigUEditorVideoIsWidth)
            {
                dict.Add(StlPlayer.Width, siteInfo.Additional.ConfigUEditorVideoWidth.ToString());
            }
            if (siteInfo.Additional.ConfigUEditorVideoIsHeight)
            {
                dict.Add(StlPlayer.Height, siteInfo.Additional.ConfigUEditorVideoHeight.ToString());
            }

            return GetInsertHtmlScript(attributeName,
                $@"<img class=""siteserver-stl-player"" src=""{SiteServerAssets.GetUrl("ueditor/video-clip.png")}"" {TranslateUtils.ToAttributesString(dict)} />");
        }

        public static string GetInsertAudioScript(string attributeName, string playUrl, SiteInfo siteInfo)
        {
            if (string.IsNullOrEmpty(playUrl)) return string.Empty;

            var dict = new Dictionary<string, string>
            {
                {StlPlayer.PlayUrl, playUrl},
                {StlPlayer.IsAutoPlay, siteInfo.Additional.ConfigUEditorAudioIsAutoPlay.ToString()},
                {"style", "width: 400px; height: 40px;" }
            };

            return GetInsertHtmlScript(attributeName,
                $@"<img class=""siteserver-stl-audio"" src=""{SiteServerAssets.GetUrl("ueditor/audio-clip.png")}"" {TranslateUtils.ToAttributesString(dict)} />");
        }

        public static string GetPureTextScript(string attributeName)
        {
            string script = $@"UE.getEditor(""{attributeName}"", {ConfigValues}).getContentTxt();";
            return script;
        }

        public static string GetContentScript(string attributeName)
        {
            string script = $@"UE.getEditor(""{attributeName}"", {ConfigValues}).getContent();";
            return script;
        }

        public static string GetSetContentScript(string attributeName, string contentWithoutQuote)
        {
            string script = $@"UE.getEditor(""{attributeName}"", {ConfigValues}).setContent({contentWithoutQuote});";
            return script;
        }

        public static string TranslateToStlElement(string html)
        {
            var retval = html;
            if (!string.IsNullOrEmpty(retval))
            {
                retval = retval.Replace(@"<img class=""siteserver-stl-player"" ", "<stl:player ");
                retval = retval.Replace(@"<img class=""siteserver-stl-audio"" ", "<stl:audio ");
            }
            return retval;
        }

        public static string TranslateToHtml(string html)
        {
            var retval = html;
            if (!string.IsNullOrEmpty(retval))
            {
                retval = retval.Replace("<stl:player ", @"<img class=""siteserver-stl-player"" ");
                retval = retval.Replace("<stl:audio ", @"<img class=""siteserver-stl-audio"" ");
            }
            return retval;
        }
    }
}
