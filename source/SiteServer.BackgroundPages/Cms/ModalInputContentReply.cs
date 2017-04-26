using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalInputContentReply : BasePageCms
    {
        protected Repeater rptContents;
        protected UEditor breReply;

        private int _contentId;
        private List<int> _relatedIdentities;
        private InputInfo _inputInfo;
        private InputContentInfo _contentInfo;

        public static string GetOpenWindowString(int publishmentSystemId, int inputId, int contentId)
        {
            return PageUtils.GetOpenWindowString("回复信息", PageUtils.GetCmsUrl(nameof(ModalInputContentReply), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"ContentID", contentId.ToString()}
            }));
        }
	
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "InputID", "ContentID");

            var inputId = Body.GetQueryInt("InputID");
            _contentId = Body.GetQueryInt("ContentID");
            _relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, PublishmentSystemId, inputId);

            _inputInfo = DataProvider.InputDao.GetInputInfo(inputId);

            _contentInfo = DataProvider.InputContentDao.GetContentInfo(_contentId);

            if (!Page.IsPostBack)
            {
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, _relatedIdentities);

                rptContents.DataSource = styleInfoList;
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();

                breReply.Text = _contentInfo.Reply;
            }
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var styleInfo = e.Item.DataItem as TableStyleInfo;

                var dataValue = _contentInfo.GetExtendedAttribute(styleInfo.AttributeName);
                dataValue = InputParserUtility.GetContentByTableStyle(dataValue, PublishmentSystemInfo, ETableStyle.InputContent, styleInfo);

                var ltlDataKey = e.Item.FindControl("ltlDataKey") as Literal;
                var ltlDataValue = e.Item.FindControl("ltlDataValue") as Literal;

                if (ltlDataKey != null) ltlDataKey.Text = styleInfo.DisplayName;
                if (ltlDataValue != null) ltlDataValue.Text = dataValue;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                _contentInfo.Reply = breReply.Text;
                DataProvider.InputContentDao.Update(_contentInfo);
                Body.AddSiteLog(PublishmentSystemId, "回复提交表单内容", $"提交表单:{_inputInfo.InputName},回复:{breReply.Text}");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                PageUtils.CloseModalPage(Page);
            }
        }
	}
}
