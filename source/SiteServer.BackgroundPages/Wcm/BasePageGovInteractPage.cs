using System;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class BasePageGovInteractPage : BasePageGovInteract
    {
        public Literal ltlTitle;
        public Literal ltlApplyAttributes;
        public Literal ltlAddDate;
        public Literal ltlQueryCode;
        public Literal ltlState;
        public Literal ltlDepartmentName;

        public PlaceHolder phReply;
        public Literal ltlDepartmentAndUserName;
        public Literal ltlReplyAddDate;
        public Literal ltlReply;
        public Literal ltlReplyFileUrl;

        public PlaceHolder phBtnAccept;
        public PlaceHolder phBtnSwitchToTranslate;
        public PlaceHolder phBtnReply;
        public PlaceHolder phBtnCheck;
        public PlaceHolder phBtnComment;
        public PlaceHolder phBtnReturn;

        public TextBox tbReply;
        public HtmlInputFile htmlFileUrl;
        public TextBox tbSwitchToRemark;
        public HtmlControl divAddDepartment;
        public Literal ltlScript;
        public DropDownList ddlTranslateNodeID;
        public TextBox tbTranslateRemark;
        public TextBox tbCommentRemark;

        public PlaceHolder phRemarks;
        public Repeater rptRemarks;
        public Repeater rptLogs;

        protected GovInteractContentInfo contentInfo;
        private string returnUrl;

        protected override bool IsSinglePage => true;

        public string MyDepartment => DepartmentManager.GetDepartmentName(Body.AdministratorInfo.DepartmentId);

        public string MyDisplayName => Body.AdministratorInfo.DisplayName;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ContentID", "ReturnUrl");

            contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(PublishmentSystemInfo, TranslateUtils.ToInt(Request.QueryString["ContentID"]));
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);

            if (!IsPostBack)
            {
                if (phBtnAccept != null)
                {
                    phBtnAccept.Visible = GovInteractManager.IsPermission(PublishmentSystemId, contentInfo.NodeId, AppManager.Wcm.Permission.GovInteract.GovInteractAccept);
                }
                if (phBtnSwitchToTranslate != null)
                {
                    phBtnSwitchToTranslate.Visible = GovInteractManager.IsPermission(PublishmentSystemId, contentInfo.NodeId, AppManager.Wcm.Permission.GovInteract.GovInteractSwitchToTranslate);
                }
                if (phBtnReply != null)
                {
                    phBtnReply.Visible = GovInteractManager.IsPermission(PublishmentSystemId, contentInfo.NodeId, AppManager.Wcm.Permission.GovInteract.GovInteractReply);
                }
                if (phBtnCheck != null)
                {
                    if (contentInfo.State == EGovInteractState.Checked)
                    {
                        phBtnCheck.Visible = false;
                    }
                    else
                    {
                        phBtnCheck.Visible = GovInteractManager.IsPermission(PublishmentSystemId, contentInfo.NodeId, AppManager.Wcm.Permission.GovInteract.GovInteractCheck);
                    }
                }
                if (phBtnComment != null)
                {
                    if (contentInfo.State == EGovInteractState.Checked)
                    {
                        phBtnComment.Visible = false;
                    }
                    else
                    {
                        phBtnComment.Visible = GovInteractManager.IsPermission(PublishmentSystemId, contentInfo.NodeId, AppManager.Wcm.Permission.GovInteract.GovInteractComment);
                    }
                }
                if (phBtnReturn != null)
                {
                    phBtnReturn.Visible = !PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow;
                }

                var builder = new StringBuilder();
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.GovInteractContent, PublishmentSystemInfo.AuxiliaryTableForGovInteract, RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, contentInfo.NodeId));

                var isPreviousSingleLine = true;
                var isPreviousLeftColumn = false;
                foreach (var styleInfo in styleInfoList)
                {
                    if (styleInfo.IsVisible && !StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, GovInteractContentAttribute.DepartmentId))
                    {
                        var value = contentInfo.GetExtendedAttribute(styleInfo.AttributeName);
                        if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, GovInteractContentAttribute.TypeId))
                        {
                            value = GovInteractManager.GetTypeName(TranslateUtils.ToInt(value));
                        }
                        else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, GovInteractContentAttribute.IsPublic))
                        {
                            value = TranslateUtils.ToBool(value) ? "公开" : "不公开";
                        }
                        else if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, GovInteractContentAttribute.FileUrl))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                value =
                                    $@"<a href=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, value)}"" target=""_blank"">{value}</a>";
                            }
                        }

                        if (builder.Length > 0)
                        {
                            if (isPreviousSingleLine)
                            {
                                builder.Append("</tr>");
                            }
                            else
                            {
                                if (!isPreviousLeftColumn)
                                {
                                    builder.Append("</tr>");
                                }
                                else if (styleInfo.IsSingleLine)
                                {
                                    builder.Append(@"<td bgcolor=""#f0f6fc"" class=""attribute""></td><td></td></tr>");
                                }
                            }
                        }

                        //this line

                        if (styleInfo.IsSingleLine || isPreviousSingleLine || !isPreviousLeftColumn)
                        {
                            builder.Append("<tr>");
                        }

                        builder.Append(
                            $@"<td bgcolor=""#f0f6fc"" class=""attribute"">{styleInfo.DisplayName}</td><td {(styleInfo
                                .IsSingleLine
                                ? @"colspan=""3"""
                                : string.Empty)} class=""tdBorder"">{value}</td>");


                        if (styleInfo.IsSingleLine)
                        {
                            isPreviousSingleLine = true;
                            isPreviousLeftColumn = false;
                        }
                        else
                        {
                            isPreviousSingleLine = false;
                            isPreviousLeftColumn = !isPreviousLeftColumn;
                        }
                    }
                }

                if (builder.Length > 0)
                {
                    if (isPreviousSingleLine || !isPreviousLeftColumn)
                    {
                        builder.Append("</tr>");
                    }
                    else
                    {
                        builder.Append(@"<td bgcolor=""#f0f6fc"" class=""attribute""></td><td></td></tr>");
                    }
                }
                ltlTitle.Text = contentInfo.Title;
                ltlApplyAttributes.Text = builder.ToString();
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
                ltlQueryCode.Text = contentInfo.QueryCode;
                ltlState.Text = EGovInteractStateUtils.GetText(contentInfo.State);
                ltlDepartmentName.Text = contentInfo.DepartmentName;

                if (phReply != null)
                {
                    if (contentInfo.State == EGovInteractState.Denied || contentInfo.State == EGovInteractState.Replied || contentInfo.State == EGovInteractState.Redo || contentInfo.State == EGovInteractState.Checked)
                    {
                        var replyInfo = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(PublishmentSystemId, contentInfo.Id);
                        if (replyInfo != null)
                        {
                            phReply.Visible = true;
                            ltlDepartmentAndUserName.Text =
                                $"{DepartmentManager.GetDepartmentName(replyInfo.DepartmentID)}({replyInfo.UserName})";
                            ltlReplyAddDate.Text = DateUtils.GetDateAndTimeString(replyInfo.AddDate);
                            ltlReply.Text = replyInfo.Reply;
                            if (!string.IsNullOrEmpty(replyInfo.FileUrl))
                            {
                                ltlReplyFileUrl.Text =
                                    $@"<a href=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo,
                                        replyInfo.FileUrl)}"" target=""_blank"">{replyInfo.FileUrl}</a>";
                            }
                        }
                    }
                }

                if (divAddDepartment != null)
                {
                    divAddDepartment.Attributes.Add("onclick", ModalGovInteractDepartmentSelect.GetOpenWindowString(PublishmentSystemId, contentInfo.NodeId));
                    var scriptBuilder = new StringBuilder();
                    if (contentInfo.DepartmentId > 0)
                    {
                        var departmentName = DepartmentManager.GetDepartmentName(contentInfo.DepartmentId);
                        scriptBuilder.Append(
                            $@"<script>showCategoryDepartment('{departmentName}', '{contentInfo.DepartmentId}');</script>");
                    }
                    ltlScript.Text = scriptBuilder.ToString();
                }

                if (ddlTranslateNodeID != null)
                {
                    var nodeInfoList = GovInteractManager.GetNodeInfoList(PublishmentSystemInfo);
                    foreach (var nodeInfo in nodeInfoList)
                    {
                        if (nodeInfo.NodeId != contentInfo.NodeId)
                        {
                            var listItem = new ListItem(nodeInfo.NodeName, nodeInfo.NodeId.ToString());
                            ddlTranslateNodeID.Items.Add(listItem);
                        }
                    }
                }

                rptRemarks.DataSource = DataProvider.GovInteractRemarkDao.GetDataSourceByContentId(PublishmentSystemId, contentInfo.Id);
                rptRemarks.ItemDataBound += rptRemarks_ItemDataBound;
                rptRemarks.DataBind();

                if (rptLogs != null)
                {
                    rptLogs.DataSource = DataProvider.GovInteractLogDao.GetDataSourceByContentId(PublishmentSystemId, contentInfo.Id);
                    rptLogs.ItemDataBound += rptLogs_ItemDataBound;
                    rptLogs.DataBind();
                }
            }
        }

        public void Reply_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbReply.Text))
            {
                FailMessage("回复失败，必须填写答复内容");
                return;
            }
            try
            {
                DataProvider.GovInteractReplyDao.DeleteByContentId(PublishmentSystemId, contentInfo.Id);
                var fileUrl = UploadFile(htmlFileUrl.PostedFile);
                var replyInfo = new GovInteractReplyInfo(0, PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, tbReply.Text, fileUrl, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovInteractReplyDao.Insert(replyInfo);

                GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Reply, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                if (Body.AdministratorInfo.DepartmentId > 0)
                {
                    DataProvider.GovInteractContentDao.UpdateDepartmentId(PublishmentSystemInfo, contentInfo.Id, Body.AdministratorInfo.DepartmentId);
                }
                DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentInfo.Id, EGovInteractState.Replied);

                SuccessMessage("办件回复成功");
                if (!PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow)
                {
                    AddWaitAndRedirectScript(ListPageUrl);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }

        private string UploadFile(HttpPostedFile myFile)
        {
            var fileUrl = string.Empty;

            if (myFile != null && !string.IsNullOrEmpty(myFile.FileName))
            {
                var filePath = myFile.FileName;
                try
                {
                    var fileExtName = PathUtils.GetExtension(filePath);
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                    var localFileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, filePath);

                    var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                    if (!PathUtility.IsFileExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                    {
                        return string.Empty;
                    }
                    if (!PathUtility.IsFileSizeAllowed(PublishmentSystemInfo, myFile.ContentLength))
                    {
                        return string.Empty;
                    }

                    myFile.SaveAs(localFilePath);
                    FileUtility.AddWaterMark(PublishmentSystemInfo, localFilePath);

                    fileUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                    fileUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, fileUrl);
                }
                catch { }
            }

            return fileUrl;
        }

        public void SwitchTo_OnClick(object sender, EventArgs e)
        {
            var switchToDepartmentID = TranslateUtils.ToInt(Request.Form["switchToDepartmentID"]);
            if (switchToDepartmentID == 0)
            {
                FailMessage("转办失败，必须选择转办部门");
                return;
            }
            var switchToDepartmentName = DepartmentManager.GetDepartmentName(switchToDepartmentID);
            try
            {
                DataProvider.GovInteractContentDao.UpdateDepartmentId(PublishmentSystemInfo, contentInfo.Id, switchToDepartmentID);

                var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractRemarkType.SwitchTo, tbSwitchToRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovInteractRemarkDao.Insert(remarkInfo);

                GovInteractApplyManager.LogSwitchTo(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, switchToDepartmentName, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);

                SuccessMessage("办件转办成功");
                if (!PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow)
                {
                    AddWaitAndRedirectScript(ListPageUrl);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }

        public void Translate_OnClick(object sender, EventArgs e)
        {
            var translateNodeID = TranslateUtils.ToInt(ddlTranslateNodeID.SelectedValue);
            if (translateNodeID == 0)
            {
                FailMessage("转移失败，必须选择转移目标");
                return;
            }
            try
            {
                contentInfo.SetExtendedAttribute(GovInteractContentAttribute.TranslateFromNodeId, contentInfo.NodeId.ToString());
                contentInfo.NodeId = translateNodeID;
                DataProvider.ContentDao.Update(PublishmentSystemInfo.AuxiliaryTableForGovInteract, PublishmentSystemInfo, contentInfo);

                if (!string.IsNullOrEmpty(tbTranslateRemark.Text))
                {
                    var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractRemarkType.Translate, tbTranslateRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                    DataProvider.GovInteractRemarkDao.Insert(remarkInfo);
                }

                GovInteractApplyManager.LogTranslate(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, NodeManager.GetNodeName(PublishmentSystemId, contentInfo.NodeId), Body.AdministratorName, Body.AdministratorInfo.DepartmentId);

                SuccessMessage("办件转移成功");
                if (!PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow)
                {
                    AddWaitAndRedirectScript(ListPageUrl);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }

        public void Comment_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbCommentRemark.Text))
                {
                    FailMessage("批示失败，必须填写意见");
                    return;
                }

                var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractRemarkType.Comment, tbCommentRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovInteractRemarkDao.Insert(remarkInfo);

                GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Comment, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);

                SuccessMessage("办件批示成功");
                if (!PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow)
                {
                    AddWaitAndRedirectScript(ListPageUrl);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }

        void rptRemarks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlRemarkType = e.Item.FindControl("ltlRemarkType") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlDepartmentAndUserName = e.Item.FindControl("ltlDepartmentAndUserName") as Literal;
                var ltlRemark = e.Item.FindControl("ltlRemark") as Literal;

                var departmentID = SqlUtils.EvalInt(e.Item.DataItem, "DepartmentID");
                var userName = SqlUtils.EvalString(e.Item.DataItem, "UserName");
                var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate");
                var remarkType = EGovInteractRemarkTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "RemarkType"));
                var remark = SqlUtils.EvalString(e.Item.DataItem, "Remark");

                if (string.IsNullOrEmpty(remark))
                {
                    e.Item.Visible = false;
                }
                else
                {
                    phRemarks.Visible = true;
                    ltlRemarkType.Text = EGovInteractRemarkTypeUtils.GetText(remarkType);
                    ltlAddDate.Text = DateUtils.GetDateAndTimeString(addDate);
                    ltlDepartmentAndUserName.Text = $"{DepartmentManager.GetDepartmentName(departmentID)}({userName})";
                    ltlRemark.Text = remark;
                }
            }
        }

        void rptLogs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlDepartment = e.Item.FindControl("ltlDepartment") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlSummary = e.Item.FindControl("ltlSummary") as Literal;

                var departmentID = SqlUtils.EvalInt(e.Item.DataItem, "DepartmentID");
                var userName = SqlUtils.EvalString(e.Item.DataItem, "UserName");
                var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate");
                var summary = SqlUtils.EvalString(e.Item.DataItem, "Summary");

                if (departmentID > 0)
                {
                    ltlDepartment.Text = DepartmentManager.GetDepartmentName(departmentID);
                }
                ltlUserName.Text = userName;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(addDate);
                ltlSummary.Text = summary;
            }
        }

        public string ListPageUrl => returnUrl;
    }
}
