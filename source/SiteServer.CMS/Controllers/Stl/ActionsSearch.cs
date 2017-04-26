using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsSearch
    {
        public const string Route = "stl/actions/search";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int publishmentSystemId, string ajaxDivId, int pageNum, bool isHighlight, bool isRedirectSingle, bool isDefaultDisplay, string dateAttribute, ECharset charset, string template)
        {
            return $@"
{{
    publishmentSystemId: '{publishmentSystemId}',
    ajaxDivId: '{ajaxDivId}',
    pageNum: {pageNum},
    isHighlight: {isHighlight.ToString().ToLower()},
    isRedirectSingle: {isRedirectSingle.ToString().ToLower()},
    isDefaultDisplay: {isDefaultDisplay.ToString().ToLower()},
    dateAttribute: '{dateAttribute}',
    charset: '{ECharsetUtils.GetValue(charset)}',
    template: '{TranslateUtils.EncryptStringBySecretKey(template)}'
}}";
        }
    }
}