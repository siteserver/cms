using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalImageCssClassSelect : BasePageCms
    {
        public Literal LtlScript;

        private string _jsMethod;
        private int _itemIndex;
        
        public static string GetOpenWindowString(int publishmentSystemId, string jsMethod)
        {
            return PageUtils.GetOpenWindowString("选择导航图标",
                PageUtils.GetWeiXinUrl(nameof(ModalImageCssClassSelect), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"jsMethod", jsMethod}
                }), true);
        }

        public static string GetOpenWindowStringByItemIndex(int publishmentSystemId, string jsMethod, string itemIndex)
        {
            return PageUtils.GetOpenWindowString("选择导航图标",
                PageUtils.GetWeiXinUrl(nameof(ModalImageCssClassSelect), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"jsMethod", jsMethod},
                    {"itemIndex", itemIndex}
                }), true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _jsMethod = Request.QueryString["jsMethod"];
            _itemIndex = TranslateUtils.ToInt(Request.QueryString["itemIndex"]);

            LtlScript.Text = $@"
            $(""a"").click(function () {{
               var cssClass = $(this).children().first().attr('class');
               window.parent.{_jsMethod}({_itemIndex}, cssClass);
               {PageUtils.HidePopWin}
           }});";
        }         
    }
}
