using System.Web.UI;

namespace SiteServer.Utils
{
    public static class LayerUtils
    {
        public static string OpenFull(string title, string url)
        {
            return $@"pageUtils.openLayer({{title: ""{title}"", url: ""{url}"", full: true}});return false";
        }

        public const string CloseScript = "if (window.parent.closeWindow) window.parent.closeWindow();if (window.parent.layer) window.parent.layer.closeAll();";

        public const string OpenPageCreateStatusFuncName = "openPageCreateStatus";

        public static string GetOpenScript(string title, string pageUrl)
        {
            return GetOpenScript(title, pageUrl, 0, 0);
        }

        public static string GetOpenScript2(string title, string pageUrl)
        {
            return GetOpenScript2(title, pageUrl, 0, 0);
        }

        public static string GetOpenScript(string title, string pageUrl, int width, int height)
        {
            return
                $@"utils.openLayer({{title: '{title}', url: '{pageUrl}', width: {width}, height: {height}}});return false;";
        }

        public static string GetOpenScript2(string title, string pageUrl, int width, int height)
        {
            return
                $@"utils.openLayer({{title: '{title}', url: '{pageUrl}', width: {width}, height: {height}}});return false;";
        }

        public static string GetOpenScriptWithTextBoxValue(string title, string pageUrl, string textBoxId)
        {
            return GetOpenScriptWithTextBoxValue(title, pageUrl, textBoxId, 0, 0);
        }

        public static string GetOpenScriptWithTextBoxValue(string title, string pageUrl, string textBoxId, int width, int height)
        {
            return
                $@"utils.openLayer({{title: '{title}', url: '{pageUrl}' + '&{textBoxId}=' + $('#{textBoxId}').val(), width: {width}, height: {height}}});return false;";
        }

        public static string GetOpenScriptWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText)
        {
            return GetOpenScriptWithCheckBoxValue(title, pageUrl, checkBoxId, alertText, 0, 0);
        }

        public static string GetOpenScriptWithCheckBoxValue(string title, string pageUrl, string checkBoxId, string alertText, int width, int height)
        {
            string areaWidth = $"'{width}px'";
            string areaHeight = $"'{height}px'";
            var offsetLeft = "''";
            var offsetRight = "''";
            if (width == 0)
            {
                areaWidth = "($(window).width() - 50) +'px'";
                offsetRight = "'25px'";
            }
            if (height == 0)
            {
                areaHeight = "($(window).height() - 50) +'px'";
                offsetLeft = "'25px'";
            }

            if (string.IsNullOrEmpty(alertText))
            {
                return
                    $@"utils.openLayer({{title: '{title}', url: '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}')), width: {width}, height: {height}}});return false;";
            }
            return
                $@"if (!_alertCheckBoxCollection(document.getElementsByName('{checkBoxId}'), '{alertText}')){{utils.openLayer({{title: '{title}', url: '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}')), width: {width}, height: {height}}});}};return false;";
        }

        public static string GetOpenScriptWithTwoCheckBoxValue(string title, string pageUrl, string checkBoxId1, string checkBoxId2, string alertText, int width, int height)
        {
            var offset = string.Empty;
            if (width == 0)
            {
                offset = "offset: ['0px','0px'],";
            }
            if (height == 0)
            {
                offset = "offset: ['0px','0px'],";
            }

            return
                $@"var collectionValue1 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}'));var collectionValue2 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}'));if (collectionValue1.length == 0 && collectionValue2.length == 0){{alert('{alertText}');}}else{{utils.openLayer({{title: '{title}', url: '{pageUrl}' + '&{checkBoxId1}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}')) + '&{checkBoxId2}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}')), width: {width}, height: {height}}});}};return false;";
        }

        public static void Close(Page page)
        {
            page.Response.Clear();
            page.Response.Write($"<script>window.parent.location.reload(true);{CloseScript}</script>");
        }

        public static void Close(Page page, string scripts)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{scripts}</script>");
            page.Response.Write($"<script>window.parent.location.reload(true);{CloseScript}</script>");
        }

        public static void CloseAndRedirect(Page page, string redirectUrl)
        {
            page.Response.Clear();
            page.Response.Write($"<script>window.parent.location.href = '{redirectUrl}';{CloseScript}</script>");
        }

        public static void CloseAndRedirect(Page page, string redirectUrl, string scripts)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{scripts}</script>");
            page.Response.Write($"<script>window.parent.location.href = '{redirectUrl}';{CloseScript}</script>");
        }

        public static void CloseWithoutRefresh(Page page)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{CloseScript}</script>");
        }

        public static void CloseWithoutRefresh(Page page, string scripts)
        {
            page.Response.Clear();
            page.Response.Write($"<script>{scripts}</script>");
            page.Response.Write($"<script>{CloseScript}</script>");
        }

        public static void CloseAndOpenPageCreateStatus(Page page)
        {
            CloseWithoutRefresh(page, $"window.top.{OpenPageCreateStatusFuncName}();");
        }
    }
}
