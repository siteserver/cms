using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.User;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalInputContentView : BasePageCms
    {
        protected Repeater rptContents;
        protected Literal ltlAddUserName;
        protected Literal ltlIPAddress;
        protected Literal ltlAddDate;
        protected Literal ltlReply;

        private int _contentId;
        private List<int> _relatedIdentities;
        private InputContentInfo _contentInfo;

        public static string GetOpenWindowString(int publishmentSystemId, int inputId, int contentId)
        {
            return PageUtils.GetOpenWindowString("查看信息", PageUtils.GetCmsUrl(nameof(ModalInputContentView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"ContentID", contentId.ToString()}
            }), 700, 630, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "InputID", "ContentID");

            var inputId = Body.GetQueryInt("InputID");
            _contentId = Body.GetQueryInt("ContentID");
            _relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, PublishmentSystemId, inputId);

            _contentInfo = DataProvider.InputContentDao.GetContentInfo(_contentId);
            if (!string.IsNullOrEmpty(_contentInfo.UserName))
            {
                //group_todo
                var showPopWinString = ModalUserView.GetOpenWindowString(_contentInfo.UserName);
                ltlAddUserName.Text =
                    $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{_contentInfo.UserName}</a>";
            }
            else
            {
                ltlAddUserName.Text = "匿名";
            }

            ltlIPAddress.Text = _contentInfo.IpAddress;
            ltlAddDate.Text = DateUtils.GetDateAndTimeString(_contentInfo.AddDate);
            ltlReply.Text = _contentInfo.Reply;

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, _relatedIdentities);

            rptContents.DataSource = styleInfoList;
            rptContents.ItemDataBound += rptContents_ItemDataBound;
            rptContents.DataBind();
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
                if (ltlDataValue != null) ltlDataValue.Text = dataValue.Replace("\r\n", "<br/>");
            }
        }
    }
}
