using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentStarSet : BasePageCms
    {
		protected TextBox TotalCount;
        protected TextBox PointAverage;

        private int _channelId;
        private int _contentId;

        public static string GetOpenWindowString(int publishmentSystemId, int channelId, int contentId)
        {
            return PageUtils.GetOpenWindowString("内容评分设置", PageUtils.GetCmsUrl(nameof(ModalContentStarSet), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ChannelID", channelId.ToString()},
                {"ContentID", contentId.ToString()}
            }), 350, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = Body.GetQueryInt("ChannelID");
            _contentId = Body.GetQueryInt("ContentID");

			if (!IsPostBack)
			{
                var totalCountAndPointAverage = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(PublishmentSystemId, _contentId);
                var settingTotalCount = (int)totalCountAndPointAverage[0];
                var settingPointAverage = (decimal)totalCountAndPointAverage[1];

                if (settingTotalCount > 0 || settingPointAverage > 0)
                {
                    TotalCount.Text = settingTotalCount.ToString();
                    PointAverage.Text = settingPointAverage.ToString();
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            try
            {
                var totalCount = TranslateUtils.ToInt(TotalCount.Text);
                var pointAverage = TranslateUtils.ToDecimal(PointAverage.Text);
                if (totalCount == 0)
                {
                    pointAverage = 0;
                }
                else if (pointAverage == 0)
                {
                    totalCount = 0;
                }

                StarsManager.SetStarSetting(PublishmentSystemId, _channelId, _contentId, totalCount, pointAverage);

                Body.AddSiteLog(PublishmentSystemId, _channelId, _contentId, "设置内容评分值", string.Empty);

                isChanged = true;
            }
            catch(Exception ex)
            {
                FailMessage(ex, "评分设置失败！");
            }

			if (isChanged)
			{
				PageUtils.CloseModalPage(Page);
			}
		}
	}
}
