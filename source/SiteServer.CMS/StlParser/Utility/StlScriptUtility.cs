using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlScriptUtility
	{
        public class PageScript
        {
            public static string GetStlLogin(string redirectUrl)
            {
                return $"stlLogin('{StringUtils.ValueToUrl(redirectUrl)}');";
            }

            public static string GetStlOpenWindow(string pageUrl, int width, int height)
            {
                return $"stlOpenWindow('{pageUrl}', {width}, {height});";
            }

            public static string GetStlRefresh()
            {
                return "stlRefresh();";
            }

            public static string GetStlInputReplaceTextarea(string attributeName, string editorUrl, int height, int width)
            {
                return $@"stlInputReplaceTextarea('{attributeName}', '{editorUrl}', {height}, {width});";
            }

            public static string GetStlInputSubmit(string resultsPageUrl, string ajaxDivID, bool isSuccessHide, bool isSuccessReload, string checkMethod, string successTemplate, string failureTemplate)
            {
                if (string.IsNullOrEmpty(checkMethod))
                {
                    return
                        $"stlInputSubmit('{resultsPageUrl}', '{ajaxDivID}', {isSuccessHide.ToString().ToLower()}, {isSuccessReload.ToString().ToLower()}, '{successTemplate}', '{failureTemplate}');return false;";
                }
                else
                {
                    checkMethod = checkMethod.Trim().TrimEnd(';');
                    if (!checkMethod.EndsWith(")"))
                    {
                        checkMethod = checkMethod + "()";
                    }
                    return
                        $"if ({checkMethod})stlInputSubmit('{resultsPageUrl}', '{ajaxDivID}', {isSuccessHide.ToString().ToLower()}, {isSuccessReload.ToString().ToLower()}, '{successTemplate}', '{failureTemplate}');return false;";
                }
            }

            public static string GetStlInputSubmitWithUpdate(string resultsPageUrl, string ajaxDivID, bool isSuccessHide, bool isSuccessReload, string checkMethod, string successTemplate, string failureTemplate, string successCallback, string successArgument)
            {
                if (string.IsNullOrEmpty(checkMethod))
                {
                    return
                        $"stlInputSubmit('{resultsPageUrl}', '{ajaxDivID}', {isSuccessHide.ToString().ToLower()}, {isSuccessReload.ToString().ToLower()}, '{successTemplate}', '{failureTemplate}', '{successCallback}', '{successArgument}');return false;";
                }
                else
                {
                    checkMethod = checkMethod.Trim().TrimEnd(';');
                    if (!checkMethod.EndsWith(")"))
                    {
                        checkMethod = checkMethod + "()";
                    }
                    return
                        $"if ({checkMethod})stlInputSubmit('{resultsPageUrl}', '{ajaxDivID}', {isSuccessHide.ToString().ToLower()}, {isSuccessReload.ToString().ToLower()}, '{successTemplate}', '{failureTemplate}', '{successCallback}', '{successArgument}');return false;";
                }
            }
        }
	}
}
