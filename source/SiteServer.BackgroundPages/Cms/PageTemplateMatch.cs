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
                var indexTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.IndexPageTemplate);
                var indexTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, indexTemplateId);
				str = string.Concat(str, $" ({indexTemplateName})");
            }
            else
            {
                var channelTemplateId = nodeInfo.ChannelTemplateId;
                var contentTemplateId = nodeInfo.ContentTemplateId;

                var channelTemplateName = string.Empty;
                if (channelTemplateId != 0)
                {
                    channelTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, channelTemplateId);
                }
                if (string.IsNullOrEmpty(channelTemplateName))
                {
                    channelTemplateName = defaultChannelTemplateName;
                }
                str = string.Concat(str, $" ({channelTemplateName})");

                var contentTemplateName = string.Empty;
                if (contentTemplateId != 0)
                {
                    contentTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, contentTemplateId);
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

            var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ChannelTemplate);
            defaultChannelTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, defaultChannelTemplateId);

            var defaultContentTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ContentTemplate);
            defaultContentTemplateName = TemplateManager.GetTemplateName(PublishmentSystemId, defaultContentTemplateId);

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "匹配模板", AppManager.Permissions.WebSite.Template);

                ChannelTemplateID.Attributes.Add("onfocus", "$('ContentTemplateID').selectedIndex = -1");
                ContentTemplateID.Attributes.Add("onfocus", "$('ChannelTemplateID').selectedIndex = -1");

				BindListBox();
			}
		}


		public void BindListBox()
		{
			var selectedNodeIdArrayList = new ArrayList();
			foreach (ListItem listitem in NodeIDCollectionToMatch.Items)
			{
				if (listitem.Selected) selectedNodeIdArrayList.Add(listitem.Value);
			}
			var selectedChannelTemplateId = ChannelTemplateID.SelectedValue;
			var selectedContentTemplateId = ContentTemplateID.SelectedValue;

			NodeIDCollectionToMatch.Items.Clear();
			ChannelTemplateID.Items.Clear();
			ContentTemplateID.Items.Clear();
			var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
            var nodeCount = nodeIdList.Count;
			IsLastNodeArray = new bool[nodeCount];
            foreach (var theNodeId in nodeIdList)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeId);
                var listitem = new ListItem(GetTitle(nodeInfo), nodeInfo.NodeId.ToString());
				NodeIDCollectionToMatch.Items.Add(listitem);
			}

            ChannelTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ChannelTemplate);
            ContentTemplateID.DataSource = DataProvider.TemplateDao.GetDataSourceByType(PublishmentSystemId, ETemplateType.ContentTemplate);
			DataBind();

			var stringArray = new string[selectedNodeIdArrayList.Count];
			selectedNodeIdArrayList.CopyTo(stringArray);
			ControlUtils.SelectListItems(NodeIDCollectionToMatch, stringArray);
			ControlUtils.SelectListItems(ChannelTemplateID, selectedChannelTemplateId);
			ControlUtils.SelectListItems(ContentTemplateID, selectedContentTemplateId);
		}


		public void MatchChannelTemplateButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(true, true))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var channelTemplateId = int.Parse(ChannelTemplateID.SelectedValue);
					Process(nodeIdArrayList, channelTemplateId, true);
				}
			}
		}

		public void RemoveChannelTemplateButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, true))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var channelTemplateID = 0;
					Process(nodeIdArrayList, channelTemplateID, true);
				}
			}
		}

		public void MatchContentTemplateButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(true, false))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var contentTemplateId = int.Parse(ContentTemplateID.SelectedValue);
					Process(nodeIdArrayList, contentTemplateId, false);
				}
			}
		}

		public void RemoveContentTemplateButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Validate(false, false))
				{
					var nodeIdArrayList = new ArrayList();
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							nodeIdArrayList.Add(nodeId);
						}
					}
					var contentTemplateID = 0;
					Process(nodeIdArrayList, contentTemplateID, false);
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

		private void Process(ArrayList nodeIdArrayList, int templateId, bool isChannelTemplate)
		{
			if (nodeIdArrayList != null && nodeIdArrayList.Count > 0)
			{
                if (isChannelTemplate)
                {
                    foreach (int nodeId in nodeIdArrayList)
                    {
                        TemplateManager.UpdateChannelTemplateId(PublishmentSystemId, nodeId, templateId);
                    }
                }
                else
                {
                    foreach (int nodeId in nodeIdArrayList)
                    {
                        TemplateManager.UpdateContentTemplateId(PublishmentSystemId, nodeId, templateId);
                    }
                }
			}
			
			if (templateId == 0)
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
                    var defaultChannelTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ChannelTemplate);
                    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
                    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
							var channelTemplateId = -1;

                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
							if (nodeInfo.NodeType != ENodeType.BackgroundPublishNode)
							{
                                channelTemplateId = nodeInfo.ChannelTemplateId;
							}

							if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
							{
                                if (TemplateManager.GetTemplateInfo(PublishmentSystemId, channelTemplateId) == null)
								{
									channelTemplateId = -1;
								}
							}

							if (channelTemplateId != -1)
							{
                                var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName, ETemplateType.ChannelTemplate, "T_" + nodeInfo.NodeName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
								if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
								{
									continue;
								}
								else if (templateNameList.Contains(templateInfo.TemplateName))
								{
									continue;
								}
								var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
                                if (nodeInfo.NodeType != ENodeType.BackgroundPublishNode)
                                {
                                    TemplateManager.UpdateChannelTemplateId(PublishmentSystemId, nodeId, insertedTemplateId);
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
                    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
                    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ChannelTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

                            var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName + "_下级", ETemplateType.ChannelTemplate, "T_" + nodeInfo.NodeName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
							if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
							{
								continue;
							}
							else if (templateNameList.Contains(templateInfo.TemplateName))
							{
								continue;
							}
							var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
							var childNodeIdArrayList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
							foreach (var childNodeId in childNodeIdArrayList)
							{
                                TemplateManager.UpdateChannelTemplateId(PublishmentSystemId, childNodeId, insertedTemplateId);
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
                    var defaultContentTemplateId = TemplateManager.GetDefaultTemplateId(PublishmentSystemId, ETemplateType.ContentTemplate);
                    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
                    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);

                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

                            var contentTemplateId = nodeInfo.ContentTemplateId;                            

							if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
							{
                                if (TemplateManager.GetTemplateInfo(PublishmentSystemId, contentTemplateId) == null)
								{
									contentTemplateId = -1;
								}
							}

							if (contentTemplateId != -1)
							{
                                var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName, ETemplateType.ContentTemplate, "T_" + nodeInfo.NodeName + ".html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
								{
									continue;
								}
								else if (templateNameList.Contains(templateInfo.TemplateName))
								{
									continue;
								}
								var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
                                TemplateManager.UpdateContentTemplateId(PublishmentSystemId, nodeId, insertedTemplateId);
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
                    var relatedFileNameList = DataProvider.TemplateDao.GetLowerRelatedFileNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
                    var templateNameList = DataProvider.TemplateDao.GetTemplateNameList(PublishmentSystemId, ETemplateType.ContentTemplate);
					foreach (ListItem item in NodeIDCollectionToMatch.Items)
					{
						if (item.Selected)
						{
							var nodeId = int.Parse(item.Value);
                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

                            var templateInfo = new TemplateInfo(0, PublishmentSystemId, nodeInfo.NodeName + "_下级", ETemplateType.ContentTemplate, "T_" + nodeInfo.NodeName + "_下级.html", "index.html", ".html", ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
								
							if (relatedFileNameList.Contains(templateInfo.RelatedFileName.ToLower()))
							{
								continue;
							}
							else if (templateNameList.Contains(templateInfo.TemplateName))
							{
								continue;
							}
							var insertedTemplateId = DataProvider.TemplateDao.Insert(templateInfo, string.Empty, Body.AdministratorName);
							var childNodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
							foreach (var childNodeId in childNodeIdList)
							{
                                TemplateManager.UpdateContentTemplateId(PublishmentSystemId, childNodeId, insertedTemplateId);
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
