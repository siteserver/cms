using System;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class BasePageGovPublicApplyToDetail : BasePageGovPublic
    {
        public Literal ltlName;
        public Literal ltlState;
        public Literal ltlType;

        public PlaceHolder phCivic;
        public Literal ltlCivicName;
        public Literal ltlCivicOrganization;
        public Literal ltlCivicCardType;
        public Literal ltlCivicCardNo;
        public Literal ltlCivicPhone;
        public Literal ltlCivicPostCode;
        public Literal ltlCivicAddress;
        public Literal ltlCivicEmail;
        public Literal ltlCivicFax;

        public PlaceHolder phOrg;
        public Literal ltlOrgName;
        public Literal ltlOrgUnitCode;
        public Literal ltlOrgLegalPerson;
        public Literal ltlOrgLinkName;
        public Literal ltlOrgPhone;
        public Literal ltlOrgPostCode;
        public Literal ltlOrgAddress;
        public Literal ltlOrgEmail;
        public Literal ltlOrgFax;

        public Literal ltlQueryCode;
        public Literal ltlTitle;
        public Literal ltlContent;
        public Literal ltlPurpose;

        public Literal ltlIsApplyFree;
        public Literal ltlProvideType;
        public Literal ltlObtainType;

        public PlaceHolder phReply;
        public Literal ltlDepartmentAndUserName;
        public Literal ltlAddDate;
        public Literal ltlReply;
        public Literal ltlFileUrl;

        public TextBox tbReply;
        public HtmlInputFile htmlFileUrl;
        public TextBox tbSwitchToRemark;
        public HtmlControl divAddDepartment;
        public Literal ltlScript;
        public TextBox tbCommentRemark;

        public PlaceHolder phRemarks;
        public Repeater rptRemarks;
        public Repeater rptLogs;

        protected GovPublicApplyInfo applyInfo;
        private string returnUrl;

        public string MyDepartment => DepartmentManager.GetDepartmentName(Body.AdministratorInfo.DepartmentId);

        public string MyDisplayName => Body.AdministratorInfo.DisplayName;

        public string ApplyDepartment
        {
            get
            {
                if (!string.IsNullOrEmpty(applyInfo.DepartmentName))
                {
                    return applyInfo.DepartmentName;
                }
                return "<<未指定>>";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ApplyID", "ReturnUrl");

            applyInfo = DataProvider.GovPublicApplyDao.GetApplyInfo(TranslateUtils.ToInt(Request.QueryString["ApplyID"]));
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);

            if (!IsPostBack)
            {
                if (!applyInfo.IsOrganization)
                {
                    ltlType.Text = "公民";
                    phCivic.Visible = true;
                    phOrg.Visible = false;
                    ltlCivicName.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicName);
                    ltlCivicOrganization.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicOrganization);
                    ltlCivicCardType.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicCardType);
                    ltlCivicCardNo.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicCardNo);
                    ltlCivicPhone.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicPhone);
                    ltlCivicPostCode.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicPostCode);
                    ltlCivicAddress.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicAddress);
                    ltlCivicEmail.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicEmail);
                    ltlCivicFax.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicFax);

                    ltlName.Text = ltlCivicName.Text;
                }
                else
                {
                    ltlType.Text = "法人/其他组织";
                    phCivic.Visible = false;
                    phOrg.Visible = true;
                    ltlOrgName.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgName);
                    ltlOrgUnitCode.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgUnitCode);
                    ltlOrgLegalPerson.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgLegalPerson);
                    ltlOrgLinkName.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgLinkName);
                    ltlOrgPhone.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgPhone);
                    ltlOrgPostCode.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgPostCode);
                    ltlOrgAddress.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgAddress);
                    ltlOrgEmail.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgEmail);
                    ltlOrgFax.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgFax);

                    ltlName.Text = ltlOrgName.Text;
                }

                ltlState.Text = EGovPublicApplyStateUtils.GetText(applyInfo.State);
                ltlQueryCode.Text = applyInfo.QueryCode;
                ltlTitle.Text = applyInfo.Title;
                ltlContent.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.Content);
                ltlPurpose.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.Purpose);
                ltlIsApplyFree.Text = TranslateUtils.ToBool(applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.Content)) ? "申请" : "不申请";
                ltlProvideType.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.ProvideType);
                ltlObtainType.Text = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.ObtainType);

                if (phReply != null)
                {
                    if (applyInfo.State == EGovPublicApplyState.Denied || applyInfo.State == EGovPublicApplyState.Replied || applyInfo.State == EGovPublicApplyState.Redo || applyInfo.State == EGovPublicApplyState.Checked)
                    {
                        var replyInfo = DataProvider.GovPublicApplyReplyDao.GetReplyInfoByApplyId(applyInfo.Id);
                        if (replyInfo != null)
                        {
                            phReply.Visible = true;
                            ltlDepartmentAndUserName.Text =
                                $"{DepartmentManager.GetDepartmentName(replyInfo.DepartmentID)}({replyInfo.UserName})";
                            ltlAddDate.Text = DateUtils.GetDateAndTimeString(replyInfo.AddDate);
                            ltlReply.Text = replyInfo.Reply;
                            ltlFileUrl.Text = replyInfo.FileUrl;
                        }
                    }
                }

                if (divAddDepartment != null)
                {
                    divAddDepartment.Attributes.Add("onclick", ModalGovPublicCategoryDepartmentSelect.GetOpenWindowString(PublishmentSystemId));
                    var scriptBuilder = new StringBuilder();
                    if (applyInfo.DepartmentId > 0)
                    {
                        var departmentName = DepartmentManager.GetDepartmentName(applyInfo.DepartmentId);
                        scriptBuilder.Append(
                            $@"<script>showCategoryDepartment('{departmentName}', '{applyInfo.DepartmentId}');</script>");
                    }
                    ltlScript.Text = scriptBuilder.ToString();
                }

                rptRemarks.DataSource = DataProvider.GovPublicApplyRemarkDao.GetDataSourceByApplyId(applyInfo.Id);
                rptRemarks.ItemDataBound += rptRemarks_ItemDataBound;
                rptRemarks.DataBind();

                if (rptLogs != null)
                {
                    rptLogs.DataSource = DataProvider.GovPublicApplyLogDao.GetDataSourceByApplyId(applyInfo.Id);
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
                DataProvider.GovPublicApplyReplyDao.DeleteByApplyId(applyInfo.Id);
                var fileUrl = string.Empty;
                var replyInfo = new GovPublicApplyReplyInfo(0, PublishmentSystemId, applyInfo.Id, tbReply.Text, fileUrl, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovPublicApplyReplyDao.Insert(replyInfo);

                GovPublicApplyManager.Log(PublishmentSystemId, applyInfo.Id, EGovPublicApplyLogType.Reply, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovPublicApplyDao.UpdateState(applyInfo.Id, EGovPublicApplyState.Replied);

                SuccessMessage("申请回复成功");

                AddWaitAndRedirectScript(ListPageUrl);
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
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
                DataProvider.GovPublicApplyDao.UpdateDepartmentId(applyInfo.Id, switchToDepartmentID);

                var remarkInfo = new GovPublicApplyRemarkInfo(0, PublishmentSystemId, applyInfo.Id, EGovPublicApplyRemarkType.SwitchTo, tbSwitchToRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovPublicApplyRemarkDao.Insert(remarkInfo);

                GovPublicApplyManager.LogSwitchTo(PublishmentSystemId, applyInfo.Id, switchToDepartmentName, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);

                SuccessMessage("申请转办成功");

                AddWaitAndRedirectScript(ListPageUrl);
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

                var remarkInfo = new GovPublicApplyRemarkInfo(0, PublishmentSystemId, applyInfo.Id, EGovPublicApplyRemarkType.Comment, tbCommentRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovPublicApplyRemarkDao.Insert(remarkInfo);

                GovPublicApplyManager.Log(PublishmentSystemId, applyInfo.Id, EGovPublicApplyLogType.Comment, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);

                SuccessMessage("申请批示成功");

                AddWaitAndRedirectScript(ListPageUrl);
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
                var remarkType = EGovPublicApplyRemarkTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "RemarkType"));
                var remark = SqlUtils.EvalString(e.Item.DataItem, "Remark");

                if (string.IsNullOrEmpty(remark))
                {
                    e.Item.Visible = false;
                }
                else
                {
                    phRemarks.Visible = true;
                    ltlRemarkType.Text = EGovPublicApplyRemarkTypeUtils.GetText(remarkType);
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
