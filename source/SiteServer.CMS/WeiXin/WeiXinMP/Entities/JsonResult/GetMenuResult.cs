using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Menu;

namespace SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult
{
    /// <summary>
    /// GetMenu返回的Json结果
    /// </summary>
    public class GetMenuResult
    {
        public ButtonGroup menu { get; set; }

        public GetMenuResult()
        {
            menu = new ButtonGroup();
        }
    }
}
