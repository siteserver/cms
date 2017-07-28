using System;
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateMatch : BasePageCms
    {
		public ListBox NodeIDCollectionToMatch;
		public ListBox ChannelTemplateID;
		public ListBox ContentTemplateID;
        public PlaceHolder phCreate;
		
		bool[] IsLastNodeArray;
		string defaultChannelTemplateName;
		string defaultContentTemplateName;

        public string GetTitle(NodeInfo nodeInfo)
		{
			var str = string.Empty;
			if (nodeInfo.NodeId == PublishmentSystemId)
			{
                nodeInfo.IsLastNode = true;
			}
            if (nodeInfo.IsLastNode == false)
			{
                IsLastNodeArray[nodeInfo.ParentsCount] = false;
			}
			else
			{
                IsLastNodeArray[nodeInfo.ParentsCount] = true;
			}
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
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
            if (nodeInfo.IsLastNode)
			{
				str = string.Concat(str, "└");
			}
			else
			{
				str = string.Concat(str, "├");
			}
            str = string.Concat(str, StringUtils.MaxLengthText(nodeInfo.NodeName, 8));


            if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
			{
                var indexTemplateID = TemplateManager.GetDefaultTemplateID(PublishmentSystemId, ETemplateType.IndexPageTemplate);
                var indexTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, indexTemplateID);
				str = string.Concat(str, $" ({indexTemplateName})");
            }
            else
            {
                var channelTemplateID = nodeInfo.ChannelTemplateId;
                var contentTemplateID = nodeInfo.ContentTemplateId;

                var channelTemplateName = string.Empty;
                if (channelTemplateID != 0)
                {
                    channelTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, channelTemplateID);
                }
                if (string.IsNullOrEmpty(channelTemplateName))
                {
                    channelTemplateName = defaultChannelTemplateName;
                }
                str = string.Concat(str, $" ({channelTemplateName})");

                var contentTemplateName = string.Empty;
                if (contentTemplateID != 0)
                {
                    contentTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, contentTemplateID);
                }
                if (string.IsNullOrEmpty(contentTemplateName))
                {
                    contentTemplateName = defaultContentTemplateName;
                }
                str = string.Concat(str, $" ({contentTemplateName})");
            }
			
			return str;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var defaultChannelTemplateID = TemplateManager.GetDefaultTemplateID(PublishmentSystemId, ETemplateType.ChannelTemplate);
            defaultChannelTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, defaultChannelTemplateID);

            var defaultContentTemplateID = TemplateManager.GetDefaultTemplateID(PublishmentSystemId, ETemplateType.ContentTemplate);
            defaultContentTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, defaultContentTemplateID);

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "匹配模板", AppManager.Cms.Permission.WebSite.Template);

                ChannelTemplateID.Attributes.Add("onfocus", "$('ContentTemplateID').selectedIndex = -1");
                ContentTemplateID.Attributes.Add("onfocus", "$('ChannelTemplateID').selectedIndex = -1");

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
			var selectedChannelTemplateID = ChannelTemplateID.SelectedValue;
			var selectedContentTemplateID = ContentTemplateID.SelectedValue;

			NodeIDCollectionToMatch.Items.Clear();
			ChannelTemplateID.Items.Clear();
			ContentTemplateID.Items.Clear();
			var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
            var nodeCount = nodeIdList.Count;
			IsLastNodeArray = new bool[nodeCount];
            foreach (int theNodeID in nodeIdList)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeID);
                var listitem = new ListItem(GetTitle(nodeInfo), nodeInfo.NodeId.ToString());
				NodeIDCollectionToMatch.Items.Add(listitem);
			}

            ChannelTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
            ContentTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);
			DataBind();

			var stringArray = new string[selectedNodeIDArrayList.Count];
			selectedNodeIDArrayList.CopyTo(stringArray);
			ControlUtils.SelectListItems(NodeIDCollectionToMatch, stringArray);
			ControlUtils.SelectListItems(ChannelTemplateID, selectedChannelTemplateID);
			ControlUtils.SelectListItems(ContentTemplateID, selectedContentTemplateID);
		}


		public void MatchChannelTemplateButton_OnClick(object sender, EventArgs e)
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
					var channelTemplateID = int.Parse(ChannelTemplateID.SelectedValue);
					Process(nodeIDArrayList, channelTemplateID, true);
				}
			}
		}

		public void RemoveChannelTemplateButton_OnClick(object sender, EventArgs e)
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
					var channelTemplateID = 0;
					Process(nodeIDArrayList, channelTemplateID, true);
				}
			}
		}

		public void MatchContentTemplateButton_OnClick(object sender, EventArgs e)
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
					var contentTemplateID = int.Parse(ContentTemplateID.SelectedValue);
					Process(nodeIDArrayList, contentTemplateID, false);
				}
			}
		}

		public void RemoveContentTemplateButton_OnClick(object sender, EventArgs e)
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
					var contentTemplateID = 0;
					Process(nodeIDArrayList, contentTemplateID, false);
				}
			}
		}

		private bool Validate(bool isMatch, bool isChannelTemplate)
		{
			if (NodeIDCollectionToMatch.SelectedIndex < 0)
			{
                FailMessage("请选择栏目！");
				return false;
			}
			if (isMatch)
			{
				if (isChannelTemplate)
				{
					if (ChannelTemplateID.SelectedIndex < 0)
					{
                        FailMessage("请选择栏目模板！");
						return false;
					}
				}
				else
				{
					if (ContentTemplateID.SelectedIndex < 0)
					{
                        FailMessage("请选择内容模板！");
						return false;
					}
				}
			}
			return true;
		}

		private void Process(ArrayList nodeIDArrayList, int templateID, bool isChannelTemplate)
		{
			if (nodeIDArrayList != null && nodeIDArrayList.Count > 0)
			{
                if (isChannelTemplate)
                {
                    foreach (int nodeID in nodeIDArrayList)
                    {
                        TemplateManager.UpdateChannelTemplateID(PublishmentSystemId, nodeID, templateID);
                    }
                }
                else
                {
                    foreach (int nodeID in nodeIDArrayList)
                    {
                        TemplateManager.UpdateContentTemplateID(PublishmentSystemId, nodeID, templateID);
                    }
                }
			}
			
			if (templateID == 0)
			{
                Body.AddSiteLog(PublishmentSystemId, "取消模板匹配", $"栏目:{GetNodeNames()}");
				SuccessMessage("取消匹配成功！");
			}
			else
			{
                Body.AddSiteLog(PublishmentSystemId, "模板匹配", $"栏目:{GetNodeNames()}");
				SuccessMessage("模板匹配成功！");
			}
            
            BindListBox();
		}

        private string GetNodeNames()
        {
            var builder = new StringBuilder();
            foreach (ListItem listItem in NodeIDCollectionToMatch.Items)
            {
                if (listItem.Selected)
                {
                    builder.Append(listItem.Text).Append(",");
                }
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }

            return RegexUtils.Replace("\\(.*?\\)|│|├|　|└", builder.ToString(), string.Empty);
        }

		public void CreateChannelTemplate_Click(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, false))
				{
                    var defaultChannelTemplateID = TemplateManager.GetDefaultTemplateID(PublishmentSystemId, ETemplateType.ChannelTemplate);
                    var relatedFileNameArrayList = DataProvider.TemplateDao.GetLowerRelatedFileNameArrayList(PublishmentSystemId, ETemplateType.ChannelTemplate);
                    var templateNameArrayList = DataProvider.TemplateDao.GetTemplateNameArrayList(PublishmentSystemId, ETemplateType.ChannelTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);
							var channelTemplateID = -1;

                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
							if (nodeInfo.NodeType != ENodeType.BackgroundPublishNode)
							{
                                channelTemplateID = nodeInfo.ChannelTemplateId;
							}

							if (channelTemplateID != -1 && channelTemplateID != 0 && channelTemplateID != defaultChannelTemplateID)
							{
                                if (TemplateManager.GetTemplateInfo(PublishmentSystemId, channelTemplateID) == null)
								{
									channelTemplateID = -1;
								}
							}

							if (channelTemplateID != -1)
							{
                                var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName, ETemplateType.ChannelTemplate, "T_" + nodeInfo.NodeName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
								if (relatedFileNameArrayList.Contains(templateInfo.RelatedFileName.ToLower()))
								{
									continue;
								}
								else if (templateNameArrayList.Contains(templateInfo.TemplateName))
								{
									continue;
								}
								var insertedTemplateID = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
                                if (nodeInfo.NodeType != ENodeType.BackgroundPublishNode)
                                {
                                    TemplateManager.UpdateChannelTemplateID(PublishmentSystemId, nodeID, insertedTemplateID);
                                    //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(nodeID, insertedTemplateID);
                                }
								
							}
						}
					}

                    Body.AddSiteLog(PublishmentSystemId, "生成并匹配栏目模版", $"栏目:{GetNodeNames()}");

					SuccessMessage("生成栏目模版并匹配成功！");

                    BindListBox();
				}
			}
		}

		public void CreateSubChannelTemplate_Click(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, false))
				{
                    var relatedFileNameArrayList = DataProvider.TemplateDao.GetLowerRelatedFileNameArrayList(PublishmentSystemId, ETemplateType.ChannelTemplate);
                    var templateNameArrayList = DataProvider.TemplateDao.GetTemplateNameArrayList(PublishmentSystemId, ETemplateType.ChannelTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);
                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

                            var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName + "_下级", ETemplateType.ChannelTemplate, "T_" + nodeInfo.NodeName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
							if (relatedFileNameArrayList.Contains(templateInfo.RelatedFileName.ToLower()))
							{
								continue;
							}
							else if (templateNameArrayList.Contains(templateInfo.TemplateName))
							{
								continue;
							}
							var insertedTemplateID = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
							var childNodeIDArrayList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeID);
							foreach (int childNodeID in childNodeIDArrayList)
							{
                                TemplateManager.UpdateChannelTemplateID(PublishmentSystemId, childNodeID, insertedTemplateID);
								//DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childNodeID, insertedTemplateID);
							}
						}
					}

                    Body.AddSiteLog(PublishmentSystemId, "生成并匹配下级栏目模版", $"栏目:{GetNodeNames()}");

					SuccessMessage("生成下级栏目模版并匹配成功！");

                    BindListBox();
				}
			}
		}

		public void CreateContentTemplate_Click(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, false))
				{
                    var defaultContentTemplateID = TemplateManager.GetDefaultTemplateID(PublishmentSystemId, ETemplateType.ContentTemplate);
                    var relatedFileNameArrayList = DataProvider.TemplateDao.GetLowerRelatedFileNameArrayList(PublishmentSystemId, ETemplateType.ContentTemplate);
                    var templateNameArrayList = DataProvider.TemplateDao.GetTemplateNameArrayList(PublishmentSystemId, ETemplateType.ContentTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);

                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

                            var contentTemplateID = nodeInfo.ContentTemplateId;                            

							if (contentTemplateID != 0 && contentTemplateID != defaultContentTemplateID)
							{
                                if (TemplateManager.GetTemplateInfo(PublishmentSystemId, contentTemplateID) == null)
								{
									contentTemplateID = -1;
								}
							}

							if (contentTemplateID != -1)
							{
                                var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName, ETemplateType.ContentTemplate, "T_" + nodeInfo.NodeName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								if (relatedFileNameArrayList.Contains(templateInfo.RelatedFileName.ToLower()))
								{
									continue;
								}
								else if (templateNameArrayList.Contains(templateInfo.TemplateName))
								{
									continue;
								}
								var insertedTemplateID = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
                                TemplateManager.UpdateContentTemplateID(PublishmentSystemId, nodeID, insertedTemplateID);
								//DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(nodeID, insertedTemplateID);
							}
						}
					}

                    Body.AddSiteLog(PublishmentSystemId, "生成并匹配内容模版", $"栏目:{GetNodeNames()}");
					
					SuccessMessage("生成内容模版并匹配成功！");
                    
                    BindListBox();
				}
			}
		}

		public void CreateSubContentTemplate_Click(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, false))
				{
                    var relatedFileNameArrayList = DataProvider.TemplateDao.GetLowerRelatedFileNameArrayList(PublishmentSystemId, ETemplateType.ContentTemplate);
                    var templateNameArrayList = DataProvider.TemplateDao.GetTemplateNameArrayList(PublishmentSystemId, ETemplateType.ContentTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeID = int.Parse(item.Value);
                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

                            var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName + "_下级", ETemplateType.ContentTemplate, "T_" + nodeInfo.NodeName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
							if (relatedFileNameArrayList.Contains(templateInfo.RelatedFileName.ToLower()))
							{
								continue;
							}
							else if (templateNameArrayList.Contains(templateInfo.TemplateName))
							{
								continue;
							}
							var insertedTemplateID = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
							var childNodeIDArrayList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeID);
							foreach (int childNodeID in childNodeIDArrayList)
							{
                                TemplateManager.UpdateContentTemplateID(PublishmentSystemId, childNodeID, insertedTemplateID);
								//DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childNodeID, insertedTemplateID);
							}
						}
					}

                    Body.AddSiteLog(PublishmentSystemId, "生成并匹配下级内容模版", $"栏目:{GetNodeNames()}");
					
					SuccessMessage("生成下级内容模版并匹配成功！");
                    
                    BindListBox();
				}
			}
		}
	}
}
