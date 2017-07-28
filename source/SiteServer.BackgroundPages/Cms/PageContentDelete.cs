using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentDelete : BasePageCms
    {
        public Literal ltlContents;
        public Control RetainRow;
        public RadioButtonList RetainFiles;
        public Button Submit;

        private Dictionary<int, List<int>> _idsDictionary = new Dictionary<int, List<int>>();
        private bool _isDeleteFromTrash;
        private string _returnUrl;

        public static string GetRedirectClickStringForMultiChannels(int publishmentSystemId, bool isDeleteFromTrash,
            string returnUrl)
        {
            return PageUtils.GetRedirectStringWithCheckBoxValue(PageUtils.GetCmsUrl(nameof(PageContentDelete),
                new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"IsDeleteFromTrash", isDeleteFromTrash.ToString()},
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), "IDsCollection", "IDsCollection", "请选择需要删除的内容！");
        }

        public static string GetRedirectClickStringForSingleChannel(int publishmentSystemId, int nodeId,
            bool isDeleteFromTrash, string returnUrl)
        {
            return PageUtils.GetRedirectStringWithCheckBoxValue(PageUtils.GetCmsUrl(nameof(PageContentDelete),
                new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()},
                    {"IsDeleteFromTrash", isDeleteFromTrash.ToString()},
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), "ContentIDCollection", "ContentIDCollection", "请选择需要删除的内容！");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ReturnUrl");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _isDeleteFromTrash = Body.GetQueryBool("IsDeleteFromTrash");
            _idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);

            //if (this.nodeID > 0)
            //{
            //    this.nodeInfo = NodeManager.GetNodeInfo(base.PublishmentSystemID, this.nodeID);
            //}
            //else
            //{
            //    this.nodeInfo = NodeManager.GetNodeInfo(base.PublishmentSystemID, -this.nodeID);
            //}
            //if (this.nodeInfo != null)
            //{
            //    this.tableStyle = NodeManager.GetTableStyle(base.PublishmentSystemInfo, nodeInfo);
            //    this.tableName = NodeManager.GetTableName(base.PublishmentSystemInfo, nodeInfo);
            //}

            //if (this.contentID == 0)
            //{
            //    if (!base.HasChannelPermissions(Math.Abs(this.nodeID), AppManager.CMS.Permission.Channel.ContentDelete))
            //    {
            //        PageUtils.RedirectToErrorPage("您没有删除此栏目内容的权限！");
            //        return;
            //    }
            //}
            //else
            //{
            //    ContentInfo contentInfo = DataProvider.ContentDAO.GetContentInfo(this.tableStyle, this.tableName, this.contentID);

            //    if (contentInfo == null || !string.Equals(Body.AdministratorName, contentInfo.AddUserName))
            //    {
            //        if (!base.HasChannelPermissions(Math.Abs(this.nodeID), AppManager.CMS.Permission.Channel.ContentDelete))
            //        {
            //            PageUtils.RedirectToErrorPage("您没有删除此栏目内容的权限！");
            //            return;
            //        }
            //    }
            //}

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "删除内容", string.Empty);

                var builder = new StringBuilder();
                foreach (var nodeID in _idsDictionary.Keys)
                {
                    var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeID);
                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
                    var contentIDList = _idsDictionary[nodeID];
                    foreach (int contentID in contentIDList)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);
                        if (contentInfo != null)
                        {
                            builder.Append(
                                $@"{WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, _returnUrl)}<br />");
                        }
                    }
                }
                ltlContents.Text = builder.ToString();

                if (!_isDeleteFromTrash)
                {
                    RetainRow.Visible = true;
                    InfoMessage("此操作将把所选内容放入回收站，确定吗？");
                }
                else
                {
                    RetainRow.Visible = false;
                    InfoMessage("此操作将从回收站中彻底删除所选内容，确定吗？");
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    foreach (var nodeID in _idsDictionary.Keys)
                    {
                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
                        var contentIDList = _idsDictionary[nodeID];

                        if (!_isDeleteFromTrash)
                        {
                            if (bool.Parse(RetainFiles.SelectedValue) == false)
                            {
                                DirectoryUtility.DeleteContents(PublishmentSystemInfo, nodeID, contentIDList);
                                SuccessMessage("成功删除内容以及生成页面！");
                            }
                            else
                            {
                                SuccessMessage("成功删除内容，生成页面未被删除！");
                            }

                            if (contentIDList.Count == 1)
                            {
                                var contentID = contentIDList[0];
                                var contentTitle = BaiRongDataProvider.ContentDao.GetValue(tableName, contentID, ContentAttribute.Title);
                                Body.AddSiteLog(PublishmentSystemId, nodeID, contentID, "删除内容",
                                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeID)},内容标题:{contentTitle}");
                            }
                            else
                            {
                                Body.AddSiteLog(PublishmentSystemId, "批量删除内容",
                                    $"栏目:{NodeManager.GetNodeNameNavigation(PublishmentSystemId, nodeID)},内容条数:{contentIDList.Count}");
                            }

                            DataProvider.ContentDao.TrashContents(PublishmentSystemId, tableName, contentIDList);

                            //引用内容，需要删除
                            var tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.BackgroundContent, EAuxiliaryTableType.JobContent, EAuxiliaryTableType.VoteContent);
                            foreach (AuxiliaryTableInfo table in tableList)
                            {
                                var targetContentIdList = BaiRongDataProvider.ContentDao.GetReferenceIdList(table.TableEnName, contentIDList);
                                if (targetContentIdList.Count > 0)
                                {
                                    var targetContentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyleUtils.GetEnumType(table.AuxiliaryTableType.ToString()), table.TableEnName, TranslateUtils.ToInt(targetContentIdList[0].ToString()));
                                    DataProvider.ContentDao.DeleteContents(targetContentInfo.PublishmentSystemId, table.TableEnName, targetContentIdList, targetContentInfo.NodeId);
                                }
                            }

                            CreateManager.CreateContentTrigger(PublishmentSystemId, nodeID);
                        }
                        else
                        {
                            SuccessMessage("成功从回收站清空内容！");
                            DataProvider.ContentDao.DeleteContents(PublishmentSystemId, tableName, contentIDList, nodeID);

                            Body.AddSiteLog(PublishmentSystemId, "从回收站清空内容", $"内容条数:{contentIDList.Count}");
                        }

                    }


                    AddWaitAndRedirectScript(_returnUrl);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "删除内容失败！");

                    LogUtils.AddErrorLog(ex);
                }
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(_returnUrl);
        }

    }
}
