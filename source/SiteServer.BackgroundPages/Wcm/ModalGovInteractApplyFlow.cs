using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovInteractApplyFlow : BasePageCms
	{
        public Literal ltlFlows;

        private int _contentId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, int contentId)
        {
            return PageUtils.GetOpenWindowString("流动轨迹", PageUtils.GetWcmUrl(nameof(ModalGovInteractApplyFlow), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()}
            }), 300, 600, true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _contentId = TranslateUtils.ToInt(Request.QueryString["ContentID"]);

			if (!IsPostBack)
			{
                if (_contentId > 0)
                {
                    var logInfoArrayList = DataProvider.GovInteractLogDao.GetLogInfoArrayList(PublishmentSystemId, _contentId);
                    var builder = new StringBuilder();

                    var count = logInfoArrayList.Count;
                    var i = 1;
                    foreach (GovInteractLogInfo logInfo in logInfoArrayList)
                    {
                        if (logInfo.DepartmentID > 0)
                        {
                            builder.Append(
                                $@"<tr class=""info""><td class=""center""> {DepartmentManager.GetDepartmentName(
                                    logInfo.DepartmentID)} {EGovInteractLogTypeUtils.GetText(logInfo.LogType)}<br />{DateUtils
                                    .GetDateAndTimeString(logInfo.AddDate)} </td></tr>");
                        }
                        else
                        {
                            builder.Append(
                                $@"<tr class=""info""><td class=""center""> {EGovInteractLogTypeUtils.GetText(
                                    logInfo.LogType)}<br />{DateUtils.GetDateAndTimeString(logInfo.AddDate)} </td></tr>");
                        }
                        if (i++ < count) builder.Append(@"<tr><td class=""center""><img src=""../pic/flow.gif"" /></td></tr>");
                    }
                    ltlFlows.Text = builder.ToString();
                }

				
			}
		}
	}
}
