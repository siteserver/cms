using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalAddToGroup : BasePageCms
    {
        protected CheckBoxList CblGroupNameCollection;
        protected Button BtnAddGroup;

        private bool _isContent;
        private Dictionary<int, List<int>> _idsDictionary = new Dictionary<int, List<int>>();
        private List<int> _channelIdArrayList = new List<int>();

        public static string GetOpenWindowStringToContentForMultiChannels(int siteId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("添加到内容组",
                PageUtils.GetCmsUrl(siteId, nameof(ModalAddToGroup), new NameValueCollection
                {
                    {"isContent", "True"}
                }), "IDsCollection", "请选择需要设置组别的内容！", 650, 550);
        }

        public static string GetOpenWindowStringToContent(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("添加到内容组",
                PageUtils.GetCmsUrl(siteId, nameof(ModalAddToGroup), new NameValueCollection
                {
                    {"channelId", channelId.ToString()},
                    {"isContent", "True"}
                }), "contentIdCollection", "请选择需要设置组别的内容！", 650, 550);
        }

        public static string GetOpenWindowStringToChannel(int siteId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("添加到栏目组",
                PageUtils.GetCmsUrl(siteId, nameof(ModalAddToGroup), new NameValueCollection
                {
                    {"isContent", "False"}
                }), "ChannelIDCollection", "请选择需要设置组别的栏目！", 650, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (AuthRequest.IsQueryExists("isContent"))
            {
                _isContent = AuthRequest.GetQueryBool("isContent");
            }
            if (_isContent)
            {
                BtnAddGroup.Text = " 新建内容组";
                _idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);
            }
            else
            {
                BtnAddGroup.Text = " 新建栏目组";
                _channelIdArrayList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("ChannelIDCollection"));
            }
            if (!IsPostBack)
            {
                if (_isContent)
                {
                    var contentGroupNameList = ContentGroupManager.GetGroupNameList(SiteId);
                    foreach (var groupName in contentGroupNameList)
                    {
                        var item = new ListItem(groupName, groupName);
                        CblGroupNameCollection.Items.Add(item);
                    }
                    var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(SiteId);
                    BtnAddGroup.Attributes.Add("onclick", showPopWinString);
                }
                else
                {
                    var nodeGroupNameList = ChannelGroupManager.GetGroupNameList(SiteId);
                    foreach (var groupName in nodeGroupNameList)
                    {
                        var item = new ListItem(groupName, groupName);
                        CblGroupNameCollection.Items.Add(item);
                    }

                    var showPopWinString = ModalNodeGroupAdd.GetOpenWindowString(SiteId);
                    BtnAddGroup.Attributes.Add("onclick", showPopWinString);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;

            try
            {
                if (_isContent)
                {
                    var groupNameList = new List<string>();
                    foreach (ListItem item in CblGroupNameCollection.Items)
                    {
                        if (item.Selected)
                        {
                            groupNameList.Add(item.Value);
                        }
                    }

                    foreach (var channelId in _idsDictionary.Keys)
                    {
                        var tableName = ChannelManager.GetTableName(SiteInfo, channelId);
                        var contentIdArrayList = _idsDictionary[channelId];
                        if (contentIdArrayList != null)
                        {
                            foreach (var contentId in contentIdArrayList)
                            {
                                DataProvider.ContentDao.AddContentGroupList(tableName, contentId, groupNameList);
                            }
                        }
                    }

                    AuthRequest.AddSiteLog(SiteId, "添加内容到内容组", $"内容组:{TranslateUtils.ObjectCollectionToString(groupNameList)}");

                    isChanged = true;
                }
                else
                {

                    var groupNameList = new List<string>();
                    foreach (ListItem item in CblGroupNameCollection.Items)
                    {
                        if (item.Selected) groupNameList.Add(item.Value);
                    }

                    foreach (int channelId in _channelIdArrayList)
                    {
                        DataProvider.ChannelDao.AddGroupNameList(SiteId, channelId, groupNameList);
                    }

                    AuthRequest.AddSiteLog(SiteId, "添加栏目到栏目组", $"栏目组:{TranslateUtils.ObjectCollectionToString(groupNameList)}");

                    isChanged = true;
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
                isChanged = false;
            }

            if (isChanged)
            {
                LayerUtils.Close(Page);
            }
        }
    }
}