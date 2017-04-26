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
	public class ModalGovPublicApplyFlow : BasePageCms
	{
        public Literal ltlFlows;

        private int _applyId;

        public static string GetOpenWindowString(int publishmentSystemId, int applyId)
        {
            return PageUtils.GetOpenWindowString("流动轨迹", PageUtils.GetWcmUrl(nameof(ModalGovPublicApplyFlow), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ApplyID", applyId.ToString()}
            }), 300, 600, true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _applyId = TranslateUtils.ToInt(Request.QueryString["ApplyID"]);

			if (!IsPostBack)
			{
                if (_applyId > 0)
                {
                    var logInfoArrayList = DataProvider.GovPublicApplyLogDao.GetLogInfoArrayList(_applyId);
                    var builder = new StringBuilder();

                    var count = logInfoArrayList.Count;
                    var i = 1;
                    foreach (GovPublicApplyLogInfo logInfo in logInfoArrayList)
                    {
                        if (logInfo.DepartmentID > 0)
                        {
                            builder.Append(
                                $@"<tr class=""info""><td class=""center""> {DepartmentManager.GetDepartmentName(
                                    logInfo.DepartmentID)} {EGovPublicApplyLogTypeUtils.GetText(logInfo.LogType)}<br />{DateUtils
                                    .GetDateAndTimeString(logInfo.AddDate)} </td></tr>");
                        }
                        else
                        {
                            builder.Append(
                                $@"<tr class=""info""><td class=""center""> {EGovPublicApplyLogTypeUtils.GetText(
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
