using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageVoteContentAdd : BasePageCms
    {
        public Literal ltlPageTitle;

        public TextBox tbSummary;
        public DropDownList ddlMaxSelectNum;
        public DateTimeTextBox dtbAddDate;
        public DropDownList ddlEndDate;
        public DateTimeTextBox dtbEndDate;
        public RadioButtonList rblIsVotedView;
        public TextBox tbHiddenContent;
        public AuxiliaryControl acAttributes;

        public Literal ltlScript;
        public Button Submit;

        private int nodeID;
        private int contentID;
        private string returnUrl;
        private bool isSummary;
        private ArrayList voteOptionInfoArrayList;
        private List<int> relatedIdentities;
        private ETableStyle tableStyle;
        private string tableName;

        public bool IsSummary => isSummary;

        public static string GetRedirectUrlOfAdd(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageVoteContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrlOfEdit(int publishmentSystemId, int nodeId, int id, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageVoteContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ID", id.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public string GetOptionTitle(int itemIndex)
        {
            if (voteOptionInfoArrayList == null || voteOptionInfoArrayList.Count <= itemIndex) return string.Empty;
            var optionInfo = voteOptionInfoArrayList[itemIndex] as VoteOptionInfo;
            return optionInfo.Title;
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            nodeID = Body.GetQueryInt("NodeID");
            contentID = Body.GetQueryInt("ID");
            returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            tableStyle = ETableStyle.VoteContent;
            tableName = PublishmentSystemInfo.AuxiliaryTableForVote;
            relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeID);

            if (!IsPostBack)
            {
                var pageTitle = (contentID == 0) ? "添加投票" : "修改投票";
                var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeID);
                BreadCrumbWithItemTitle(AppManager.Cms.LeftMenu.IdContent, pageTitle, nodeNames, string.Empty);

                ltlPageTitle.Text = pageTitle;

                var excludeAttributeNames = TableManager.GetExcludeAttributeNames(tableStyle);
                acAttributes.AddExcludeAttributeNames(excludeAttributeNames);

                if (contentID == 0)
                {
                    var listItem = new ListItem("无结束日期", string.Empty);
                    ddlEndDate.Items.Add(listItem);
                    listItem = new ListItem("一周", DateTime.Now.AddDays(7).ToShortDateString());
                    ddlEndDate.Items.Add(listItem);
                    listItem = new ListItem("一月", DateTime.Now.AddDays(30).ToShortDateString());
                    listItem.Selected = true;
                    ddlEndDate.Items.Add(listItem);
                    listItem = new ListItem("半年", DateTime.Now.AddDays(180).ToShortDateString());
                    ddlEndDate.Items.Add(listItem);
                    listItem = new ListItem("一年", DateTime.Now.AddDays(365).ToShortDateString());
                    ddlEndDate.Items.Add(listItem);
                    listItem = new ListItem("自定义", DateTime.Now.AddDays(30).ToShortDateString());
                    ddlEndDate.Items.Add(listItem);

                    dtbEndDate.DateTime = DateTime.Now.AddDays(30);

                    var formCollection = new NameValueCollection();

                    acAttributes.SetParameters(formCollection, PublishmentSystemInfo, nodeID, relatedIdentities, tableStyle, tableName, false, IsPostBack);
                }
                else
                {
                    var contentInfo = DataProvider.VoteContentDao.GetContentInfo(PublishmentSystemInfo, contentID);

                    voteOptionInfoArrayList = DataProvider.VoteOptionDao.GetVoteOptionInfoArrayList(PublishmentSystemId, nodeID, contentID);
                    var script = string.Empty;
                    for (var i = 2; i < voteOptionInfoArrayList.Count; i++ )
                    {
                        var optionInfo = voteOptionInfoArrayList[i] as VoteOptionInfo;
                        script += $"addItem('{optionInfo.Title}');";
                    }
                    if (!string.IsNullOrEmpty(script))
                    {
                        ltlScript.Text = $@"<script>{script}</script>";
                    }

                    isSummary = contentInfo.IsSummary;
                    tbSummary.Text = contentInfo.Summary;

                    for (var i = 2; i < voteOptionInfoArrayList.Count; i++)
                    {
                        var listItem = new ListItem($"最多选{i}项", i.ToString());
                        ddlMaxSelectNum.Items.Add(listItem);
                    }

                    ControlUtils.SelectListItems(ddlMaxSelectNum, contentInfo.MaxSelectNum.ToString());
                    dtbAddDate.DateTime = contentInfo.AddDate;
                    ddlEndDate.Visible = false;
                    dtbEndDate.DateTime = contentInfo.EndDate;
                    ControlUtils.SelectListItemsIgnoreCase(rblIsVotedView, contentInfo.IsVotedView.ToString());
                    tbHiddenContent.Text = contentInfo.HiddenContent;

                    acAttributes.SetParameters(contentInfo.Attributes, PublishmentSystemInfo, nodeID, relatedIdentities, tableStyle, tableName, true, IsPostBack);
                }

                Submit.Attributes.Add("onclick", InputParserUtils.GetValidateSubmitOnClickScript("myForm"));
            }
            else
            {
                if (contentID == 0)
                {
                    acAttributes.SetParameters(Request.Form, PublishmentSystemInfo, nodeID, relatedIdentities, tableStyle, tableName, false, IsPostBack);
                }
                else
                {
                    acAttributes.SetParameters(Request.Form, PublishmentSystemInfo, nodeID, relatedIdentities, tableStyle, tableName, true, IsPostBack);
                }
            }
            DataBind();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (this.contentID == 0)
                {
                    var contentInfo = new VoteContentInfo();
                    try
                    {
                        InputTypeParser.AddValuesToAttributes(tableStyle, tableName, PublishmentSystemInfo, relatedIdentities, Request.Form, contentInfo.Attributes, ContentAttribute.HiddenAttributes);

                        contentInfo.NodeId = nodeID;
                        contentInfo.PublishmentSystemId = PublishmentSystemId;
                        contentInfo.AddUserName = Body.AdministratorName;
                        if (contentInfo.AddDate.Year == DateUtils.SqlMinValue.Year)
                        {
                            FailMessage($"投票添加失败：系统时间不能为{DateUtils.SqlMinValue.Year}年");
                            return;
                        }
                        contentInfo.LastEditUserName = contentInfo.AddUserName;
                        contentInfo.LastEditDate = DateTime.Now;
                        contentInfo.IsChecked = false;

                        contentInfo.IsSummary = TranslateUtils.ToBool(Request.Form["IsSummary"]);
                        contentInfo.Summary = tbSummary.Text;
                        contentInfo.MaxSelectNum = TranslateUtils.ToInt(ddlMaxSelectNum.SelectedValue);
                        contentInfo.AddDate = dtbAddDate.DateTime;
                        if (string.IsNullOrEmpty(ddlEndDate.SelectedValue))
                        {
                            contentInfo.EndDate = dtbEndDate.DateTime;
                        }
                        else
                        {
                            contentInfo.EndDate = TranslateUtils.ToDateTime(ddlEndDate.SelectedValue);
                        }
                        contentInfo.IsVotedView = TranslateUtils.ToBool(rblIsVotedView.SelectedValue);
                        contentInfo.HiddenContent = tbHiddenContent.Text;

                        var checkedLevel = 0;
                        contentInfo.IsChecked = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, nodeID, out checkedLevel);
                        contentInfo.CheckedLevel = checkedLevel;

                        var contentID = DataProvider.ContentDao.Insert(tableName, PublishmentSystemInfo, contentInfo);

                        var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);
                        var voteOptionInfoArrayList = new ArrayList();
                        for (var i = 0; i < itemCount; i++)
                        {
                            var title = Request.Form["options[" + i + "]"];
                            if (!string.IsNullOrEmpty(title))
                            {
                                var optionInfo = new VoteOptionInfo(0, PublishmentSystemId, nodeID, contentID, title, string.Empty, string.Empty, 0);
                                voteOptionInfoArrayList.Add(optionInfo);
                            }
                        }
                        DataProvider.VoteOptionDao.Insert(voteOptionInfoArrayList);

                        contentInfo.Id = contentID;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, $"投票添加失败：{ex.Message}");
                        LogUtils.AddErrorLog(ex);
                        return;
                    }

                    if (contentInfo.IsChecked)
                    {
                        CreateManager.CreateContentAndTrigger(PublishmentSystemId, nodeID, contentInfo.Id);
                    }

                    Body.AddSiteLog(PublishmentSystemId, nodeID, contentInfo.Id, "添加投票",
                        $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},投票标题:{contentInfo.Title}");

                    PageUtils.Redirect(PageContentAddAfter.GetRedirectUrl(PublishmentSystemId, nodeID, contentInfo.Id, returnUrl));
                }
                else
                {
                    var contentInfo = DataProvider.VoteContentDao.GetContentInfo(PublishmentSystemInfo, contentID);
                    try
                    {                        
                        InputTypeParser.AddValuesToAttributes(tableStyle, tableName, PublishmentSystemInfo, relatedIdentities, Request.Form, contentInfo.Attributes, ContentAttribute.HiddenAttributes);

                        contentInfo.LastEditUserName = Body.AdministratorName;
                        contentInfo.LastEditDate = DateTime.Now;

                        contentInfo.IsSummary = TranslateUtils.ToBool(Request.Form["IsSummary"]);
                        contentInfo.Summary = tbSummary.Text;
                        contentInfo.MaxSelectNum = TranslateUtils.ToInt(ddlMaxSelectNum.SelectedValue);
                        contentInfo.AddDate = dtbAddDate.DateTime;
                        contentInfo.EndDate = dtbEndDate.DateTime;
                        contentInfo.IsVotedView = TranslateUtils.ToBool(rblIsVotedView.SelectedValue);
                        contentInfo.HiddenContent = tbHiddenContent.Text;

                        DataProvider.ContentDao.Update(tableName, PublishmentSystemInfo, contentInfo);

                        var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);
                        var voteOptionInfoArrayList = new ArrayList();
                        for (var i = 0; i < itemCount; i++)
                        {
                            var title = Request.Form["options[" + i + "]"];
                            if (!string.IsNullOrEmpty(title))
                            {
                                var optionInfo = new VoteOptionInfo(0, PublishmentSystemId, nodeID, contentID, title, string.Empty, string.Empty, 0);
                                voteOptionInfoArrayList.Add(optionInfo);
                            }
                        }
                        DataProvider.VoteOptionDao.UpdateVoteOptionInfoArrayList(PublishmentSystemId, nodeID, contentID, voteOptionInfoArrayList);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, $"投票修改失败：{ex.Message}");
                        LogUtils.AddErrorLog(ex);
                        return;
                    }

                    if (contentInfo.IsChecked)
                    {
                        CreateManager.CreateContentAndTrigger(PublishmentSystemId, nodeID, contentID);
                    }

                    Body.AddSiteLog(PublishmentSystemId, nodeID, contentID, "修改投票",
                        $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId)},投票标题:{contentInfo.Title}");

                    PageUtils.Redirect(returnUrl);
                }
            }
        }

        public string ReturnUrl => returnUrl;
    }
}
