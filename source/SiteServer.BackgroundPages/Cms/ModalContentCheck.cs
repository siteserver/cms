using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.User;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentCheck : BasePageCms
    {
        public Literal ltlTitles;
        public RadioButtonList rblCheckType;
        public DropDownList ddlTranslateNodeID;
        public TextBox tbCheckReasons;

        private Dictionary<int, List<int>> _idsDictionary = new Dictionary<int, List<int>>();
        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("审核内容", PageUtils.GetCmsUrl(nameof(ModalContentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "请选择需要审核的内容！", 560, 550);
        }

        public static string GetOpenWindowStringForMultiChannels(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("审核内容", PageUtils.GetCmsUrl(nameof(ModalContentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "IDsCollection", "请选择需要审核的内容！", 560, 550);
        }

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("审核内容", PageUtils.GetCmsUrl(nameof(ModalContentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentIDCollection", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 560, 550);
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(ModalContentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)},
                {"ContentIDCollection", contentId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ReturnUrl");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            _idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);

            if (!IsPostBack)
            {
                var checkTaskTotal = 0;
                var checkContentTotal = 0;
                var unCheckTaskTotal = 0;
                ContentInfo contentInfo;
                var titles = new StringBuilder();
                foreach (var nodeID in _idsDictionary.Keys)
                {
                    var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeID);
                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
                    var contentIdList = _idsDictionary[nodeID];
                    foreach (var contentId in contentIdList)
                    {
                        checkContentTotal++;
                        contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                        
                        titles.Append(contentInfo.Title + "<br />");
                    }
                }

                if (!string.IsNullOrEmpty(ltlTitles.Text))
                {
                    titles.Length -= 6;
                }
                ltlTitles.Text = titles.ToString();

                var checkedLevel = 5;
                var isChecked = true;

                foreach (var nodeID in _idsDictionary.Keys)
                {
                    int checkedLevelByNodeID;
                    var isCheckedByNodeID = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, nodeID, out checkedLevelByNodeID);
                    if (checkedLevel > checkedLevelByNodeID)
                    {
                        checkedLevel = checkedLevelByNodeID;
                    }
                    if (!isCheckedByNodeID)
                    {
                        isChecked = isCheckedByNodeID;
                    }
                }

                LevelManager.LoadContentLevelToCheck(rblCheckType, PublishmentSystemInfo, isChecked, checkedLevel);

                var listItem = new ListItem("<保持原栏目不变>", "0");
                ddlTranslateNodeID.Items.Add(listItem);

                NodeManager.AddListItemsForAddContent(ddlTranslateNodeID.Items, PublishmentSystemInfo, true, Body.AdministratorName);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var taskID = 0;
            var checkedLevel = TranslateUtils.ToIntWithNagetive(rblCheckType.SelectedValue);
            var isChecked = false;
            var isTask = false;

            if (checkedLevel >= PublishmentSystemInfo.CheckContentLevel)
            {
                isChecked = true;
            }
            else
            {
                isChecked = false;
            }

            var contentInfoArrayListToCheck = new List<ContentInfo>();
            var idsDictionaryToCheck = new Dictionary<int, List<int>>();
            foreach (var nodeID in _idsDictionary.Keys)
            {
                var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeID);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
                var contentIDArrayList = _idsDictionary[nodeID];
                var contentIDArrayListToCheck = new List<int>();

                var checkedLevelOfUser = 0;
                var isCheckedOfUser = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, nodeID, out checkedLevelOfUser);

                foreach (int contentID in contentIDArrayList)
                {
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);
                    if (contentInfo != null)
                    {

                        if (LevelManager.IsCheckable(PublishmentSystemInfo, contentInfo.NodeId, contentInfo.IsChecked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                        {
                            contentInfoArrayListToCheck.Add(contentInfo);
                            contentIDArrayListToCheck.Add(contentID);
                        }

                        DataProvider.ContentDao.Update(tableName, PublishmentSystemInfo, contentInfo);

                        if (contentInfo.IsChecked)
                        {
                            CreateManager.CreateContentAndTrigger(PublishmentSystemId, contentInfo.NodeId, contentID);
                        }

                    }
                }
                if (contentIDArrayListToCheck.Count > 0)
                {
                    idsDictionaryToCheck[nodeID] = contentIDArrayListToCheck;
                }
            }

            if (contentInfoArrayListToCheck.Count == 0)
            {
                PageUtils.CloseModalPageWithoutRefresh(Page, "alert('您的审核权限不足，无法审核所选内容！');");
            }
            else
            {
                try
                {
                    var translateNodeID = TranslateUtils.ToInt(ddlTranslateNodeID.SelectedValue);

                    foreach (int nodeID in idsDictionaryToCheck.Keys)
                    {
                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
                        var contentIDArrayList = idsDictionaryToCheck[nodeID];
                        BaiRongDataProvider.ContentDao.UpdateIsChecked(tableName, PublishmentSystemId, nodeID, contentIDArrayList, translateNodeID, true, Body.AdministratorName, isChecked, checkedLevel, tbCheckReasons.Text);

                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo, nodeID, true);
                    }

                    if (translateNodeID > 0)
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo, translateNodeID, true);
                    }

                    Body.AddSiteLog(PublishmentSystemId, PublishmentSystemId, 0, "设置内容状态为" + rblCheckType.SelectedItem.Text, tbCheckReasons.Text);

                    if (isChecked)
                    {
                        foreach (int nodeID in idsDictionaryToCheck.Keys)
                        {
                            var contentIDArrayList = _idsDictionary[nodeID];
                            if (contentIDArrayList != null)
                            {
                                foreach (int contentID in contentIDArrayList)
                                {
                                    CreateManager.CreateContent(PublishmentSystemId, nodeID, contentID);
                                }
                            }
                        }
                    }

                    PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "操作失败！");
                }
            }
        }
    }
}
