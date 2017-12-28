using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Plugins
{
	public class PageSeoMetaMatch : BasePageCms
    {
		public ListBox NodeIdCollectionToMatch;
		public ListBox ChannelSeoMetaId;
		public ListBox ContentSeoMetaId;
		
		bool[] _isLastNodeArray;
		string _defaultSeoMetaName;

		public string GetTitle(int nodeId, string nodeName, int parentsCount, bool isLastNode)
		{
			var str = "";
            if (nodeId == PublishmentSystemId)
			{
				isLastNode = true;
			}
			if (isLastNode == false)
			{
				_isLastNodeArray[parentsCount] = false;
			}
			else
			{
				_isLastNodeArray[parentsCount] = true;
			}
			for (var i = 0; i < parentsCount; i++)
			{
			    str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
			}
		    str = string.Concat(str, isLastNode ? "└" : "├");
		    str = string.Concat(str, StringUtils.MaxLengthText(nodeName, 8));

			var channelSeoMetaId = DataProvider.SeoMetasInNodesDao.GetSeoMetaIdByNodeId(nodeId, true);
			var channelSeoMetaName = string.Empty;
			if (channelSeoMetaId != 0)
			{
				channelSeoMetaName = DataProvider.SeoMetaDao.GetSeoMetaName(channelSeoMetaId);
			}
			if (string.IsNullOrEmpty(channelSeoMetaName))
			{
				channelSeoMetaName = _defaultSeoMetaName;
			}
			str = string.Concat(str, $" ({channelSeoMetaName})");

			var contentSeoMetaId = DataProvider.SeoMetasInNodesDao.GetSeoMetaIdByNodeId(nodeId, false);
			var contentSeoMetaName = string.Empty;
			if (contentSeoMetaId != 0)
			{
				contentSeoMetaName = DataProvider.SeoMetaDao.GetSeoMetaName(contentSeoMetaId);
			}
			if (string.IsNullOrEmpty(contentSeoMetaName))
			{
				contentSeoMetaName = _defaultSeoMetaName;
			}
			str = string.Concat(str, $" ({contentSeoMetaName})");
			
			return str;
		}
		

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var defaultSeoMetaId = DataProvider.SeoMetaDao.GetDefaultSeoMetaId(PublishmentSystemId);
			_defaultSeoMetaName = defaultSeoMetaId == 0 ? "<无>" : DataProvider.SeoMetaDao.GetSeoMetaName(defaultSeoMetaId);

			if (!IsPostBack)
			{
				BindListBox();
			}
		}


		public void BindListBox()
		{
			var selectedNodeIdList = new List<string>();
			foreach (ListItem listitem in NodeIdCollectionToMatch.Items)
			{
				if (listitem.Selected) selectedNodeIdList.Add(listitem.Value);
			}
			var selectedChannelSeoMetaId = ChannelSeoMetaId.SelectedValue;
			var selectedContentSeoMetaId = ContentSeoMetaId.SelectedValue;

			NodeIdCollectionToMatch.Items.Clear();
			ChannelSeoMetaId.Items.Clear();
			ContentSeoMetaId.Items.Clear();
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
            var nodeCount = nodeIdList.Count;
			_isLastNodeArray = new bool[nodeCount];
            foreach (int theNodeId in nodeIdList)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeId);
				var listitem = new ListItem(GetTitle(nodeInfo.NodeId, nodeInfo.NodeName, nodeInfo.ParentsCount, nodeInfo.IsLastNode), nodeInfo.NodeId.ToString());
				NodeIdCollectionToMatch.Items.Add(listitem);
			}

            var seoMetaInfoArrayList = DataProvider.SeoMetaDao.GetSeoMetaInfoArrayListByPublishmentSystemId(PublishmentSystemId);
			foreach (SeoMetaInfo seoMetaInfo in seoMetaInfoArrayList)
			{
				var listitem = new ListItem(seoMetaInfo.SeoMetaName, seoMetaInfo.SeoMetaId.ToString());
				ChannelSeoMetaId.Items.Add(listitem);
				ContentSeoMetaId.Items.Add(listitem);
			}

			ControlUtils.SelectMultiItems(NodeIdCollectionToMatch, selectedNodeIdList);
			ControlUtils.SelectSingleItem(ChannelSeoMetaId, selectedChannelSeoMetaId);
			ControlUtils.SelectSingleItem(ContentSeoMetaId, selectedContentSeoMetaId);
		}


		public void MatchChannelSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(true, true))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIdCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var channelSeoMetaId = int.Parse(ChannelSeoMetaId.SelectedValue);
					Process(nodeIdArrayList, channelSeoMetaId, true);
				}
			}
		}

		public void RemoveChannelSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, true))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIdCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var channelSeoMetaId = 0;
					Process(nodeIdArrayList, channelSeoMetaId, true);
				}
			}
		}

		public void MatchContentSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(true, false))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIdCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var contentSeoMetaId = int.Parse(ContentSeoMetaId.SelectedValue);
					Process(nodeIdArrayList, contentSeoMetaId, false);
				}
			}
		}

		public void RemoveContentSeoMetaButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, false))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIdCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var contentSeoMetaId = 0;
					Process(nodeIdArrayList, contentSeoMetaId, false);
				}
			}
		}

		private bool Validate(bool isMatch, bool isChannelSeoMeta)
		{
			if (NodeIdCollectionToMatch.SelectedIndex < 0)
			{
				FailMessage("请选择栏目！");
				return false;
			}
			if (isMatch)
			{
				if (isChannelSeoMeta)
				{
					if (ChannelSeoMetaId.SelectedIndex < 0)
					{
                        FailMessage("请选择栏目页元数据！");
						return false;
					}
				}
				else
				{
					if (ContentSeoMetaId.SelectedIndex < 0)
					{
                        FailMessage("请选择内容页元数据！");
						return false;
					}
				}
			}
			return true;
		}

		private void Process(ArrayList nodeIdArrayList, int seoMetaId, bool isChannel)
		{
			if (nodeIdArrayList != null && nodeIdArrayList.Count > 0)
			{
				foreach (int nodeId in nodeIdArrayList)
				{
					if (seoMetaId == 0)
					{
						DataProvider.SeoMetasInNodesDao.DeleteMatch(PublishmentSystemId, nodeId, isChannel);
					}
					else
					{
                        DataProvider.SeoMetasInNodesDao.InsertMatch(PublishmentSystemId, nodeId, seoMetaId, isChannel);
					}
				}
			}
			BindListBox();
		    SuccessMessage(seoMetaId == 0 ? "取消匹配成功！" : "模板匹配成功！");
		}

	}
}
