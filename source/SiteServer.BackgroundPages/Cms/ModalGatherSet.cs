using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalGatherSet : BasePageCms
    {		
		public CheckBox GatherUrlIsCollection;
		public Control GatherUrlCollectionRow;
		public TextBox GatherUrlCollection;

		public CheckBox GatherUrlIsSerialize;
		public Control GatherUrlSerializeRow;
		public TextBox GatherUrlSerialize;
		public TextBox SerializeFrom;
		public TextBox SerializeTo;
		public TextBox SerializeInterval;
		public CheckBox SerializeIsOrderByDesc;
		public CheckBox SerializeIsAddZero;
		public TextBox UrlInclude;

		protected DropDownList NodeIDDropDownList;
		protected TextBox GatherNum;

		private string _gatherRuleName;

        public static string GetOpenWindowString(int publishmentSystemId, string gatherRuleName)
        {
            return PageUtils.GetOpenWindowString("信息采集", PageUtils.GetCmsUrl(nameof(ModalGatherSet), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherRuleName", gatherRuleName}
            }), 550, 550);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "GatherRuleName");

            _gatherRuleName = Body.GetQueryString("GatherRuleName");

			if (!IsPostBack)
			{
                InfoMessage("采集名称：" + _gatherRuleName);

				var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(_gatherRuleName, PublishmentSystemId);
                GatherUrlIsCollection.Checked = gatherRuleInfo.GatherUrlIsCollection;
				GatherUrlCollection.Text = gatherRuleInfo.GatherUrlCollection;
                GatherUrlIsSerialize.Checked = gatherRuleInfo.GatherUrlIsSerialize;
				GatherUrlSerialize.Text = gatherRuleInfo.GatherUrlSerialize;
				SerializeFrom.Text = gatherRuleInfo.SerializeFrom.ToString();
				SerializeTo.Text = gatherRuleInfo.SerializeTo.ToString();
				SerializeInterval.Text = gatherRuleInfo.SerializeInterval.ToString();
                SerializeIsOrderByDesc.Checked = gatherRuleInfo.SerializeIsOrderByDesc;
                SerializeIsAddZero.Checked = gatherRuleInfo.SerializeIsAddZero;
				UrlInclude.Text = gatherRuleInfo.UrlInclude;

				GatherNum.Text = gatherRuleInfo.Additional.GatherNum.ToString();

                NodeManager.AddListItemsForAddContent(NodeIDDropDownList.Items, PublishmentSystemInfo, true, Body.AdministratorName);
                ControlUtils.SelectListItems(NodeIDDropDownList, gatherRuleInfo.NodeId.ToString());

				GatherUrl_CheckedChanged(null, null);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (GatherUrlIsCollection.Checked)
			{
				if (string.IsNullOrEmpty(GatherUrlCollection.Text))
				{
                    FailMessage("必须填写起始网页地址！");
					return;
				}
			}
			
			if (GatherUrlIsSerialize.Checked)
			{
				if (string.IsNullOrEmpty(GatherUrlSerialize.Text))
				{
					FailMessage("必须填写起始网页地址！");
					return;
				}

				if (GatherUrlSerialize.Text.IndexOf("*") == -1)
				{
					FailMessage("起始网页地址必须带有 * 字符！");
					return;
				}

				if (string.IsNullOrEmpty(SerializeFrom.Text) || string.IsNullOrEmpty(SerializeTo.Text))
				{
                    FailMessage("必须填写变动数字范围！");
					return;
				}
				else
				{
					if (TranslateUtils.ToInt(SerializeFrom.Text) < 0 || TranslateUtils.ToInt(SerializeTo.Text) < 0)
					{
                        FailMessage("变动数字范围必须大于等于0！");
						return;
					}
					if (TranslateUtils.ToInt(SerializeTo.Text) <= TranslateUtils.ToInt(SerializeFrom.Text))
					{
                        FailMessage("变动数字范围结束必须大于开始！");
						return;
					}
				}
				
				if (string.IsNullOrEmpty(SerializeInterval.Text))
				{
                    FailMessage("必须填写数字变动倍数！");
					return;
				}
				else
				{
					if (TranslateUtils.ToInt(SerializeInterval.Text) <= 0)
					{
                        FailMessage("数字变动倍数必须大于等于1！");
						return;
					}
				}
			}
			
			if(string.IsNullOrEmpty(UrlInclude.Text))
			{
                FailMessage("必须填写内容地址包含字符串！");
				return;
			}

            var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(_gatherRuleName, PublishmentSystemId);

            gatherRuleInfo.GatherUrlIsCollection = GatherUrlIsCollection.Checked;
			gatherRuleInfo.GatherUrlCollection = GatherUrlCollection.Text;
            gatherRuleInfo.GatherUrlIsSerialize = GatherUrlIsSerialize.Checked;
			gatherRuleInfo.GatherUrlSerialize = GatherUrlSerialize.Text;
			gatherRuleInfo.SerializeFrom = TranslateUtils.ToInt(SerializeFrom.Text);
			gatherRuleInfo.SerializeTo = TranslateUtils.ToInt(SerializeTo.Text);
			gatherRuleInfo.SerializeInterval = TranslateUtils.ToInt(SerializeInterval.Text);
            gatherRuleInfo.SerializeIsOrderByDesc = SerializeIsOrderByDesc.Checked;
            gatherRuleInfo.SerializeIsAddZero = SerializeIsAddZero.Checked;
			gatherRuleInfo.UrlInclude = UrlInclude.Text;

			gatherRuleInfo.NodeId = TranslateUtils.ToInt(NodeIDDropDownList.SelectedValue);
			gatherRuleInfo.Additional.GatherNum = TranslateUtils.ToInt(GatherNum.Text);
			DataProvider.GatherRuleDao.Update(gatherRuleInfo);

            PageUtils.Redirect(ModalProgressBar.GetRedirectUrlStringWithGather(PublishmentSystemId, _gatherRuleName));
		}

		public void GatherUrl_CheckedChanged(object sender, EventArgs e)
		{
			GatherUrlCollectionRow.Visible = false;
			GatherUrlSerializeRow.Visible = false;

			if (!GatherUrlIsCollection.Checked && !GatherUrlIsSerialize.Checked)
			{
                FailMessage("必须填写起始网页地址！");
				GatherUrlIsCollection.Checked = true;
			}

			if (GatherUrlIsCollection.Checked)
			{
				GatherUrlCollectionRow.Visible = true;
			}
			
			if (GatherUrlIsSerialize.Checked)
			{
				GatherUrlSerializeRow.Visible = true;
			}
		}
	}
}
