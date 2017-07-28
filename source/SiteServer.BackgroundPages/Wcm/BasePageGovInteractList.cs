using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class BasePageGovInteractList : BasePageGovInteract
    {
        public PlaceHolder phAccept;
        public HyperLink hlAccept;
        public HyperLink hlDeny;
        public PlaceHolder phCheck;
        public HyperLink hlCheck;
        public HyperLink hlRedo;
        public PlaceHolder phSwitchToTranslate;
        public HyperLink hlSwitchTo;
        public HyperLink hlTranslate;
        public PlaceHolder phComment;
        public HyperLink hlComment;
        public PlaceHolder phDelete;
        public HyperLink hlDelete;
        public HyperLink hlExport;

        public Literal ltlTotalCount;

        public Repeater rptContents;
        public SqlPager spContents;

        protected int nodeID;
        private bool isPermissionReply = false;
        private bool isPermissionEdit = false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");
            nodeID = TranslateUtils.ToInt(Request.QueryString["NodeID"]);

            isPermissionReply = GovInteractManager.IsPermission(PublishmentSystemId, nodeID, AppManager.Wcm.Permission.GovInteract.GovInteractReply);
            isPermissionEdit = GovInteractManager.IsPermission(PublishmentSystemId, nodeID, AppManager.Wcm.Permission.GovInteract.GovInteractEdit);

            if (Body.IsQueryExists("Delete"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (arraylist.Count > 0)
                {
                    try
                    {
                        DataProvider.ContentDao.DeleteContents(PublishmentSystemId, PublishmentSystemInfo.AuxiliaryTableForGovInteract, arraylist, nodeID);
                        Body.AddSiteLog(PublishmentSystemId, "删除申请", Body.AdministratorName);
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
                foreach (var contentId in arraylist)
                {
                    var contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(PublishmentSystemInfo, contentId);
                    if (contentInfo.State == EGovInteractState.New || contentInfo.State == EGovInteractState.Denied)
                    {
                        GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Accept, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                        DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentInfo.Id, EGovInteractState.Accepted);
                    }
                }
                SuccessMessage("受理申请成功！");
            }
            else if (Body.IsQueryExists("Deny"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                foreach (var contentId in arraylist)
                {
                    var contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(PublishmentSystemInfo, contentId);
                    if (contentInfo.State == EGovInteractState.New || contentInfo.State == EGovInteractState.Accepted)
                    {
                        GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Deny, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                        DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentId, EGovInteractState.Denied);
                    }
                }
                SuccessMessage("拒绝受理申请成功！");
            }
            else if (Body.IsQueryExists("Check"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                foreach (var contentId in arraylist)
                {
                    var contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(PublishmentSystemInfo, contentId);
                    if (contentInfo.State == EGovInteractState.Replied)
                    {
                        GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Check, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                        DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentId, EGovInteractState.Checked);
                    }
                }
                SuccessMessage("审核申请成功！");
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SelectCommand = GetSelectString();
            spContents.SortField = ContentAttribute.Taxis;
            spContents.SortMode = GetSortMode();
            rptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                spContents.DataBind();
                ltlTotalCount.Text = spContents.TotalCount.ToString();

                if (phAccept != null)
                {
                    phAccept.Visible = GovInteractManager.IsPermission(PublishmentSystemId, nodeID, AppManager.Wcm.Permission.GovInteract.GovInteractAccept);
                    if (hlAccept != null)
                    {
                        hlAccept.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Accept=True", "IDCollection", "IDCollection", "请选择需要受理的申请！", "此操作将受理所选申请，确定吗？"));
                    }
                    if (hlDeny != null)
                    {
                        hlDeny.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Deny=True", "IDCollection", "IDCollection", "请选择需要拒绝的申请！", "此操作将拒绝受理所选申请，确定吗？"));
                    }
                }
                if (phCheck != null)
                {
                    phCheck.Visible = GovInteractManager.IsPermission(PublishmentSystemId, nodeID, AppManager.Wcm.Permission.GovInteract.GovInteractCheck);
                    if (hlCheck != null)
                    {
                        hlCheck.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Check=True", "IDCollection", "IDCollection", "请选择需要审核的申请！", "此操作将审核所选申请，确定吗？"));
                    }
                    if (hlRedo != null)
                    {
                        hlRedo.Attributes.Add("onclick", ModalGovInteractApplyRedo.GetOpenWindowString(PublishmentSystemId));
                    }
                }
                if (phSwitchToTranslate != null)
                {
                    phSwitchToTranslate.Visible = GovInteractManager.IsPermission(PublishmentSystemId, nodeID, AppManager.Wcm.Permission.GovInteract.GovInteractSwitchToTranslate);
                    if (hlSwitchTo != null)
                    {
                        hlSwitchTo.Attributes.Add("onclick", ModalGovInteractApplySwitchTo.GetOpenWindowString(PublishmentSystemId, nodeID));
                    }
                    if (hlTranslate != null)
                    {
                        hlTranslate.Attributes.Add("onclick", ModalGovInteractApplyTranslate.GetOpenWindowString(PublishmentSystemId, nodeID));
                    }
                }
                if (phComment != null)
                {
                    phComment.Visible = GovInteractManager.IsPermission(PublishmentSystemId, nodeID, AppManager.Wcm.Permission.GovInteract.GovInteractComment);
                    hlComment.Attributes.Add("onclick", ModalGovInteractApplyComment.GetOpenWindowString(PublishmentSystemId));
                }
                if (phDelete != null)
                {
                    phDelete.Visible = GovInteractManager.IsPermission(PublishmentSystemId, nodeID, AppManager.Wcm.Permission.GovInteract.GovInteractDelete) && PublishmentSystemInfo.Additional.GovInteractApplyIsDeleteAllowed;
                    hlDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUrl + "&Delete=True", "IDCollection", "IDCollection", "请选择需要删除的申请！", "此操作将删除所选申请，确定吗？"));
                }
                if (hlExport != null)
                {
                    hlExport.Attributes.Add("onclick", ModalContentExport.GetOpenWindowString(PublishmentSystemId, nodeID));
                }
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var contentInfo = new GovInteractContentInfo(e.Item.DataItem);

                var ltlTr = e.Item.FindControl("ltlTr") as Literal;
                var ltlID = e.Item.FindControl("ltlID") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlRemark = e.Item.FindControl("ltlRemark") as Literal;
                var ltlDepartment = e.Item.FindControl("ltlDepartment") as Literal;
                var ltlLimit = e.Item.FindControl("ltlLimit") as Literal;
                var ltlState = e.Item.FindControl("ltlState") as Literal;
                var ltlFlowUrl = e.Item.FindControl("ltlFlowUrl") as Literal;
                var ltlViewUrl = e.Item.FindControl("ltlViewUrl") as Literal;
                var ltlReplyUrl = e.Item.FindControl("ltlReplyUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                var limitType = EGovInteractLimitType.Normal;
                ltlTr.Text = @"<tr>";
                if (contentInfo.State == EGovInteractState.Denied || contentInfo.State == EGovInteractState.Checked)
                {
                    ltlTr.Text = @"<tr class=""success"">";
                }
                else
                {
                    limitType = GovInteractApplyManager.GetLimitType(PublishmentSystemInfo, contentInfo);
                    if (limitType == EGovInteractLimitType.Alert)
                    {
                        ltlTr.Text = @"<tr class=""info"">";
                    }
                    else if (limitType == EGovInteractLimitType.Yellow)
                    {
                        ltlTr.Text = @"<tr class=""warning"">";
                    }
                    else if (limitType == EGovInteractLimitType.Red)
                    {
                        ltlTr.Text = @"<tr class=""error"">";
                    }
                }

                ltlID.Text = contentInfo.Id.ToString();

                var title = contentInfo.Title;
                if (string.IsNullOrEmpty(title))
                {
                    title = StringUtils.MaxLengthText(contentInfo.Content, 30);
                }
                if (string.IsNullOrEmpty(title))
                {
                    title = contentInfo.QueryCode;
                }

                var target = PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow ? @"target=""_blank""" : string.Empty;
                if (contentInfo.State == EGovInteractState.Accepted || contentInfo.State == EGovInteractState.Redo)
                {
                    ltlTitle.Text =
                        $@"<a href=""{PageGovInteractPageReply.GetRedirectUrl(PublishmentSystemId,
                            contentInfo.NodeId, contentInfo.Id, PageUrl)}"" {target}>{title}</a>";
                }
                else if (contentInfo.State == EGovInteractState.Checked || contentInfo.State == EGovInteractState.Replied)
                {
                    ltlTitle.Text =
                        $@"<a href=""{PageGovInteractPageCheck.GetRedirectUrl(PublishmentSystemId,
                            contentInfo.NodeId, contentInfo.Id, PageUrl)}"" {target}>{title}</a>";
                }
                else if (contentInfo.State == EGovInteractState.Denied || contentInfo.State == EGovInteractState.New)
                {
                    ltlTitle.Text =
                        $@"<a href=""{PageGovInteractPageAccept.GetRedirectUrl(PublishmentSystemId,
                            contentInfo.NodeId, contentInfo.Id, PageUrl)}"" {target}>{title}</a>";
                }
                var departmentName = DepartmentManager.GetDepartmentName(contentInfo.DepartmentId);
                if (contentInfo.DepartmentId > 0 && departmentName != contentInfo.DepartmentName)
                {
                    ltlTitle.Text += "<span style='color:red'>【转办】</span>";
                }
                else if (TranslateUtils.ToInt(contentInfo.GetExtendedAttribute(GovInteractContentAttribute.TranslateFromNodeId)) > 0)
                {
                    ltlTitle.Text += "<span style='color:red'>【转移】</span>";
                }
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
                ltlRemark.Text = GovInteractApplyManager.GetApplyRemark(PublishmentSystemId, contentInfo.Id);
                ltlDepartment.Text = departmentName;
                ltlLimit.Text = EGovInteractLimitTypeUtils.GetText(limitType);
                ltlState.Text = EGovInteractStateUtils.GetText(contentInfo.State);
                if (contentInfo.State == EGovInteractState.New)
                {
                    ltlState.Text = $"<span style='color:red'>{ltlState.Text}</span>";
                }
                else if (contentInfo.State == EGovInteractState.Redo)
                {
                    ltlState.Text = $"<span style='color:red'>{ltlState.Text}</span>";
                }
                ltlFlowUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalGovInteractApplyFlow.GetOpenWindowString(
                        PublishmentSystemId, contentInfo.NodeId, contentInfo.Id)}"">轨迹</a>";
                ltlViewUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalGovInteractApplyView.GetOpenWindowString(
                        PublishmentSystemId, contentInfo.NodeId, contentInfo.Id)}"">查看</a>";
                if (isPermissionReply)
                {
                    ltlReplyUrl.Text =
                        $@"<a href=""javascript:;"" onclick=""{ModalGovInteractApplyReply.GetOpenWindowString(
                            PublishmentSystemId, contentInfo.NodeId, contentInfo.Id)}"">办理</a>";
                }
                if (isPermissionEdit)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, contentInfo.NodeId);
                    ltlEditUrl.Text =
                        $@"<a href=""{WebUtils.GetContentAddEditUrl(PublishmentSystemId, nodeInfo, contentInfo.Id,
                            PageUrl)}"">编辑</a>";
                }
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
