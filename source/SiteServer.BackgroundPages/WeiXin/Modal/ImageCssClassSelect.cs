using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;

using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class ImageCssClassSelect : BackgroundBasePage
    {
        public Literal ltlScript;

        private string jsMethod;
        private int itemIndex;
        
        public static string GetOpenWindowString(int publishmentSystemID, string jsMethod)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("jsMethod", jsMethod);
            return PageUtilityWX.GetOpenWindowString("选择导航图标", "modal_imageCssClassSelect.aspx", arguments, true);
        }

        public static string GetOpenWindowStringByItemIndex(int publishmentSystemID, string jsMethod, string itemIndex)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("jsMethod", jsMethod);
            arguments.Add("itemIndex", itemIndex);
            return PageUtilityWX.GetOpenWindowString("选择导航图标", "modal_imageCssClassSelect.aspx", arguments, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            jsMethod = Request.QueryString["jsMethod"];
            itemIndex = TranslateUtils.ToInt(Request.QueryString["itemIndex"]);

            ltlScript.Text = $@"
            $(""a"").click(function () {{
               var cssClass = $(this).children().first().attr('class');
               window.parent.{jsMethod}({itemIndex}, cssClass);
               {JsUtils.OpenWindow.HIDE_POP_WIN}
           }});";
        }         
    }
}
