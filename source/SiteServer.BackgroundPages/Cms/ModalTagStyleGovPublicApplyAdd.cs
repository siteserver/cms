using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using System.Collections.Specialized;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTagStyleGovPublicApplyAdd : BasePageCms
    {
        protected TextBox StyleName;

        protected RadioButtonList IsSMS;
        protected PlaceHolder phSMS;
        public TextBox SMSTo;
        public TextBox SMSTitle;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加依申请公开提交样式",
                PageUtils.GetCmsUrl(nameof(ModalTagStyleGovPublicApplyAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                }), 560, 420);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int styleId)
        {
            return PageUtils.GetOpenWindowString("修改依申请公开提交样式", PageUtils.GetCmsUrl(nameof(ModalTagStyleGovPublicApplyAdd), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"StyleID", styleId.ToString()}
                }), 560, 420);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                if (Body.IsQueryExists("StyleID"))
                {
                    var styleId = Body.GetQueryInt("StyleID");
                    var styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleId);
                    if (styleInfo != null)
                    {
                        var applyInfo = new TagStyleGovPublicApplyInfo(styleInfo.SettingsXML);
                        StyleName.Text = styleInfo.StyleName;

                        ControlUtils.SelectListItems(IsSMS, applyInfo.IsSMS.ToString());
                        if (applyInfo.IsSMS)
                        {
                            phSMS.Visible = true;
                        }
                        SMSTo.Text = applyInfo.SMSTo;
                        SMSTitle.Text = applyInfo.SMSTitle;
                    }
                }
				
			}
		}

        public void IsSMS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                phSMS.Visible = TranslateUtils.ToBool(IsSMS.SelectedValue);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
            TagStyleInfo styleInfo;
				
			if (Body.IsQueryExists("StyleID"))
			{
				try
				{
                    var styleId = Body.GetQueryInt("StyleID");
                    styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleId);
                    if (styleInfo != null)
                    {
                        var applyInfo = new TagStyleGovPublicApplyInfo(styleInfo.SettingsXML);

                        styleInfo.StyleName = StyleName.Text;

                        applyInfo.IsSMS = TranslateUtils.ToBool(IsSMS.SelectedValue);
                        applyInfo.SMSTo = SMSTo.Text;
                        applyInfo.SMSTitle = SMSTitle.Text;

                        styleInfo.SettingsXML = applyInfo.ToString();
                    }
                    DataProvider.TagStyleDao.Update(styleInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改依申请公开提交样式", $"样式名称:{styleInfo.StyleName}");

					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "依申请公开提交样式修改失败！");
				}
			}
			else
			{
                var styleNameArrayList = DataProvider.TagStyleDao.GetStyleNameArrayList(PublishmentSystemId, StlGovPublicApply.ElementName);
                if (styleNameArrayList.IndexOf(StyleName.Text) != -1)
				{
                    FailMessage("依申请公开提交样式添加失败，依申请公开提交样式名称已存在！");
				}
				else
				{
					try
					{
                        styleInfo = new TagStyleInfo();
                        var applyInfo = new TagStyleGovPublicApplyInfo(string.Empty);

                        styleInfo.StyleName = StyleName.Text;
                        styleInfo.ElementName = StlGovPublicApply.ElementName;
                        styleInfo.PublishmentSystemID = PublishmentSystemId;

                        applyInfo.IsSMS = TranslateUtils.ToBool(IsSMS.SelectedValue);
                        applyInfo.SMSTo = SMSTo.Text;
                        applyInfo.SMSTitle = SMSTitle.Text;

                        styleInfo.SettingsXML = applyInfo.ToString();

                        DataProvider.TagStyleDao.Insert(styleInfo);

                        Body.AddSiteLog(PublishmentSystemId, "添加依申请公开提交样式", $"样式名称:{styleInfo.StyleName}");

						isChanged = true;
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "依申请公开提交样式添加失败！");
					}
				}
			}

			if (isChanged)
			{
                PageUtils.CloseModalPage(Page);
			}
		}
	}
}
