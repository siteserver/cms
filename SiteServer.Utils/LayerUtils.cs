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

        public static string GetOpenScript(string title, string pageUrl, int width, int height)
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
            return
                $@"$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}'}}, area: [{areaWidth}, {areaHeight}], offset: [{offsetLeft}, {offsetRight}]}});return false;";
        }

        public static string GetOpenScriptWithTextBoxValue(string title, string pageUrl, string textBoxId)
        {
            return GetOpenScriptWithTextBoxValue(title, pageUrl, textBoxId, 0, 0);
        }

        public static string GetOpenScriptWithTextBoxValue(string title, string pageUrl, string textBoxId, int width, int height)
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
            return
                $@"$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{textBoxId}=' + $('#{textBoxId}').val()}}, area: [{areaWidth}, {areaHeight}], offset: [{offsetLeft}, {offsetRight}]}});return false;";
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
                    $@"$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}'))}}, area: [{areaWidth}, {areaHeight}], offset: [{offsetLeft}, {offsetRight}]}});return false;";
            }
            return
                $@"if (!_alertCheckBoxCollection(document.getElementsByName('{checkBoxId}'), '{alertText}')){{$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{checkBoxId}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId}'))}}, area: [{areaWidth}, {areaHeight}], offset: [{offsetLeft}, {offsetRight}]}});}};return false;";
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
                $@"var collectionValue1 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}'));var collectionValue2 = _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}'));if (collectionValue1.length == 0 && collectionValue2.length == 0){{alert('{alertText}');}}else{{$.layer({{type: 2, maxmin: true, shadeClose: true, title: '{title}', shade: [0.1,'#fff'], iframe: {{src: '{pageUrl}' + '&{checkBoxId1}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId1}')) + '&{checkBoxId2}=' + _getCheckBoxCollectionValue(document.getElementsByName('{checkBoxId2}'))}}, area: [{width}, {height}], {offset}}});}};return false;";
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
