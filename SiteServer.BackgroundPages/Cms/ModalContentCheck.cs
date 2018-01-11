using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentCheck : BasePageCms
    {
        public Literal LtlTitles;
        public DropDownList DdlCheckType;
        public DropDownList DdlTranslateNodeId;
        public TextBox TbCheckReasons;

        private Dictionary<int, List<int>> _idsDictionary = new Dictionary<int, List<int>>();
        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("审核内容", PageUtils.GetCmsUrl(nameof(ModalContentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "请选择需要审核的内容！", 560, 550);
        }

        public static string GetOpenWindowStringForMultiChannels(int publishmentSystemId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("审核内容", PageUtils.GetCmsUrl(nameof(ModalContentCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "IDsCollection", "请选择需要审核的内容！", 560, 550);
        }

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("审核内容", PageUtils.GetCmsUrl(nameof(ModalContentCheck), new NameValueCollection
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
                var titles = new StringBuilder();
                foreach (var nodeId in _idsDictionary.Keys)
                {
                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);
                    var contentIdList = _idsDictionary[nodeId];
                    foreach (var contentId in contentIdList)
                    {
                        var title = DataProvider.ContentDao.GetValue(tableName, contentId, ContentAttribute.Title);
                        titles.Append(title + "<br />");
                    }
                }

                if (!string.IsNullOrEmpty(LtlTitles.Text))
                {
                    titles.Length -= 6;
                }
                LtlTitles.Text = titles.ToString();

                var checkedLevel = 5;
                var isChecked = true;

                foreach (var nodeId in _idsDictionary.Keys)
                {
                    int checkedLevelByNodeId;
                    var isCheckedByNodeId = CheckManager.GetUserCheckLevel(Body.AdminName, PublishmentSystemInfo, nodeId, out checkedLevelByNodeId);
                    if (checkedLevel > checkedLevelByNodeId)
                    {
                        checkedLevel = checkedLevelByNodeId;
                    }
                    if (!isCheckedByNodeId)
                    {
                        isChecked = false;
                    }
                }

                CheckManager.LoadContentLevelToCheck(DdlCheckType, PublishmentSystemInfo, isChecked, checkedLevel);

                var listItem = new ListItem("<保持原栏目不变>", "0");
                DdlTranslateNodeId.Items.Add(listItem);

                NodeManager.AddListItemsForAddContent(DdlTranslateNodeId.Items, PublishmentSystemInfo, true, Body.AdminName);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var checkedLevel = TranslateUtils.ToIntWithNagetive(DdlCheckType.SelectedValue);

            var isChecked = checkedLevel >= PublishmentSystemInfo.CheckContentLevel;

            var contentInfoArrayListToCheck = new List<ContentInfo>();
            var idsDictionaryToCheck = new Dictionary<int, List<int>>();
            foreach (var nodeId in _idsDictionary.Keys)
            {
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);
                var contentIdList = _idsDictionary[nodeId];
                var contentIdListToCheck = new List<int>();

                int checkedLevelOfUser;
                var isCheckedOfUser = CheckManager.GetUserCheckLevel(Body.AdminName, PublishmentSystemInfo, nodeId, out checkedLevelOfUser);

                foreach (var contentId in contentIdList)
                {
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                    if (contentInfo != null)
                    {
                        if (CheckManager.IsCheckable(PublishmentSystemInfo, contentInfo.NodeId, contentInfo.IsChecked, contentInfo.CheckedLevel, isCheckedOfUser, checkedLevelOfUser))
                        {
                            contentInfoArrayListToCheck.Add(contentInfo);
                            contentIdListToCheck.Add(contentId);
                        }

                        DataProvider.ContentDao.Update(tableName, PublishmentSystemInfo, contentInfo);

                        if (contentInfo.IsChecked)
                        {
                            CreateManager.CreateContentAndTrigger(PublishmentSystemId, contentInfo.NodeId, contentId);
                        }

                    }
                }
                if (contentIdListToCheck.Count > 0)
                {
                    idsDictionaryToCheck[nodeId] = contentIdListToCheck;
                }
            }

            if (contentInfoArrayListToCheck.Count == 0)
            {
                LayerUtils.CloseWithoutRefresh(Page, "alert('您的审核权限不足，无法审核所选内容！');");
            }
            else
            {
                try
                {
                    var translateNodeId = TranslateUtils.ToInt(DdlTranslateNodeId.SelectedValue);

                    foreach (var nodeId in idsDictionaryToCheck.Keys)
                    {
                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);
                        var contentIdList = idsDictionaryToCheck[nodeId];
                        DataProvider.ContentDao.UpdateIsChecked(tableName, PublishmentSystemId, nodeId, contentIdList, translateNodeId, true, Body.AdminName, isChecked, checkedLevel, TbCheckReasons.Text);

                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo, nodeId, true);
                    }

                    if (translateNodeId > 0)
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo, translateNodeId, true);
                    }

                    Body.AddSiteLog(PublishmentSystemId, PublishmentSystemId, 0, "设置内容状态为" + DdlCheckType.SelectedItem.Text, TbCheckReasons.Text);

                    if (isChecked)
                    {
                        foreach (var nodeId in idsDictionaryToCheck.Keys)
                        {
                            var contentIdList = _idsDictionary[nodeId];
                            if (contentIdList != null)
                            {
                                foreach (var contentId in contentIdList)
                                {
                                    CreateManager.CreateContent(PublishmentSystemId, nodeId, contentId);
                                }
                            }
                        }
                    }

                    LayerUtils.CloseAndRedirect(Page, _returnUrl);
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "操作失败！");
                }
            }
        }
    }
}
