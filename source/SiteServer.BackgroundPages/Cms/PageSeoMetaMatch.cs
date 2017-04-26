using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageSeoMetaMatch : BasePageCms
    {
		public ListBox NodeIDCollectionToMatch;
		public ListBox ChannelSeoMetaID;
		public ListBox ContentSeoMetaID;
		
		bool[] IsLastNodeArray;
		string defaultSeoMetaName;

		public string GetTitle(int nodeID, string nodeName, ENodeType nodeType, int parentsCount, bool isLastNode)
		{
			var str = "";
            if (nodeID == PublishmentSystemId)
			{
				isLastNode = true;
			}
			if (isLastNode == false)
			{
				IsLastNodeArray[parentsCount] = false;
			}
			else
			{
				IsLastNodeArray[parentsCount] = true;
			}
			for (var i = 0; i < parentsCount; i++)
			{
				if (IsLastNodeArray[i])
				{
					str = string.Concat(str, "　");
				}
				else
				{
					str = string.Concat(str, "│");
				}
			}
			if (isLastNode)
			{
				str = string.Concat(str, "└");
			}
			else
			{
				str = string.Concat(str, "├");
			}
			str = string.Concat(str, StringUtils.MaxLengthText(nodeName, 8));

			var channelSeoMetaID = DataProvider.SeoMetaDao.GetSeoMetaIdByNodeId(nodeID, true);
			var channelSeoMetaName = string.Empty;
			if (channelSeoMetaID != 0)
			{
				channelSeoMetaName = DataProvider.SeoMetaDao.GetSeoMetaName(channelSeoMetaID);
			}
			if (string.IsNullOrEmpty(channelSeoMetaName))
			{
				channelSeoMetaName = defaultSeoMetaName;
			}
			str = string.Concat(str, $" ({channelSeoMetaName})");

			var contentSeoMetaID = DataProvider.SeoMetaDao.GetSeoMetaIdByNodeId(nodeID, false);
			var contentSeoMetaName = string.Empty;
			if (contentSeoMetaID != 0)
			{
				contentSeoMetaName = DataProvider.SeoMetaDao.GetSeoMetaName(contentSeoMetaID);
			}
			if (string.IsNullOrEmpty(contentSeoMetaName))
			{
				contentSeoMetaName = defaultSeoMetaName;
			}
			str = string.Concat(str, $" ({contentSeoMetaName})");
			
			return str;
		}
		

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var defaultSeoMetaID = DataProvider.SeoMetaDao.GetDefaultSeoMetaId(PublishmentSystemId);
			if (defaultSeoMetaID == 0)
			{
				defaultSeoMetaName = "<无>";
			}
			else
			{
				defaultSeoMetaName = DataProvider.SeoMetaDao.GetSeoMetaName(defaultSeoMetaID);
			}

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdSeo, "页面元数据(Meta) / 页面元数据匹配", AppManager.Cms.Permission.WebSite.Seo);

				BindListBox();
			}
		}


		public void BindListBox()
		{
			var selectedNodeIDArrayList = new ArrayList();
			foreach (ListItem listitem in NodeIDCollectionToMatch.Items)
			{
				if (listitem.Selected) selectedNodeIDArrayList.Add(listitem.Value);
			}
			var selectedChannelSeoMetaID = ChannelSeoMetaID.SelectedValue;
			var selectedContentSeoMetaID = ContentSeoMetaID.SelectedValue;

			NodeIDCollectionToMatch.Items.Clear();
			ChannelSeoMetaID.Items.Clear();
			ContentSeoMetaID.Items.Clear();
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
            var nodeCount = nodeIdList.Count;
			IsLastNodeArray = new bool[nodeCount];
            foreach (int theNodeID in nodeIdList)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeID);
				var listitem = new ListItem(GetTitle(nodeInfo.NodeId, nodeInfo.NodeName, nodeInfo.NodeType, nodeInfo.ParentsCount, nodeInfo.IsLastNode), nodeInfo.NodeId.ToString());
				NodeIDCollectionToMatch.Items.Add(listitem);
			}

            var seoMetaInfoArrayList = DataProvider.SeoMetaDao.GetSeoMetaInfoArrayListByPublishmentSystemId(PublishmentSystemId);
			foreach (SeoMetaInfo seoMetaInfo in seoMetaInfoArrayList)
			{
				var listitem = new ListItem(seoMetaInfo.SeoMetaName, seoMetaInfo.SeoMetaId.ToString());
				ChannelSeoMetaID.Items.Add(listitem);
				ContentSeoMetaID.Items.Add(listitem);
			}

			var stringArray = new string[selectedNodeIDArrayList.Count];
			selectedNodeIDArrayList.CopyTo(stringArray);
			ControlUtils.SelectListItems(NodeIDCollectionToMatch, stringArray);
			ControlUtils.SelectListItems(ChannelSeoMetaID, selectedChannelSeoMetaID);
			ControlUtils.SelectListItems(ContentSeoMetaID, selectedContentSeoMetaID);
		}


		public void MatchChannelSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(true, true))
				{
					var nodeIDArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);
							nodeIDArrayList.Add(nodeID);
						}
					}
					var channelSeoMetaID = int.Parse(ChannelSeoMetaID.SelectedValue);
					Process(nodeIDArrayList, channelSeoMetaID, true);
				}
			}
		}

		public void RemoveChannelSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, true))
				{
					var nodeIDArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);
							nodeIDArrayList.Add(nodeID);
						}
					}
					var channelSeoMetaID = 0;
					Process(nodeIDArrayList, channelSeoMetaID, true);
				}
			}
		}

		public void MatchContentSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(true, false))
				{
					var nodeIDArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);
							nodeIDArrayList.Add(nodeID);
						}
					}
					var contentSeoMetaID = int.Parse(ContentSeoMetaID.SelectedValue);
					Process(nodeIDArrayList, contentSeoMetaID, false);
				}
			}
		}

		public void RemoveContentSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, false))
				{
					var nodeIDArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);
							nodeIDArrayList.Add(nodeID);
						}
					}
					var contentSeoMetaID = 0;
					Process(nodeIDArrayList, contentSeoMetaID, false);
				}
			}
		}

		private bool Validate(bool isMatch, bool isChannelSeoMeta)
		{
			if (NodeIDCollectionToMatch.SelectedIndex < 0)
			{
				FailMessage("请选择栏目！");
				return false;
			}
			if (isMatch)
			{
				if (isChannelSeoMeta)
				{
					if (ChannelSeoMetaID.SelectedIndex < 0)
					{
                        FailMessage("请选择栏目页元数据！");
						return false;
					}
				}
				else
				{
					if (ContentSeoMetaID.SelectedIndex < 0)
					{
                        FailMessage("请选择内容页元数据！");
						return false;
					}
				}
			}
			return true;
		}

		private void Process(ArrayList nodeIDArrayList, int seoMetaID, bool isChannel)
		{
			if (nodeIDArrayList != null && nodeIDArrayList.Count > 0)
			{
				foreach (int nodeID in nodeIDArrayList)
				{
					if (seoMetaID == 0)
					{
						DataProvider.SeoMetaDao.DeleteMatch(PublishmentSystemId, nodeID, isChannel);
					}
					else
					{
                        DataProvider.SeoMetaDao.InsertMatch(PublishmentSystemId, nodeID, seoMetaID, isChannel);
					}
				}
			}
			BindListBox();
			if (seoMetaID == 0)
			{
				SuccessMessage("取消匹配成功！");
			}
			else
			{
				SuccessMessage("模板匹配成功！");
			}
		}

	}
}
