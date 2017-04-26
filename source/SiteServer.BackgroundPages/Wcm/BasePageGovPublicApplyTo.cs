using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class BasePageGovPublicApplyTo : BasePageGovPublic
    {
        public HyperLink hlAccept;
        public HyperLink hlDeny;
        public HyperLink hlCheck;
        public HyperLink hlRedo;
        public HyperLink hlSwitchTo;
        public HyperLink hlComment;
        public PlaceHolder phDelete;
        public HyperLink hlDelete;
        public Literal ltlTotalCount;

        public Repeater rptContents;
        public SqlPager spContents;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("Delete"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (arraylist.Count > 0)
                {
                    try
                    {
                        DataProvider.GovPublicApplyDao.Delete(arraylist);
                        Body.AddSiteLog(PublishmentSystemId, "删除申请");
                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }
            else if (Body.IsQueryExists("Accept"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                foreach (var applyId in arraylist)
                {
                    var state = DataProvider.GovPublicApplyDao.GetState(applyId);
                    if (state == EGovPublicApplyState.New || state == EGovPublicApplyState.Denied)
                    {
                        GovPublicApplyManager.Log(PublishmentSystemId, applyId, EGovPublicApplyLogType.Accept, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                        DataProvider.GovPublicApplyDao.UpdateState(applyId, EGovPublicApplyState.Accepted);
                    }
                }
                SuccessMessage("受理申请成功！");
            }
            else if (Body.IsQueryExists("Deny"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                foreach (var applyId in arraylist)
                {
                    var state = DataProvider.GovPublicApplyDao.GetState(applyId);
                    if (state == EGovPublicApplyState.New || state == EGovPublicApplyState.Accepted)
                    {
                        GovPublicApplyManager.Log(PublishmentSystemId, applyId, EGovPublicApplyLogType.Deny, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                        DataProvider.GovPublicApplyDao.UpdateState(applyId, EGovPublicApplyState.Denied);
                    }
                }
                SuccessMessage("拒绝受理申请成功！");
            }
            else if (Body.IsQueryExists("Check"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                foreach (var applyId in arraylist)
                {
                    var state = DataProvider.GovPublicApplyDao.GetState(applyId);
                    if (state == EGovPublicApplyState.Replied)
                    {
                        GovPublicApplyManager.Log(PublishmentSystemId, applyId, EGovPublicApplyLogType.Check, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                        DataProvider.GovPublicApplyDao.UpdateState(applyId, EGovPublicApplyState.Checked);
                    }
                }
                SuccessMessage("审核申请成功！");
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SelectCommand = GetSelectString();
            spContents.SortField = DataProvider.GovPublicApplyDao.GetSortFieldName();
            spContents.SortMode = GetSortMode();
            rptContents.ItemDataBound +=rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                spContents.DataBind();
                ltlTotalCount.Text = spContents.TotalCount.ToString();

                if (hlAccept != null)
                {
                    hlAccept.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Accept=True", "IDCollection", "IDCollection", "请选择需要受理的申请！", "此操作将受理所选申请，确定吗？"));
                }
                if (hlDeny != null)
                {
                    hlDeny.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Deny=True", "IDCollection", "IDCollection", "请选择需要拒绝的申请！", "此操作将拒绝受理所选申请，确定吗？"));
                }
                if (hlCheck != null)
                {
                    hlCheck.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Check=True", "IDCollection", "IDCollection", "请选择需要审核的申请！", "此操作将审核所选申请，确定吗？"));
                }
                if (hlRedo != null)
                {
                    hlRedo.Attributes.Add("onclick", ModalGovPublicApplyRedo.GetOpenWindowString(PublishmentSystemId));
                }
                if (hlSwitchTo != null)
                {
                    hlSwitchTo.Attributes.Add("onclick", ModalGovPublicApplySwitchTo.GetOpenWindowString(PublishmentSystemId));
                }
                if (hlComment != null)
                {
                    hlComment.Attributes.Add("onclick", ModalGovPublicApplyComment.GetOpenWindowString(PublishmentSystemId));
                }
                if (phDelete != null)
                {
                    phDelete.Visible = PublishmentSystemInfo.Additional.GovPublicApplyIsDeleteAllowed;
                    hlDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Delete=True", "IDCollection", "IDCollection", "请选择需要删除的申请！", "此操作将删除所选申请，确定吗？"));
                }
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var applyInfo = new GovPublicApplyInfo(e.Item.DataItem);

                var ltlTr = e.Item.FindControl("ltlTr") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlRemark = e.Item.FindControl("ltlRemark") as Literal;
                var ltlDepartment = e.Item.FindControl("ltlDepartment") as Literal;
                var ltlLimit = e.Item.FindControl("ltlLimit") as Literal;
                var ltlState = e.Item.FindControl("ltlState") as Literal;
                var ltlFlowUrl = e.Item.FindControl("ltlFlowUrl") as Literal;
                var ltlViewUrl = e.Item.FindControl("ltlViewUrl") as Literal;

                ltlTr.Text = @"<tr class=""tdbg"" style=""height:25px"">";
                var limitType = GovPublicApplyManager.GetLimitType(PublishmentSystemInfo, applyInfo);
                if (limitType == EGovPublicApplyLimitType.Alert)
                {
                    ltlTr.Text = @"<tr class=""tdbg"" style=""height:25px;background-color:#AAAAD5"">";
                }
                else if (limitType == EGovPublicApplyLimitType.Yellow)
                {
                    ltlTr.Text = @"<tr class=""tdbg"" style=""height:25px;background-color:#FF9"">";
                }
                else if (limitType == EGovPublicApplyLimitType.Red)
                {
                    ltlTr.Text = @"<tr class=""tdbg"" style=""height:25px;background-color:#FE5461"">";
                }

                if (applyInfo.State == EGovPublicApplyState.Accepted || applyInfo.State == EGovPublicApplyState.Redo)
                {
                    ltlTitle.Text =
                        $@"<a href=""{PageGovPublicApplyToReplyDetail.GetRedirectUrl(PublishmentSystemId,
                            applyInfo.Id, PageUrl)}"">{applyInfo.Title}</a>";
                }
                else if (applyInfo.State == EGovPublicApplyState.Checked || applyInfo.State == EGovPublicApplyState.Replied)
                {
                    ltlTitle.Text =
                        $@"<a href=""{PageGovPublicApplyToCheckDetail.GetRedirectUrl(PublishmentSystemId,
                            applyInfo.Id, PageUrl)}"">{applyInfo.Title}</a>";
                }
                else if (applyInfo.State == EGovPublicApplyState.Denied || applyInfo.State == EGovPublicApplyState.New)
                {
                    ltlTitle.Text =
                        $@"<a href=""{PageGovPublicApplyToAcceptDetail.GetRedirectUrl(PublishmentSystemId,
                            applyInfo.Id, PageUrl)}"">{applyInfo.Title}</a>";
                }
                var departmentName = DepartmentManager.GetDepartmentName(applyInfo.DepartmentId);
                if (applyInfo.DepartmentId > 0 && departmentName != applyInfo.DepartmentName)
                {
                    ltlTitle.Text += "<span style='color:red'>【转】</span>";
                }
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(applyInfo.AddDate);
                ltlRemark.Text = GovPublicApplyManager.GetApplyRemark(applyInfo.Id);
                ltlDepartment.Text = departmentName;
                ltlLimit.Text = EGovPublicApplyLimitTypeUtils.GetText(limitType);
                ltlState.Text = EGovPublicApplyStateUtils.GetText(applyInfo.State);
                if (applyInfo.State == EGovPublicApplyState.New)
                {
                    ltlState.Text = $"<span style='color:red'>{ltlState.Text}</span>";
                }
                else if (applyInfo.State == EGovPublicApplyState.Redo)
                {
                    ltlState.Text = $"<span style='color:red'>{ltlState.Text}</span>";
                }
                ltlFlowUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalGovPublicApplyFlow.GetOpenWindowString(
                        PublishmentSystemId, applyInfo.Id)}"">轨迹</a>";
                ltlViewUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalGovPublicApplyView.GetOpenWindowString(
                        PublishmentSystemId, applyInfo.Id)}"">查看</a>";
            }
        }

        protected virtual string GetSelectString() { return string.Empty;}

        protected virtual string PageUrl => string.Empty;

        protected virtual SortMode GetSortMode()
        {
            return SortMode.DESC;
        }
    }
}
