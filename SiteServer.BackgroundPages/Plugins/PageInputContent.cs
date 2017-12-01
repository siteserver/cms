using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Plugins
{
    public class PageInputContent : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlColumnHeadRows;
        public Literal LtlHeadRowReply;

        public Button BtnAdd;
        public Button BtnCheck;
        public Button BtnDelete;
        public Button BtnImportExcel;
        public Button BtnExportExcel;
        public Button BtnTaxis;
        public Button BtnSelectList;
        public Button BtnSelectForm;
        public Button BtnReturn;

        private List<int> _relatedIdentities;
        private InputInfo _inputInfo;
        private List<TableStyleInfo> _styleInfoList;

        public static string GetRedirectUrl(int publishmentSystemId, string inputName)
        {
            return PageUtils.GetPluginsUrl(nameof(PageInputContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputName", inputName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "InputName");
            var theInputName = Body.GetQueryString("InputName");

            _inputInfo = DataProvider.InputDao.GetInputInfo(theInputName, PublishmentSystemId);
            if (_inputInfo == null) return;

            if (Body.IsQueryExists("Delete") && Body.IsQueryExists("ContentIDCollection"))
            {
                var arraylist = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
                try
                {
                    DataProvider.InputContentDao.Delete(arraylist);
                    Body.AddSiteLog(PublishmentSystemId, "删除提交表单内容", $"提交表单:{_inputInfo.InputName}");
                    SuccessMessage("删除成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "删除失败！");
                }
            }

            _relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, PublishmentSystemId, _inputInfo.InputId);

            _styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, _relatedIdentities);

            var isAnythingVisible = false;
            foreach (var styleInfo in _styleInfoList)
            {
                if (styleInfo.IsVisibleInList)
                {
                    isAnythingVisible = true;
                    break;
                }
            }
            if (!isAnythingVisible && _styleInfoList != null && _styleInfoList.Count > 0)
            {
                _styleInfoList[0].IsVisibleInList = true;
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            SpContents.SelectCommand = DataProvider.InputContentDao.GetSelectStringOfContentId(_inputInfo.InputId, string.Empty);
            SpContents.SortField = DataProvider.InputContentDao.GetSortFieldName();
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (!IsPostBack)
            {
                SpContents.DataBind();

                var showPopWinString = ModalInputContentAdd.GetOpenWindowStringToAdd(PublishmentSystemId, _inputInfo.InputId, PageUrl);
                BtnAdd.Attributes.Add("onclick", showPopWinString);

                BtnDelete.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                        PageUtils.GetPluginsUrl(nameof(PageInputContent), new NameValueCollection
                        {
                                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                                {"InputName", _inputInfo.InputName},
                                {"Delete", true.ToString()}
                        }), "ContentIDCollection", "ContentIDCollection", "请选择需要删除的表单内容！", "此操作将删除所选内容，确定删除吗？"));

                BtnCheck.Attributes.Add("onclick", "return confirm(\"此操作将把所选内容设为审核通过，确定吗？\");");

                showPopWinString = ModalInputContentTaxis.GetOpenWindowString(PublishmentSystemId, _inputInfo.InputId, PageUrl);
                BtnTaxis.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowStringToInputContent(PublishmentSystemId, _inputInfo.InputId, true);
                BtnSelectList.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowStringToInputContent(PublishmentSystemId, _inputInfo.InputId, false);
                BtnSelectForm.Attributes.Add("onclick", showPopWinString);

                BtnImportExcel.Attributes.Add("onclick", ModalInputContentImport.GetOpenWindowString(PublishmentSystemId, _inputInfo.InputId));

                BtnExportExcel.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToInputContent(PublishmentSystemId, _inputInfo.InputId));

                var urlReturn = PageInput.GetRedirectUrl(PublishmentSystemId);
                BtnReturn.Attributes.Add("onclick", $"location.href='{urlReturn}';return false;");

                if (_styleInfoList != null)
                {
                    foreach (var styleInfo in _styleInfoList)
                    {
                        if (styleInfo.IsVisibleInList)
                        {
                            LtlColumnHeadRows.Text += $@"<td class=""center"">{styleInfo.DisplayName}</td>";
                        }
                    }
                }

                if (_inputInfo.IsReply)
                {
                    LtlHeadRowReply.Text = @"
<td class=""center"" style=""width:60px;"">是否回复</td>
<td class=""center"" style=""width:60px;"">&nbsp;</td>
";
                }
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentId = SqlUtils.EvalInt(e.Item.DataItem, "ID");
            var contentInfo = DataProvider.InputContentDao.GetContentInfo(contentId);

            var ltlColumnRows = (Literal)e.Item.FindControl("ltlColumnRows");
            var ltlEidt = (Literal)e.Item.FindControl("ltlEidt");
            var ltlView = (Literal)e.Item.FindControl("ltlView");
            var ltlReplyRows = (Literal)e.Item.FindControl("ltlReplyRows");
            var ltlDateTime = (Literal)e.Item.FindControl("ltlDateTime");

            if (_styleInfoList != null)
            {
                foreach (var styleInfo in _styleInfoList)
                {
                    if (styleInfo.IsVisibleInList)
                    {
                        var value = contentInfo.GetExtendedAttribute(styleInfo.AttributeName);

                        if (!string.IsNullOrEmpty(value))
                        {
                            value = InputParserUtility.GetContentByTableStyle(value, PublishmentSystemInfo, ETableStyle.InputContent, styleInfo);
                        }

                        if (contentInfo.IsChecked == false && string.IsNullOrEmpty(ltlColumnRows.Text))
                        {
                            ltlColumnRows.Text += $@"<td>{value}<span style=""color:red"">[未审核]</span></td>";
                        }
                        else
                        {
                            ltlColumnRows.Text += $@"<td>{value}</td>";
                        }
                    }
                }
            }

            ltlView.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalInputContentView.GetOpenWindowString(
                    PublishmentSystemId, _inputInfo.InputId, contentInfo.Id)}"">查看</a>";
            ltlEidt.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalInputContentAdd.GetOpenWindowStringToEdit(
                    PublishmentSystemId, _inputInfo.InputId, contentInfo.Id, PageUrl)}"">修改</a>";

            if (_inputInfo.IsReply)
            {
                var text = string.IsNullOrEmpty(contentInfo.Reply) ? "提交回复" : "修改回复";
                ltlReplyRows.Text = $@"
<td class=""center"">{StringUtils.GetTrueImageHtml(!string.IsNullOrEmpty(contentInfo.Reply))}</a></td>
<td class=""center""><a href=""javascript:;"" onclick=""{ModalInputContentReply.GetOpenWindowString(
                    PublishmentSystemId, _inputInfo.InputId, contentInfo.Id)}"">{text}</a></td>";
            }

            ltlDateTime.Text = DateUtils.GetDateString(contentInfo.AddDate);
        }

        public void BtnCheck_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (Request.Form["ContentIDCollection"] != null)
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.Form["ContentIDCollection"]);
                try
                {
                    DataProvider.InputContentDao.Check(list);
                    Body.AddSiteLog(PublishmentSystemId, "审核提交表单内容", $"提交表单:{_inputInfo.InputName}");
                    SuccessMessage("审核成功！");
                    PageUtils.Redirect(PageUtils.GetPluginsUrl(nameof(PageInputContent), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"InputName", _inputInfo.InputName}
                    }));
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "审核失败！");
                }
            }
            else
            {
                FailMessage("删除失败,请选择需要审核的内容！");
            }
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetPluginsUrl(nameof(PageInputContent), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"InputName", _inputInfo.InputName}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
