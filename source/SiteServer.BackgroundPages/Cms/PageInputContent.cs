using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageInputContent : BasePageCms
    {
        public Repeater rptContents;
        public SqlPager spContents;
        public Literal ltlColumnHeadRows;
        public Literal ltlHeadRowReply;

        public Button AddButton;
        public Button Check;
        public Button Delete;
        public Button ImportExcel;
        public Button ExportExcel;
        public Button TaxisButton;
        public Button SelectListButton;
        public Button SelectFormButton;
        public Button btnReturn;

        private List<int> _relatedIdentities;
        private InputInfo _inputInfo;
        private List<TableStyleInfo> _styleInfoList;

        public static string GetRedirectUrl(int publishmentSystemId, string inputName)
        {
            return PageUtils.GetCmsUrl(nameof(PageInputContent), new NameValueCollection
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

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;
            spContents.SelectCommand = DataProvider.InputContentDao.GetSelectStringOfContentId(_inputInfo.InputId, string.Empty);
            spContents.SortField = DataProvider.InputContentDao.GetSortFieldName();
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                spContents.DataBind();

                BreadCrumbWithItemTitle(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdInput, "提交表单内容管理",
                    $"{_inputInfo.InputName}({spContents.TotalCount})", AppManager.Cms.Permission.WebSite.Input);

                var showPopWinString = string.Empty;

                showPopWinString = ModalInputContentAdd.GetOpenWindowStringToAdd(PublishmentSystemId, _inputInfo.InputId, PageUrl);
                AddButton.Attributes.Add("onclick", showPopWinString);


                //this.Delete.Attributes.Add("onclick", "return confirm(\"此操作将删除所选内容，确定吗？\");");
                Delete.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                        PageUtils.GetCmsUrl(nameof(PageInputContent), new NameValueCollection
                        {
                                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                                {"InputName", _inputInfo.InputName},
                                {"Delete", true.ToString()}
                        }), "ContentIDCollection", "ContentIDCollection", "请选择需要删除的表单内容！", "此操作将删除所选内容，确定删除吗？"));

                Check.Attributes.Add("onclick", "return confirm(\"此操作将把所选内容设为审核通过，确定吗？\");");


                showPopWinString = ModalInputContentTaxis.GetOpenWindowString(PublishmentSystemId, _inputInfo.InputId, PageUrl);
                TaxisButton.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowStringToInputContent(PublishmentSystemId, _inputInfo.InputId, true);
                SelectListButton.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectColumns.GetOpenWindowStringToInputContent(PublishmentSystemId, _inputInfo.InputId, false);
                SelectFormButton.Attributes.Add("onclick", showPopWinString);

                ImportExcel.Attributes.Add("onclick", ModalInputContentImport.GetOpenWindowString(PublishmentSystemId, _inputInfo.InputId));

                ExportExcel.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToInputContent(PublishmentSystemId, _inputInfo.InputId));

                var urlReturn = PageInput.GetRedirectUrl(PublishmentSystemId);
                btnReturn.Attributes.Add("onclick", $"location.href='{urlReturn}';return false;");


                if (_styleInfoList != null)
                {
                    foreach (var styleInfo in _styleInfoList)
                    {
                        if (styleInfo.IsVisibleInList)
                        {
                            ltlColumnHeadRows.Text += $@"<td class=""center"">{styleInfo.DisplayName}</td>";
                        }
                    }
                }

                if (_inputInfo.IsReply)
                {
                    ltlHeadRowReply.Text = @"
<td class=""center"" style=""width:60px;"">是否回复</td>
<td class=""center"" style=""width:60px;"">&nbsp;</td>
";
                }
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var contentID = SqlUtils.EvalInt(e.Item.DataItem, "ID");
                var contentInfo = DataProvider.InputContentDao.GetContentInfo(contentID);
                var columnItemRows = (Literal)e.Item.FindControl("ColumnItemRows");
                var itemEidtRow = (Literal)e.Item.FindControl("ItemEidtRow");
                var itemViewRow = (Literal)e.Item.FindControl("ItemViewRow");
                var itemRowReply = e.Item.FindControl("ItemRowReply") as Literal;
                var itemDateTime = e.Item.FindControl("ItemDateTime") as Literal;

                if (_styleInfoList != null)
                {
                    foreach (var styleInfo in _styleInfoList)
                    {
                        if (styleInfo.IsVisibleInList)
                        {
                            var value = contentInfo.Attributes.Get(styleInfo.AttributeName);

                            if (!string.IsNullOrEmpty(value))
                            {
                                value = InputParserUtility.GetContentByTableStyle(value, PublishmentSystemInfo, ETableStyle.InputContent, styleInfo);
                            }

                            if (contentInfo.IsChecked == false && string.IsNullOrEmpty(columnItemRows.Text))
                            {
                                columnItemRows.Text += $@"<td>{value}<span style=""color:red"">[未审核]</span></td>";
                            }
                            else
                            {
                                columnItemRows.Text += $@"<td>{value}</td>";
                            }
                        }
                    }
                }

                itemViewRow.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalInputContentView.GetOpenWindowString(
                        PublishmentSystemId, _inputInfo.InputId, contentInfo.Id)}"">查看</a>";
                itemEidtRow.Text =
                        $@"<a href=""javascript:;"" onclick=""{ModalInputContentAdd.GetOpenWindowStringToEdit(
                            PublishmentSystemId, _inputInfo.InputId, contentInfo.Id, PageUrl)}"">修改</a>";

                if (_inputInfo.IsReply)
                {
                    var text = string.IsNullOrEmpty(contentInfo.Reply) ? "提交回复" : "修改回复";
                    itemRowReply.Text = $@"
<td class=""center"">{StringUtils.GetTrueImageHtml(!string.IsNullOrEmpty(contentInfo.Reply))}</a></td>
<td class=""center""><a href=""javascript:;"" onclick=""{ModalInputContentReply.GetOpenWindowString(
                        PublishmentSystemId, _inputInfo.InputId, contentInfo.Id)}"">{text}</a></td>";
                }

                itemDateTime.Text = DateUtils.GetDateString(contentInfo.AddDate);
            }
        }

        public void Delete_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (Request.Form["ContentIDCollection"] != null)
                {
                    var arraylist = TranslateUtils.StringCollectionToIntList(Request.Form["ContentIDCollection"]);
                    try
                    {
                        DataProvider.InputContentDao.Delete(arraylist);
                        Body.AddSiteLog(PublishmentSystemId, "删除提交表单内容", $"提交表单:{_inputInfo.InputName}");
                        SuccessMessage("删除成功！");
                        PageUtils.Redirect(PageUtils.GetCmsUrl(nameof(PageInputContent), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"InputName", _inputInfo.InputName}
                        }));
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
                else
                    FailMessage("删除失败,请选择需要删除的内容！");
            }
        }

        public void Check_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (Request.Form["ContentIDCollection"] != null)
                {
                    var arraylist = TranslateUtils.StringCollectionToIntList(Request.Form["ContentIDCollection"]);
                    try
                    {
                        DataProvider.InputContentDao.Check(arraylist);
                        Body.AddSiteLog(PublishmentSystemId, "审核提交表单内容", $"提交表单:{_inputInfo.InputName}");
                        SuccessMessage("审核成功！");
                        PageUtils.Redirect(PageUtils.GetCmsUrl(nameof(PageInputContent), new NameValueCollection
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
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageInputContent), new NameValueCollection
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
