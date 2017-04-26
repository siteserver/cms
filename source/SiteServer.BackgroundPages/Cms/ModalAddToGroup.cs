using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalAddToGroup : BasePageCms
    {
        protected CheckBoxList cblGroupNameCollection;
        protected Button btnAddGroup;

        private bool _isContent;
        private Dictionary<int, List<int>> _idsDictionary = new Dictionary<int, List<int>>();
        private List<int> _nodeIdArrayList = new List<int>();

        public static string GetOpenWindowStringToContentForMultiChannels(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("添加到内容组",
                PageUtils.GetCmsUrl(nameof(ModalAddToGroup), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"IsContent", "True"}
                }), "IDsCollection", "请选择需要设置组别的内容！", 450, 420);
        }

        public static string GetOpenWindowStringToContent(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("添加到内容组",
                PageUtils.GetCmsUrl(nameof(ModalAddToGroup), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()},
                    {"IsContent", "True"}
                }), "ContentIDCollection", "请选择需要设置组别的内容！", 450, 420);
        }

        public static string GetOpenWindowStringToChannel(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("添加到栏目组",
                PageUtils.GetCmsUrl(nameof(ModalAddToGroup), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"IsContent", "False"}
                }), "ChannelIDCollection", "请选择需要设置组别的栏目！", 450, 420);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("IsContent"))
            {
                _isContent = Body.GetQueryBool("IsContent");
            }
            if (_isContent)
            {
                btnAddGroup.Text = " 新建内容组";
                _idsDictionary = ContentUtility.GetIDsDictionary(Request.QueryString);
            }
            else
            {
                btnAddGroup.Text = " 新建栏目组";
                _nodeIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection"));
            }
            if (!IsPostBack)
            {
                if (_isContent)
                {
                    var contentGroupNameList = DataProvider.ContentGroupDao.GetContentGroupNameList(PublishmentSystemId);
                    foreach (var groupName in contentGroupNameList)
                    {
                        var item = new ListItem(groupName, groupName);
                        cblGroupNameCollection.Items.Add(item);
                    }
                    var showPopWinString = ModalContentGroupAdd.GetOpenWindowString(PublishmentSystemId);
                    btnAddGroup.Attributes.Add("onclick", showPopWinString);
                }
                else
                {
                    var nodeGroupNameList = DataProvider.NodeGroupDao.GetNodeGroupNameList(PublishmentSystemId);
                    foreach (var groupName in nodeGroupNameList)
                    {
                        var item = new ListItem(groupName, groupName);
                        cblGroupNameCollection.Items.Add(item);
                    }

                    var showPopWinString = ModalNodeGroupAdd.GetOpenWindowString(PublishmentSystemId);
                    btnAddGroup.Attributes.Add("onclick", showPopWinString);
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
                    foreach (ListItem item in cblGroupNameCollection.Items)
                    {
                        if (item.Selected)
                        {
                            groupNameList.Add(item.Value);
                        }
                    }

                    foreach (var nodeId in _idsDictionary.Keys)
                    {
                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);
                        var contentIdArrayList = _idsDictionary[nodeId];
                        if (contentIdArrayList != null)
                        {
                            foreach (var contentId in contentIdArrayList)
                            {
                                BaiRongDataProvider.ContentDao.AddContentGroupList(tableName, contentId, groupNameList);
                            }
                        }
                    }

                    Body.AddSiteLog(PublishmentSystemId, "添加内容到内容组", $"内容组:{TranslateUtils.ObjectCollectionToString(groupNameList)}");

                    isChanged = true;
                }
                else
                {

                    var groupNameList = new List<string>();
                    foreach (ListItem item in cblGroupNameCollection.Items)
                    {
                        if (item.Selected) groupNameList.Add(item.Value);
                    }

                    foreach (int nodeId in _nodeIdArrayList)
                    {
                        DataProvider.NodeDao.AddNodeGroupList(PublishmentSystemId, nodeId, groupNameList);
                    }

                    Body.AddSiteLog(PublishmentSystemId, "添加栏目到栏目组", $"栏目组:{TranslateUtils.ObjectCollectionToString(groupNameList)}");

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
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}