using System;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAppointmentItemDelete : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        { 
            var list = TranslateUtils.StringCollectionToIntList(Request["IDCollection"]);
            if (list.Count > 0)
            {
                try
                {
                    DataProviderWX.AppointmentItemDAO.Delete(PublishmentSystemId, list);
                    Response.Write("success");
                    Response.End();
                }
                catch (Exception ex)
                {
                    Response.Write("failure");
                    Response.End();
                }
            }
        }
    }
}
