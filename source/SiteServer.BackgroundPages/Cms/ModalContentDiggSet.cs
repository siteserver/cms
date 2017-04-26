using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentDiggSet : BasePageCms
    {
		protected TextBox GoodNum;
        protected TextBox BadNum;

        private int _channelId;
        private int _contentId;

        public static string GetOpenWindowString(int publishmentSystemId, int channelId, int contentId)
        {
            return PageUtils.GetOpenWindowString("内容Digg设置", PageUtils.GetCmsUrl(nameof(ModalContentDiggSet), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ChannelID", channelId.ToString()},
                {"ContentID", contentId.ToString()}
            }), 350, 280);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = Body.GetQueryInt("ChannelID");
            _contentId = Body.GetQueryInt("ContentID");

			if (!IsPostBack)
			{
                var nums = BaiRongDataProvider.DiggDao.GetCount(PublishmentSystemId, _contentId);

                GoodNum.Text = Convert.ToString(nums[0]);
                BadNum.Text = Convert.ToString(nums[1]);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

            try
            {
                var goodNum = TranslateUtils.ToInt(GoodNum.Text);
                var badNum = TranslateUtils.ToInt(BadNum.Text);

                BaiRongDataProvider.DiggDao.SetCount(PublishmentSystemId, _contentId, goodNum, badNum);
                Body.AddSiteLog(PublishmentSystemId, _channelId, _contentId, "设置内容Digg值", string.Empty);
                isChanged = true;
            }
            catch(Exception ex)
            {
                FailMessage(ex, "Digg设置失败！");
            }

			if (isChanged)
			{
				PageUtils.CloseModalPage(Page);
			}
		}
	}
}
